using GameDefines;
using CharacterDefines;
using UIDefines;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using OptionDefines;

public class Player : Identity
{
    private Camera mainCamera;

    [SerializeField] private int Hp = 20;
    [SerializeField] private int money = 0;
    [SerializeField] public int RerollAttributeTypeTypeCount = 0;
    [SerializeField] public int RerollSpeciesTypeCount = 0;
    [SerializeField] private int RoundPeriodic = 5;

    public int AdventureCharacterLimitCount { get; private set; } = 10;   // 최대제한 카운트
    public int AdventureCharacterPickCount { get; private set; } = 0;    // 뽑기 캐릭터 카운트
    public int AdventureCharacterCount { get; private set; } = 0;   // 현재 카운트
    public int AdventureCharacterMaxCount { get; private set; } = 6;   // 뽑기 최대 카운트

    public bool isComplete = false;
    public bool isShowNotice = false;
    public bool isStopAction = false;
    private bool isDead = false;

    private Ray PlayerRay;
    private RaycastHit PlayerRayHit;

    public event Action<UIEventArgs> OnUpdatePlayerEvent;
    public event Action<UIEventArgs> OnDiePlayerEvent;

    public int selectedBuildingIndex { get; set; } = -1;
    public short addMoney { get; private set; } = 0;
    public bool isUIMouseOver = false;

    //Upgrade
    private Character selectTarget = null;
    private Dictionary<SpeciesType, int> CurrentUpgradeData;
    //Workman
    private List<Workman> workmanList = null;
    //Achievement
    private List<KeyAchievementInfo> VolatilityKeyAchievementInfoList = null;
    private List<UnitAchievementInfo> VolatilityUnitAchievementInfoList = null;
    private Dictionary<int, int> VolatilityKeyAchievementTryInfo = null;
    //Adventure
    private List<Player_Adventure> AdventureModePlayerList = null;
    private AdventureMoveController AdventureController = null;
    private PlayerAdventureBasicInfo MyPlayerAdventureBasicInfo;
    private List<SpeciesType> UseSpeciesTypeList = null;
    private Dictionary<AdventureLevelUpStatType, int> m_PlayerAdventureLevelUp = null;
    
    struct VolatilityUnitAchievementTryInfo
    {
        public List<SpeciesType> eSpeciesTypeList;
        public List<bool> bCheckList;

        public VolatilityUnitAchievementTryInfo(SpeciesType[] arrSpeciesType)
        {
            eSpeciesTypeList = new List<SpeciesType>();
            bCheckList = new List<bool>();

            foreach (SpeciesType eSpeciesType in arrSpeciesType)
            {
                eSpeciesTypeList.Add(eSpeciesType);
                bCheckList.Add(false);
            }
        }
    }

    void Awake()
    {
        mainCamera = Camera.main;
        isDead = false;

        VolatilityKeyAchievementInfoList = new List<KeyAchievementInfo>();
        VolatilityUnitAchievementInfoList = new List<UnitAchievementInfo>();
        VolatilityKeyAchievementTryInfo = new Dictionary<int, int>();
        CurrentUpgradeData = new Dictionary<SpeciesType, int>();
        UseSpeciesTypeList = new List<SpeciesType>();
        m_PlayerAdventureLevelUp = new Dictionary<AdventureLevelUpStatType, int>();
        for (int i = 0; i < (int)(AdventureLevelUpStatType.MAX); ++i)
        {
            m_PlayerAdventureLevelUp.Add((AdventureLevelUpStatType)i, 0);
        };

        AdventureModePlayerList = new List<Player_Adventure>();
        //공격,사거리,이속,점프,공속,타겟
        MyPlayerAdventureBasicInfo = new PlayerAdventureBasicInfo(0, 0f, 3f, 7f, 0f, 0);
    }
    void Start()
    {
        if (Oracle.m_eGameType == MapType.ADVENTURE)
        {
            List<SpeciesType> saveLastInfo = GameManager.Instance.GetAdventureModePlayerList();
            if (saveLastInfo != null)
            {
                selectedBuildingIndex = 4;

                int ibonusIndex = saveLastInfo.Count - AdventureCharacterMaxCount;
                for (int i = 0; i < saveLastInfo.Count; ++i)
                {
                    SetRandomSpeciesType_Adv(saveLastInfo[i], i < ibonusIndex);
                }

                Hp = GameManager.Instance.m_SaveCurrentHP;

                List<int> saveLastStatInfo = GameManager.Instance.GetPlayerAdventureLevelUpStat();
                if (saveLastStatInfo != null)
                {
                    for (int i = 0; i < saveLastStatInfo.Count; ++i)
                    {
                        AdventureLevelUpStatType eAdventureLevelUpStatType = (AdventureLevelUpStatType)i;
                        m_PlayerAdventureLevelUp[eAdventureLevelUpStatType] = saveLastStatInfo[i];
                    }

                    SetStat_PlayerAdventure();
                }
            }
            else
            {
                selectedBuildingIndex = 0;
                List<SpeciesType> useableSpeciesTypeList = UnlockManager.Instance.GetUseableSpeciesTypeList();

                //SetRandomSpeciesType_Adv(m_eSpeciesType);
                SetRandomSpeciesType_Adv(useableSpeciesTypeList[Oracle.RandomDice(0, useableSpeciesTypeList.Count)], false);
            }

            //Hp = 100000;
            OnUpdatePlayerEvent?.Invoke(new UIEventArgs { });
        }
        else
        {
            CursorInfo AddInfo = UnlockManager.Instance.GetCursorInfoList(OptionManager.Instance.iCursorIndex);
            if (AddInfo.Money != 0 && Oracle.m_eGameType != MapType.SPAWN)
            {
                addMoney = AddInfo.Money;
            }

            RefillUnitAchievementInfo();
        }
    }
    void Update()
    {
        if (isDead || isComplete)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (isUIMouseOver == false)
            {
                if (Oracle.m_eGameType == MapType.ADVENTURE)
                {
                    PlayerRaycast();
                }
                else
                {
                    if (selectedBuildingIndex >= 0)
                    {
                        SpawnBuilding();
                    }
                    else
                    {
                        PlayerRaycast();
                    }
                }
            }
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            OffPlayerUI();
            UIManager.Instance.HideUI(UIIndexType.MAININFO);
        }

        CheckKeyAchievement();

        if (Oracle.m_eGameType == MapType.ADVENTURE)
        {
            if (UnlockManager.Instance.bSteamDeactivate)
            {
                //Test//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    selectedBuildingIndex++;
                    if (selectedBuildingIndex > 4)
                        selectedBuildingIndex = 0;

                    ChoiceCharacter(m_eSpeciesType);
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    MonsterPool.Instance.ForceDeSpawnMonster();
                }
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    UIManager.Instance.ShowRoundPeriodic(GameManager.Instance.stageIndex);
                    GameManager.Instance.GameStop(true);
                }
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            }
        }
    }

    void LateUpdate()
    {
        if (Hp <= 0)
        {
            OnDie();
        }
    }

    private void PlayerRaycast()
    {
        selectTarget = null;
        PlayerRay = mainCamera.ScreenPointToRay(Input.mousePosition);

        bool bFindObject = false;
        RaycastHit[] hits = Physics.RaycastAll(PlayerRay);
        for(int i = 0; i < hits.Length; ++i)
        {
            PlayerRayHit = hits[i];
            if (PlayerRayHit.collider.CompareTag("Object"))
            {
                bFindObject = true;
                break;
            }
            else if (PlayerRayHit.collider.CompareTag("Item"))
            {
                ItemBase selectItem = PlayerRayHit.collider.GetComponent<ItemBase>();
                if (selectItem == null)
                {
                    Debug.Log("PlayerRaycast item Click error");
                    return;
                }

                selectItem.PickUp();
                return;
            }
            else if (PlayerRayHit.collider.CompareTag("Spawn"))
            {
                SpawnSphere selectSpawnSphere = PlayerRayHit.collider.GetComponent<SpawnSphere>();
                if (selectSpawnSphere == null)
                {
                    Debug.Log("PlayerRaycast SpawnSphere Click error");
                    return;
                }

                UIManager.Instance.ShowSpawnSphereUI(UIIndexType.MAININFO, selectSpawnSphere);
                return;
            }
        }

        if (bFindObject)
        {
            selectTarget = PlayerRayHit.collider.GetComponent<Character>();
            UIManager.Instance.ShowCharacterUI(UIIndexType.MAININFO, selectTarget, false);
        }
        else
        {
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                UIManager.Instance.HideUI(UIIndexType.MAININFO);
            }
        }
    }

    public void SetWave()
    {
        if (Oracle.m_eGameType == MapType.ADVENTURE)
        {
            if (GameManager.Instance.CheckActivateLastBoss(false) == false)
            {
                if (GameManager.Instance.stageIndex % RoundPeriodic == 0)
                {
                    UIManager.Instance.ShowRoundPeriodic(GameManager.Instance.stageIndex);
                    GameManager.Instance.GameStop(true);
                }
            }
        }
        else
            selectedBuildingIndex = -1;

        UIManager.Instance.ShowWaveUI(m_eSpeciesType);
    }

    public int GetHp()
    {
        return Hp;
    }

    public override void ReduceHP(int Damage, HitParticleType eHitParticleType = HitParticleType.NONE)
    {
        Hp -= (byte)Damage;
        if (Hp < 0)
            Hp = 0;

        OnUpdatePlayerEvent?.Invoke(new UIEventArgs { });
    }
    public void DivideHP(int Damage)
    {
        if (Hp < 2)
            return;

        Hp /= Damage;
        OnUpdatePlayerEvent?.Invoke(new UIEventArgs { });
    }

    public void IncreaseHP(byte Heal)
    {
        Hp += Heal;
        if (Hp > 9999)
            Hp = 9999;

        OnUpdatePlayerEvent?.Invoke(new UIEventArgs { });
    }

    public void GameOver()
    {
        ReduceHP(Hp);
    }

    void OnDie()
    {
        if (Oracle.m_eGameType == MapType.ADVENTURE)
        {
            if (GameManager.Instance.CheckActivateLastBoss(true))
            {
                OnDiePlayerEvent?.Invoke(new UIEventArgs(1) { });
                GameManager.Instance.SaveAdventureModePlayerList();
            }

            for (int i = 0; i < AdventureModePlayerList.Count; ++i)
            {
                AdventureModePlayerList[i].Ondie();

                Destroy(AdventureModePlayerList[i].gameObject);
                AdventureModePlayerList[i] = null;
            }
            AdventureModePlayerList.Clear();

            Destroy(AdventureController);
            ImpedimentsPool.Instance.OnDie();
            BossAdventure_Last_Manager.Instance.OnDie();
        }

        isDead = true;
        OnDiePlayerEvent?.Invoke(new UIEventArgs { });
        GameManager.Instance.isDie = true;
        GameManager.Instance.GameOver();

        if (GameManager.Instance.stageIndex == 1)
        {
            UnlockManager.Instance.CheckLoser();
        }

        gameObject.SetActive(false);
    }

    public void GameComplete(bool bComplete)
    {
        isComplete = bComplete;
    }

    public int GetMoney()
    {
        return money;
    }

    public void AddMoney(int addedmoney)
    {
        money += addedmoney;

        OnUpdatePlayerEvent?.Invoke(new UIEventArgs { });
    }

    // UI Communication
    public void SpawnBuilding()
    {
        if (Oracle.m_eGameType == MapType.ADVENTURE)
            return;

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        BuildingPool.Instance.SpawnBuilding(m_eSpeciesType, selectedBuildingIndex, mouseWorldPos);
    }

    public bool Refund(Building selectedBuilding)
    {
        if (selectedBuilding == null)
            return false;

        if (BuildingPool.Instance.RemoveBuilding(selectedBuilding, false)) 
        {
            SoundManager.Instance.PlayCharacterSfx(SpeciesType.NONE, CharacterState.SELL);
            float fRefundMoney = (float)selectedBuilding.Cost * 0.8f;
            AddMoney((int)fRefundMoney);
            isUIMouseOver = false;
            return true;
        }

        return false;
    }

    public void RerollAttributeType()
    {
        if (RerollAttributeTypeTypeCount <= 0)
        {
            return;
        }    

        m_eAttributeType = (AttributeType)Oracle.RandomDice(1, (int)AttributeType.MAX);
        --RerollAttributeTypeTypeCount;

        SetBasicInfo();

        UIManager.Instance.UpdateSpeciesType(m_eSpeciesType, m_eAttributeType);
    }

    public void RerollSpeciesType()
    {
        if (RerollSpeciesTypeCount <= 0)
        {
            return;
        }

        m_eSpeciesType = UnlockManager.Instance.GetRandomSpeciesType();
        --RerollSpeciesTypeCount;

        SetBasicInfo();

        UIManager.Instance.UpdateSpeciesType(m_eSpeciesType, m_eAttributeType);
    }

    public void OffPlayerUI()
    {
        if (Oracle.m_eGameType == MapType.ADVENTURE)
            return;

        UI_BuildingList UIBuildingList = UIManager.Instance.GetUI(UIIndexType.BUILDINGLIST) as UI_BuildingList;
        if (UIBuildingList)
        {
            UIBuildingList.OffBuildBuilding();
        }

        selectedBuildingIndex = -1;
    }

    public void InitUpgradeData()
    {
        CurrentUpgradeData.Clear();
        List<SpeciesType> deckSpeciesTypeList = BuildingPool.Instance.GetDeckSpeciesTypeList();
        for (int i = 0; i < deckSpeciesTypeList.Count; ++i)
        {
            CurrentUpgradeData.Add(deckSpeciesTypeList[i], 0);
        }

        UIManager.Instance.StartGame();
    }
    // 계산된 정보
    public ControlInfo GetCurrentUpgradeData(SpeciesType eSpeciesType)
    {
        if (CurrentUpgradeData.ContainsKey(eSpeciesType) == false)
            return new ControlInfo(0, 0, 0, 0, 0, 0, 0, null, null);

        ControlInfo controlInfo = ResourceAgent.Instance.GetControlInfoData();
        int UpgradeCount = CurrentUpgradeData[eSpeciesType];

        int UnitAtkMulti = 1;
        if (UpgradeCount >= 30)
        {
            controlInfo.Grade = 3;
            UnitAtkMulti = 8;
        }
        else if (UpgradeCount >= 20)
        {
            controlInfo.Grade = 2;
            UnitAtkMulti = 4;
        }
        else if (UpgradeCount >= 10)
        {
            controlInfo.Grade = 1;
            UnitAtkMulti = 2;
        }

        controlInfo.Cost *= (UpgradeCount+1);
        for (int i = 0; i < controlInfo.UnitAtk.Length; ++i)
        {
            controlInfo.UnitAtk[i] *= (UpgradeCount * UnitAtkMulti);
        }

        return controlInfo;
    }

    public void UpgradeComplete(SpeciesType eSpeciesType)
    {
        if (CurrentUpgradeData.ContainsKey(eSpeciesType) == false)
            return;

        ControlInfo controlInfoData = GetCurrentUpgradeData(eSpeciesType);
        if (money - controlInfoData.Cost < 0)
        {
            SoundManager.Instance.PlayUISound(UISoundType.UPGRADEFAIL);
            return;
        }

        CurrentUpgradeData[eSpeciesType]++;

        AddMoney(-controlInfoData.Cost);
        SoundManager.Instance.PlayUISound(UISoundType.UPGRADE);

        int UpgradeCount = CurrentUpgradeData[eSpeciesType];
        if (UpgradeCount >= 30)
        {
            controlInfoData.Grade = 3;
        }
        else if (UpgradeCount >= 20)
        {
            controlInfoData.Grade = 2;
        }
        else if (UpgradeCount >= 10)
        {
            controlInfoData.Grade = 1;
        }

        BuildingPool.Instance.SetBuildingBasicDamage(eSpeciesType, controlInfoData.Grade);

        if (selectTarget)
        {
            UIManager.Instance.ShowCharacterUI(UIIndexType.MAININFO, selectTarget, true);
        }
    }

    public bool CreateWorkman()
    {
        if (workmanList == null)
        {
            workmanList = new List<Workman>();
        }

        if (workmanList.Count >= 100)
            return true;

        ControlInfo controlInfo = ResourceAgent.Instance.GetControlInfoData();
        if (money - controlInfo.WorkmanCost < 0)
        {
            SoundManager.Instance.PlayUISound(UISoundType.UPGRADEFAIL);
            return false;
        }

        GameObject workmanObject = Instantiate(ResourceAgent.Instance.GetRandomEtcPrefab());
        if (workmanObject == null)
            return false;

        AddMoney(-controlInfo.WorkmanCost);
        SoundManager.Instance.PlayUISound(UISoundType.SPAWN);

        Workman workman = workmanObject.GetComponent<Workman>();
        if (workman)
        {
            workman.SetComponent(workman);
            workmanList.Add(workman);

            if (workmanList.Count >= 100)
            {
                for (int i = 0; i < workmanList.Count; ++i)
                {
                    Workman myWorkman = workmanList[i];
                    if (myWorkman == null)
                        continue;

                    myWorkman.ChangeMoveSpeedPercent(1.1f);
                }

                string strName = string.Format("Achieve <color=white>UNSCRUPULOUS OWNERS</color> - Get Move Speed +10%");
                UIManager.Instance.ShowNoticeText(strName, true);

                return true;
            }
        }

        return false;
    }

    public void RefillUnitAchievementInfo()
    {
        ResourceAgent.Instance.RefillUnitAchievementInfo(VolatilityKeyAchievementInfoList, VolatilityUnitAchievementInfoList);

        VolatilityKeyAchievementTryInfo.Clear();
        for (int i = 0; i < VolatilityKeyAchievementInfoList.Count; ++i)
        {
            VolatilityKeyAchievementTryInfo.Add(i, 0);
        }
    }

    private void CheckKeyAchievement()
    {
        if (VolatilityKeyAchievementInfoList == null || isShowNotice)
            return;

        if (VolatilityKeyAchievementTryInfo.Count == 0)
            return;

        string strName = "";
        int Money = 0;
        bool bSound = false;
        bool bFind = false;
        for (int i = 0; i < VolatilityKeyAchievementInfoList.Count; ++i)
        {
            if (Input.GetKeyUp(VolatilityKeyAchievementInfoList[i].ChoiceKey))
            {
                bFind = true;
                strName = VolatilityKeyAchievementInfoList[i].Name;
                Money = VolatilityKeyAchievementInfoList[i].Money;
                bSound = true;

                AddMoney(Money);
                strName = string.Format("Achieve <color=white>{0}</color> - Get money +{1}", strName, Money);

                UnlockManager.Instance.SetSpawnMapAchievement(VolatilityKeyAchievementInfoList[i].SteamName);

                VolatilityKeyAchievementInfoList.RemoveAt(i);
                VolatilityKeyAchievementTryInfo.Remove(i);
                break;
            }

            if (VolatilityKeyAchievementTryInfo[i] < 2)
            {
                for (int j = 0; j < VolatilityKeyAchievementInfoList[i].AllKeyCodes.Count; ++j)
                {
                    KeyCode key = VolatilityKeyAchievementInfoList[i].AllKeyCodes[j];
                    if (Input.GetKeyUp(key))
                    {
                        bFind = true;
                        if (VolatilityKeyAchievementTryInfo[i] == 0)
                        {
                            strName = string.Format("Incorrect Key! Give you {0} more chance", 1);
                            VolatilityKeyAchievementInfoList[i].AllKeyCodes.RemoveAt(j);
                            VolatilityKeyAchievementTryInfo[i]++;
                        }
                        else
                        {
                            strName = "What a bummer!";
                            VolatilityKeyAchievementInfoList.RemoveAt(i);
                            VolatilityKeyAchievementTryInfo.Remove(i);
                        }

                        strName = string.Format("<color=red>{0}</color>", strName);
                        break;
                    }
                }
            }
        }

        if (bFind)
        {
            isShowNotice = true;
            UIManager.Instance.ShowNoticeText(strName, bSound);
        }
    }

    public void CheckUnitAchievement()
    {
        List<GameObject> allBuildinglist = BuildingPool.Instance.GetBuildingList();
        if (allBuildinglist.Count < 2)
            return;

        VolatilityUnitAchievementTryInfo[] tempVolatilityUnitAchievementTryInfo = new VolatilityUnitAchievementTryInfo[VolatilityUnitAchievementInfoList.Count];
        for (int i = 0; i < VolatilityUnitAchievementInfoList.Count; ++i)
        {
            tempVolatilityUnitAchievementTryInfo[i] = new VolatilityUnitAchievementTryInfo(VolatilityUnitAchievementInfoList[i].AchievementSpeciesType);
        }

        foreach (GameObject BuildingObject in allBuildinglist)
        {
            Building building = BuildingObject.GetComponent<Building>();
            if (building == null)
                continue;
            
            foreach (VolatilityUnitAchievementTryInfo unitAchievementData in tempVolatilityUnitAchievementTryInfo)
            {
                for (int i = 0; i < unitAchievementData.eSpeciesTypeList.Count; ++i)
                {
                    if (building.m_eSpeciesType == unitAchievementData.eSpeciesTypeList[i])
                    {
                        if (unitAchievementData.bCheckList[i] == false)
                        {
                            unitAchievementData.bCheckList[i] = true;
                            break;
                        }
                    }
                }
            }
        }

        for (int i = 0; i < tempVolatilityUnitAchievementTryInfo.Length; ++i)
        {
            bool bSuccess = true;
            for (int j = 0; j < tempVolatilityUnitAchievementTryInfo[i].bCheckList.Count; ++j)
            {
                bSuccess &= tempVolatilityUnitAchievementTryInfo[i].bCheckList[j];
            }

            if (bSuccess)
            {
                int Money = VolatilityUnitAchievementInfoList[i].Money;
                string strName = string.Format("Achieve <color=white>{0}</color> - Get money +{1}", VolatilityUnitAchievementInfoList[i].Name, Money);
                AddMoney(Money);
                UIManager.Instance.ShowNoticeText(strName, true);

                UnlockManager.Instance.SetSpawnMapAchievement(VolatilityUnitAchievementInfoList[i].SteamName);
                VolatilityUnitAchievementInfoList.RemoveAt(i);
            }
        }
    }

    //Adventure
    public void ChoiceCharacter(SpeciesType eForceSpeciesType)
    {
        if (AdventureCharacterCount > AdventureCharacterLimitCount)
            return;

        if (eForceSpeciesType == SpeciesType.NONE)
            return;

        m_eSpeciesType = eForceSpeciesType;

        // 캐릭터 생성후 바꿔주자
        Vector3 SpawnPosition = new Vector3(0f, -3f, 0f);
        if (AdventureModePlayerList.Count > 0)
        {
            SpawnPosition = AdventureModePlayerList[0].GetPosition();

            MapBase pMapBase = MapManager.Instance.GetCurrentMapInfo();
            if (pMapBase != null)
            {
                Map_Adventure pMapAdventure = pMapBase as Map_Adventure;
                float CurrentPos = pMapAdventure.GetFloorBothEnds(AdventureModePlayerList[0].GetCurrentLayerFloor(), true).y;
                SpawnPosition = AdventureModePlayerList[0].GetPosition();
                SpawnPosition = new Vector3(SpawnPosition.x, CurrentPos, SpawnPosition.z);
            }

            //switch (AdventureModePlayerList[0].GetCurrentLayerFloor())
            //{
            //    case ADVLayerType.ADVLayerType_1:
            //        SpawnPosition.y = -9f;
            //        break;
            //    case ADVLayerType.ADVLayerType_2:
            //        SpawnPosition.y = -4f;
            //        break;
            //    case ADVLayerType.ADVLayerType_3:
            //        SpawnPosition.y = 1f;
            //        break;
            //    case ADVLayerType.ADVLayerType_4:
            //        SpawnPosition.y = 6f;
            //        break;
            //}
        }

        if (AdventureCharacterCount == AdventureModePlayerList.Count)
        {
            ADVLayerType eMainADVLayerType = ADVLayerType.ADVLayerType_None;
            for (int i = 0; i < AdventureModePlayerList.Count; ++i)
            {
                SpeciesType CurSpeciesType = AdventureModePlayerList[i].m_eSpeciesType;
                if (AdventureModePlayerList[i] != null)
                {
                    if (i == 0)
                        eMainADVLayerType = AdventureModePlayerList[i].GetCurrentLayerFloor();

                    //SpawnPosition = AdventureModePlayerList[i].GetPosition();
                    Destroy(AdventureModePlayerList[i].gameObject);
                    AdventureModePlayerList[i] = null;
                }

                GameObject prefab = ResourceAgent.Instance.GetPrefab(CurSpeciesType, selectedBuildingIndex);
                if (prefab == null)
                    continue;

                GameObject ADVCharacter = Instantiate(prefab, SpawnPosition, Quaternion.identity);
                if (ADVCharacter != null)
                {
                    Character tempCharacterInfo = ADVCharacter.GetComponent<Character>();
                    if (tempCharacterInfo != null)
                    {
                        if (i == 0)
                        {
                            AdventureModePlayerList[i] = ADVCharacter.AddComponent<Player_Adventure>();
                            AdventureModePlayerList[i].SetComponent(tempCharacterInfo);
                            AdventureModePlayerList[i].SetInfo(CurSpeciesType, selectedBuildingIndex, MyPlayerAdventureBasicInfo);
                            AdventureController = ADVCharacter.AddComponent<AdventureMoveController>();
                            AdventureController.SetInfo(MyPlayerAdventureBasicInfo);
                            AdventureModePlayerList[i].transform.localScale *= 1.1f;
                            AdventureModePlayerList[i].ChangeRanderOrder(5);   //Change Order layer
                        }
                        else
                        {
                            AdventureModePlayerList[i] = ADVCharacter.AddComponent<Player_Adventure_Friends>();
                            AdventureModePlayerList[i].SetComponent(tempCharacterInfo);
                            AdventureModePlayerList[i].SetInfo(CurSpeciesType, selectedBuildingIndex, MyPlayerAdventureBasicInfo);
                            FollowPlayerController FollowController = ADVCharacter.AddComponent<FollowPlayerController>();
                            FollowController.SetInfo(AdventureModePlayerList[i - 1], MyPlayerAdventureBasicInfo, eMainADVLayerType);
                            AdventureModePlayerList[i].transform.localScale *= 0.8f;
                            AdventureModePlayerList[i].ChangeRanderOrder(4);   //Change Order layer
                        }
                        AdventureModePlayerList[i].SetAdventureController();
                        Destroy(tempCharacterInfo);
                    }
                }
            }

            ImpedimentsPool.Instance.ChangeTarget();
            BossAdventure_Last_Manager.Instance.ChangeTarget();
            UIManager.Instance.HideUI(UIIndexType.MAININFO);
        }
        else
        {
            GameObject prefab = ResourceAgent.Instance.GetPrefab(eForceSpeciesType, selectedBuildingIndex);
            if (prefab == null)
                return;

            GameObject ADVCharacter = Instantiate(prefab, SpawnPosition, Quaternion.identity);
            if (ADVCharacter != null)
            {
                Character tempCharacterInfo = ADVCharacter.GetComponent<Character>();
                if (tempCharacterInfo != null)
                {
                    int OrderIndex;
                    Player_Adventure playerAdventure = null;
                    if (AdventureCharacterCount == 1)
                    {
                        playerAdventure = ADVCharacter.AddComponent<Player_Adventure>();
                        playerAdventure.transform.localScale *= 1.1f;
                        playerAdventure.SetFirstLayerFloor(ADVLayerType.ADVLayerType_2);
                        OrderIndex = 5;
                    }
                    else
                    {
                        playerAdventure = ADVCharacter.AddComponent<Player_Adventure_Friends>();
                        playerAdventure.transform.localScale *= 0.8f;
                        OrderIndex = 4;
                    }

                    playerAdventure.SetComponent(tempCharacterInfo);
                    playerAdventure.SetInfo(eForceSpeciesType, selectedBuildingIndex, MyPlayerAdventureBasicInfo);
                    playerAdventure.ChangeRanderOrder(OrderIndex);   //Change Order layer
                    Destroy(tempCharacterInfo);

                    AdventureModePlayerList.Add(playerAdventure);

                    if (AdventureCharacterCount == 1)
                    {
                        AdventureController = ADVCharacter.AddComponent<AdventureMoveController>();
                        AdventureController.SetInfo(MyPlayerAdventureBasicInfo);
                        UIManager.Instance.HideUI(UIIndexType.MAININFO);
                    }
                    else
                    {
                        FollowPlayerController FollowController = ADVCharacter.AddComponent<FollowPlayerController>();
                        FollowController.SetInfo(AdventureModePlayerList[AdventureModePlayerList.Count - 2], MyPlayerAdventureBasicInfo, AdventureModePlayerList[0].GetCurrentLayerFloor());
                    }
                    playerAdventure.SetAdventureController();
                }
            }
        }

        if (AdventureController)
        {
#if UNITY_EDITOR
            AdventureController.SetTrailRenderer(GetComponent<TrailRenderer>());
#endif
        }

        AdventureMonsterByChangeCharacter();
        PlayerAndFriendsIgnoreCollision();
    }

    public void PlayerIgnoreCollision(Collider targetCollider)
    {
        if (AdventureController == null || targetCollider == null) 
            return;

        AdventureController.PlayerIgnoreCollision(targetCollider);
        //for(int i = 1; i < AdventureModePlayerList.Count; ++i)
        //{
        //    Player_Adventure_Friends playerAdventureFriends = AdventureModePlayerList[i] as Player_Adventure_Friends;
        //    if (playerAdventureFriends == null)
        //        continue;

        //    playerAdventureFriends.GetAdventureController().PlayerIgnoreCollision(targetCollider);
        //}
    }

    public void PlayerAndFriendsIgnoreCollision()
    {
        if (AdventureController == null)
            return;

        //for (int i = 0; i < AdventureModePlayerList.Count; ++i)
        //{
        //    Player_Adventure playerAdventureFriends = AdventureModePlayerList[i];
        //    if (playerAdventureFriends == null)
        //        continue;

        //    //AdventureController.PlayerIgnoreCollision(playerAdventureFriends.GetCollider());
        //    for (int j = i + 1; j < AdventureModePlayerList.Count; ++j)
        //    {
        //        Player_Adventure_Friends playerAdventureFriend = AdventureModePlayerList[j] as Player_Adventure_Friends;
        //        if (playerAdventureFriend == null)
        //            continue;

        //        playerAdventureFriends.GetAdventureController().PlayerIgnoreCollision(playerAdventureFriend.GetCollider());
        //    }
        //}
    }

    private void AdventureMonsterByChangeCharacter()
    {
        List<GameObject> MonsterList = MonsterPool.Instance.GetMonsters();
        if (MonsterList == null)
            return;

        for (int i = 0; i < MonsterList.Count; ++i)
        {
            GameObject monsterObject = MonsterList[i];
            if (monsterObject == null)
                continue;

            //if (monsterObject.gameObject.activeSelf == false)
            //    continue;

            MonsterBase findMonster = monsterObject.GetComponent<MonsterBase>();
            if (findMonster == null)
                continue;

            PlayerIgnoreCollision(findMonster.GetCollider());
        }
    }

    public void SetSelectedPick(AdventureLevelUpItemType eAdventureLevelUpItemType, int iValue, bool bBonus)
    {
        switch (eAdventureLevelUpItemType)
        {
            case AdventureLevelUpItemType.CHARACTER:
                {
                    SetRandomSpeciesType_Adv((SpeciesType)iValue, bBonus);
                }
                break;
            case AdventureLevelUpItemType.STAT:
                {
                    AdventureLevelUpStatType eAdventureLevelUpStatType = (AdventureLevelUpStatType)iValue;
                    SetStatType_Adv(eAdventureLevelUpStatType);
                }
                break;
            case AdventureLevelUpItemType.UTIL:
                {
                    if (iValue == 1)
                    {
                        IncreaseHP(50);
                    }
                    else
                    {
                        if (selectedBuildingIndex < 4)
                        {
                            selectedBuildingIndex += 1;
                            ChoiceCharacter(m_eSpeciesType);
                        }
                    }
                }
                break;
        }

        GameManager.Instance.GameStop(false);
    }

    private void SetRandomSpeciesType_Adv(SpeciesType eSpeciesType, bool bBonus)
    {
        if (!bBonus)
            ++AdventureCharacterPickCount;
        
        ++AdventureCharacterCount;
        UseSpeciesTypeList.Add(eSpeciesType);
        ChoiceCharacter(eSpeciesType);
    }

    private void SetStatType_Adv(AdventureLevelUpStatType eAdventureLevelUpStatType)
    {
        if (m_PlayerAdventureLevelUp[eAdventureLevelUpStatType] < 5)
            m_PlayerAdventureLevelUp[eAdventureLevelUpStatType] += 1;

        SetStat_PlayerAdventure();
    }
    private void SetStat_PlayerAdventure()
    {
        if (AdventureModePlayerList == null || AdventureController == null)
            return;

        for (int i = 0; i < (int)(AdventureLevelUpStatType.MAX); ++i)
        {
            AdventureLevelUpStatType eAdventureLevelUpStatType = (AdventureLevelUpStatType)i;
            if (m_PlayerAdventureLevelUp[eAdventureLevelUpStatType] > 0)
            {
                int iValueIndex = m_PlayerAdventureLevelUp[eAdventureLevelUpStatType] - 1;
                LevelUpSystemInfo outLevelUpSystemInfo = ResourceAgent.Instance.GetLevelUpSystemData(AdventureLevelUpItemType.STAT, i);
                switch (eAdventureLevelUpStatType)
                {
                    case AdventureLevelUpStatType.DAMAGE:
                        MyPlayerAdventureBasicInfo.AddDamagePer = (int)(outLevelUpSystemInfo.valueList[iValueIndex]);
                        break;
                    case AdventureLevelUpStatType.ATTACKSPEED:
                        MyPlayerAdventureBasicInfo.AddatkSpeedPer = outLevelUpSystemInfo.valueList[iValueIndex];
                        break;
                    case AdventureLevelUpStatType.RANGE:
                        MyPlayerAdventureBasicInfo.AddRangePer = outLevelUpSystemInfo.valueList[iValueIndex];
                        break;
                    case AdventureLevelUpStatType.MOVE:
                        MyPlayerAdventureBasicInfo.moveSpeed = outLevelUpSystemInfo.valueList[iValueIndex];
                        break;
                    case AdventureLevelUpStatType.JUMP:
                        MyPlayerAdventureBasicInfo.jumpSpeed = outLevelUpSystemInfo.valueList[iValueIndex];
                        break;
                }
            }
        }

        foreach (var character in AdventureModePlayerList)
        {
            character.SetAddStat(MyPlayerAdventureBasicInfo);
        }
        AdventureController.SetInfo(MyPlayerAdventureBasicInfo);

        if (selectTarget)
        {
            UIManager.Instance.ShowCharacterUI(UIIndexType.MAININFO, selectTarget, true);
        }
    }

    public List<SpeciesType> GetPlayerUseSpeciesTypeList()
    {
        List<SpeciesType> UseableSpeciesTypeList = new List<SpeciesType>();
        for (int i = 0; i < UseSpeciesTypeList.Count; ++i)
        {
            UseableSpeciesTypeList.Add(UseSpeciesTypeList[i]);
        }

        return UseableSpeciesTypeList;
    }

    public int GetAdventureLevelUpStat(AdventureLevelUpStatType eAdventureLevelUpStatType)
    {
        if (m_PlayerAdventureLevelUp == null)
            return 0;

        if (m_PlayerAdventureLevelUp.ContainsKey(eAdventureLevelUpStatType) == false)
            return 0;

        return m_PlayerAdventureLevelUp[eAdventureLevelUpStatType];
    }

    public void AddBuffActor(BuffType eBuffType, bool bOnlyMain = false)
    {
        for (int i = 0; i < AdventureModePlayerList.Count; ++i)
        {
            Player_Adventure playerAdventureFriends = AdventureModePlayerList[i];
            if (playerAdventureFriends == null)
                continue;

            playerAdventureFriends.AddBuffActor(eBuffType);

            if (bOnlyMain && i == 0)
            {
                break;
            }
        }
    }

    public void AddBuffActor(BuffType eBuffType, Vector3 vTargetPosition, float Range = 0f)
    {
        for (int i = 0; i < AdventureModePlayerList.Count; ++i)
        {
            Player_Adventure playerAdventureFriends = AdventureModePlayerList[i];
            if (playerAdventureFriends == null)
                continue;

            float fDistance = Vector3.Distance(playerAdventureFriends.transform.position, vTargetPosition);
            if (Range == 0 || Range >= fDistance)
            {
                playerAdventureFriends.AddBuffActor(eBuffType);
            }
        }
    }

    public Player_Adventure GetAdventureMainPlayer(int index = 0)
    {
        if (AdventureModePlayerList == null || AdventureModePlayerList.Count == 0)
            return null;

        if (index < 0 || index >= AdventureModePlayerList.Count)
            return null;

        return AdventureModePlayerList[index];
    }
}
