using GameDefines;
using UIDefines;
using CharacterDefines;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections;

public class MonsterPool : Singleton<MonsterPool>
{
    [SerializeField] public byte maxCount = 30;
    [SerializeField] public float spawnTimer = 1f;
    [SerializeField] private Transform monsterCanvas = null;
    [SerializeField] private Prey prey = null;

    private byte bossMaxCount = 10;
    private Dictionary<SpeciesType, int> monsterIndexDictionary = new Dictionary<SpeciesType, int>();

    private bool bSpawn = false;
    private float timer = 0f;
    private int currentStage = 0;
    private List<GameObject> poolsList;
    private List<GameObject> ReserveRemoveList;
    private List<GameObject> HPBarList;
    private int poolIndex = 0;
    private byte RemoveMonsterByWaveCount = 0;

    public AttributeType m_eCurrentWaveAttributeType = AttributeType.NONE;
    public event Action OnUpdateBossPatternEvent;

    //boss
    private List<int> AlreadyUsedBossIndex = new List<int>();

    private int iSpecialPercent = 1;

    // Loop
    private bool bInfiniteMode = false;
    private List<int> RemoveIndexList_InfiniteMode;
    private StageInfo LastStageInfo;
    //private byte RemoveMonsterMaxCount_InfiniteMode = 0;

    // Adventure
    private Dictionary<int, List<int>> monsterPostList_Adv;

#if UNITY_EDITOR
    private bool bStopSpawn = false;    //Test
#endif

    protected override void Awake()
    {
        poolsList = new List<GameObject>();
        poolsList.Clear();
        for (int i = 0; i < maxCount; ++i)
        {
            poolsList.Add(null);
        }
        // Boss
        if (Oracle.m_eGameType == MapType.SPAWN)
        {
            for (int i = 0; i < bossMaxCount; ++i)
            {
                poolsList.Add(null);
            }
        }

        ReserveRemoveList = new List<GameObject>();

        for (int i = 0; i < (int)SpeciesType.MAX; ++i)
        {
            monsterIndexDictionary.Add((SpeciesType)i, -1);
        }

        HPBarList = new List<GameObject>();
        HPBarList.Clear();

        monsterPostList_Adv = new Dictionary<int, List<int>>();
    }
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.P))
        {
            bStopSpawn = !bStopSpawn;
        }

        if (bStopSpawn)
            return;
#endif

        if (!bSpawn)
            return;

        timer += Time.deltaTime;
        if (Oracle.m_eGameType == MapType.ADVENTURE)
        {
            if (monsterPostList_Adv.Count > 0)
            {
                bSpawn = false;

                List<IEnumerator> coroutines = new List<IEnumerator>();
                foreach (var data in monsterPostList_Adv)
                {
                    //SpawnMonsterDelay(data.Value);
                    coroutines.Add(SpawnMonsterDelayCoroutine(data.Value));
                }

                monsterPostList_Adv.Clear();

                List<Coroutine> activeCoroutines = new List<Coroutine>();
                foreach (var coroutine in coroutines)
                {
                    activeCoroutines.Add(StartCoroutine(coroutine));
                }

                //// 모든 코루틴이 완료될 때까지 기다리기
                //foreach (var activeCoroutine in activeCoroutines)
                //{
                //    yield return activeCoroutine;
                //}
            }
            else
            {
                if (timer > spawnTimer)
                {
                    timer = 0f;
                    SpawnMonster();
                }
            }
        }
        else
        {
            if (timer > spawnTimer)
            {
                timer = 0f;
                SpawnMonster();
            }
        }
    }

    public List<GameObject> GetMonsters()
    {
        return poolsList;
    }

    GameObject GetMonster(int index)
    {
        if (poolsList.Count <= index || index < 0)
            return null;

        GameObject selectUnit = poolsList[index];
        return selectUnit;
    }

    void SpawnCustomMonster(int index)
    {
        poolsList[index].SetActive(true);
    }

    void SpawnMonster()
    {
        int index = poolIndex++;
        GameObject monster = GetMonster(index);
        if (monster == null)
        {
            bSpawn = false;
            return;
        }

        if (index < HPBarList.Count)
        {
            HPBarList[index].SetActive(true);
        }

        monster.SetActive(true);
    }

    async void SpawnMonsterDelay(List<int> indexList)
    {
        for (int i = 0; i < indexList.Count; ++i)
        {
            while (Time.timeScale == 0)
            {
                await Task.Yield();
            }

            int index = indexList[i];
            poolIndex++;

            GameObject monster = GetMonster(index);
            if (monster == null)
            {
                bSpawn = false;
                return;
            }

            if (index < HPBarList.Count)
            {
                HPBarList[index].SetActive(true);
            }

            monster.SetActive(true);
            float actualDelay = spawnTimer * 1000f;
            await Task.Delay((int)actualDelay);
        }
    }

    IEnumerator SpawnMonsterDelayCoroutine(List<int> indexList)
    {
        for (int i = 0; i < indexList.Count; ++i)
        {
            int index = indexList[i];
            poolIndex++;

            GameObject monster = GetMonster(index);
            if (monster == null)
            {
                bSpawn = false;
                yield break;
            }

            if (index < HPBarList.Count)
            {
                HPBarList[index].SetActive(true);
            }

            monster.SetActive(true);
            yield return new WaitForSeconds(spawnTimer);
        }
    }

    public void StartSpawn()
    {
        bSpawn = true;
    }

    public void RemoveMonsterByWave(int index)
    {
        if (index < 0)
            return;

        if (index < HPBarList.Count)
        {
            HPBarList[index].SetActive(false);
        }

        if (bInfiniteMode)
        {
            RemoveIndexList_InfiniteMode.Add(index);
        }

        --RemoveMonsterByWaveCount;
        if (RemoveMonsterByWaveCount <= 0)
        {
            DeSpawnMonster();
        }
    }
    public void RemoveBoss(int removeIndex = -1)
    {
        if (Oracle.m_eGameType == MapType.SPAWN)
        {
            if (removeIndex >= 0 && removeIndex < poolsList.Count)
            {
                if (poolsList[removeIndex] != null)
                {
                    DestroyImmediate(poolsList[removeIndex]);
                    poolsList[removeIndex] = null;
                }
            }
        }
        else
        {
            DeSpawnMonster();
        }

        for (int i = 0; i < ReserveRemoveList.Count; ++i)
        {
            DestroyImmediate(ReserveRemoveList[i]);
            ReserveRemoveList[i] = null;
        }
        ReserveRemoveList.Clear();
    }

    void DeSpawnMonster()
    {
        if (bInfiniteMode)
        {
            RemoveIndexList_InfiniteMode.Sort();
            for (int i = 0; i < RemoveIndexList_InfiniteMode.Count; ++i)
            {
                int index = RemoveIndexList_InfiniteMode[i];
                if (poolsList[index] != null)
                {
                    DestroyImmediate(poolsList[index]);
                    poolsList[index] = null;
                }

                if (index < HPBarList.Count)
                {
                    DestroyImmediate(HPBarList[index]);
                    HPBarList.RemoveAt(index);
                }
            }

            RemoveIndexList_InfiniteMode.Clear();
        }
        else
        {
            for (int i = 0; i < maxCount; ++i)
            {
                if (poolsList[i] != null)
                {
                    DestroyImmediate(poolsList[i]);
                    poolsList[i] = null;
                }

                if (i < HPBarList.Count)
                {
                    DestroyImmediate(HPBarList[i]);
                }
            }

            HPBarList.Clear();
        }

        bSpawn = false;
        timer = 0f;
        poolIndex = 0;
        RemoveMonsterByWaveCount = 0;
        GameManager.Instance.NextWave();
    }

    public void ForceDeSpawnMonster()
    {
        DeSpawnMonster();
    }

    public void SetWaveInfo(byte stageIndex)
    {
        currentStage = stageIndex;

        --stageIndex;

        AttributeType eAttributeType = (AttributeType)Oracle.RandomDice(1, (int)AttributeType.MAX);

        if (Oracle.m_eGameType == MapType.ADVENTURE)
        {
            StageInfo_Adv stageInfo = ResourceAgent.Instance.GetStageInfo_Adv(stageIndex);
            if (stageInfo.Hp == 0)
                return;

            if (prey)
            {
                prey.ChangePosition(currentStage == 100);
            }
            
            if (currentStage % 10 != 0)
            {
                SetMonsterInfo_Adv(stageInfo, eAttributeType);
            }
            else
            {
                int bossIndex = currentStage / 10;
                if (bossIndex == 10)
                {
                    m_eCurrentWaveAttributeType = AttributeType.NONE;
                    BossAdventure_Last_Manager.Instance.SetLastBossInfo_Adv(stageInfo);
                }
                else
                {
                    SetBossInfo_Adv(stageInfo, eAttributeType, bossIndex);
                }
            }
        }
        else
        {
            StageInfo stageInfo = ResourceAgent.Instance.GetStageInfo(stageIndex);
            if (stageInfo.Hp == 0)
            {
                if (stageIndex >= 100)
                {
                    if (stageIndex == 100)
                    {
                        SetLoopStageInfo();
                    }

                    SetLoopMonsterInfo(eAttributeType);
                }

                return;
            }

            GameManager.Instance.GetPlayer().AddMoney(stageInfo.stageMoney);
            if (currentStage % 10 != 0 || Oracle.m_eGameType == MapType.SPAWN)
            {
                SetMonsterInfo(stageInfo, eAttributeType);
            }
            else if (Oracle.m_eGameType == MapType.BUILD)
            {
                SetBossInfo(stageInfo, eAttributeType, currentStage / 10);
            }
        }
    }

    private void SetMonsterInfo(StageInfo stageInfo, AttributeType eAttributeType)
    {
        SpeciesType eSpeciesType = (SpeciesType)Oracle.RandomDice(1, (int)SpeciesType.MAX);
        m_eCurrentWaveAttributeType = eAttributeType;

        List<GameObject> refMonsterPrefabs = ResourceAgent.Instance.GetPrefab(eSpeciesType);
        if (refMonsterPrefabs == null)
        {
            Debug.Log("MonsterPrefabs is null = " + eSpeciesType);
        }

        int prefabsIndex = monsterIndexDictionary[eSpeciesType] + 1;
        if (prefabsIndex >= refMonsterPrefabs.Count)
            prefabsIndex = 0;

        bool bSpecialUnit = false;

        monsterIndexDictionary[eSpeciesType] = prefabsIndex;
        for (int i = 0; i < maxCount; ++i)
        {
            if (poolsList[i] != null)
                continue;

            bool bSpawnSpecialUnit = false;
            if (bSpecialUnit == false)
            {
                if (Oracle.PercentSuccess(iSpecialPercent))
                {
                    bSpawnSpecialUnit = true;
                    bSpecialUnit = true;
                    poolsList[i] = Instantiate(ResourceAgent.Instance.GetPrefab(SpeciesType.GOLDGOBLIN)[0]);
                }
            }

            if (!bSpawnSpecialUnit)
                poolsList[i] = Instantiate(refMonsterPrefabs[prefabsIndex]);

            Character tempCharacterInfo = poolsList[i].GetComponent<Character>();
            if (tempCharacterInfo != null)
            {
                MonsterBase tempMonsterInfo = poolsList[i].AddComponent<MonsterBase>();
                tempMonsterInfo.SetComponent(tempCharacterInfo);
                tempMonsterInfo.m_eSpeciesType = bSpawnSpecialUnit ? SpeciesType.GOLDGOBLIN : eSpeciesType;
                tempMonsterInfo.m_eAttributeType = m_eCurrentWaveAttributeType;
                tempMonsterInfo.SetInfo(i, stageInfo.Hp, stageInfo.Defense, stageInfo.moveSpeed, stageInfo.money);
                tempMonsterInfo.gameObject.SetActive(false);
                Destroy(tempCharacterInfo); // MonsterBase 컴포넌트 제거

                //UI Create
                GameObject HPBarPrefeb = Instantiate(UIManager.Instance.GetUIPrefeb(UIPrefebType.HPBAR));
                if (HPBarPrefeb)
                {
                    HPBarPrefeb.transform.SetParent(monsterCanvas);
                    HPBarPrefeb.transform.localScale = Vector3.one * 0.7f;
                    UI_HPBar UIHPBar = HPBarPrefeb.GetComponent<UI_HPBar>();
                    if (UIHPBar)
                    {
                        UIHPBar.SetUp(tempMonsterInfo);
                    }
                    HPBarPrefeb.gameObject.SetActive(false);
                    HPBarList.Add(HPBarPrefeb);
                }

                // Spawn Speed
                spawnTimer = 1f / stageInfo.moveSpeed;
            }

            ++RemoveMonsterByWaveCount;
        }
    }

    private void SetBossInfo(StageInfo stageInfo, AttributeType eAttributeType, int bossIndex)
    {
        int index = bossIndex >= 10 ? Oracle.m_iBossPrefabCount : Oracle.RandomDice(0, Oracle.m_iBossPrefabCount);
        index = CheckAlreadyUsedIndex(index);

        m_eCurrentWaveAttributeType = eAttributeType;
        GameObject refBossPrefabs = ResourceAgent.Instance.GetBossPrefab(index);
        if (refBossPrefabs == null)
        {
            Debug.Log("BossPrefabs is null = " + index);
            return;
        }

        poolsList[0] = Instantiate(refBossPrefabs);
        BossBase Boss = poolsList[0].GetComponent<BossBase>();
        if (Boss != null)
        {
            BossBase tempBoss = null;
            switch (bossIndex)
            {
                case 1:
                    tempBoss = poolsList[0].AddComponent<Boss1>();
                    break;
                case 2:
                    tempBoss = poolsList[0].AddComponent<Boss2>();
                    break;
                case 3:
                    tempBoss = poolsList[0].AddComponent<Boss3>();
                    break;
                case 4:
                    tempBoss = poolsList[0].AddComponent<Boss4>();
                    break;
                case 5:
                    tempBoss = poolsList[0].AddComponent<Boss5>();
                    break;
                case 6:
                    tempBoss = poolsList[0].AddComponent<Boss6>();
                    break;
                case 7:
                    tempBoss = poolsList[0].AddComponent<Boss7>();
                    break;
                case 8:
                    tempBoss = poolsList[0].AddComponent<Boss8>();
                    break;
                case 9:
                    tempBoss = poolsList[0].AddComponent<Boss9>();
                    break;
                case 10:
                    tempBoss = poolsList[0].AddComponent<Boss10>();
                    break;
            };

            if (tempBoss != null)
            {
                tempBoss.SetComponent(Boss);
                tempBoss.m_eSpeciesType = SpeciesType.MAX;
                tempBoss.m_eAttributeType = eAttributeType;
                tempBoss.SetInfo(0, stageInfo.Hp, stageInfo.Defense, stageInfo.moveSpeed, stageInfo.money);
                tempBoss.prefabIndex = index;
                tempBoss.gameObject.SetActive(false);
            }
            Destroy(Boss);
        }
    }

    public BossBase AddBossInfo(int prefabIndex, int bossIndex, bool bSetComponent = false)
    {
        GameObject refBossPrefabs = ResourceAgent.Instance.GetBossPrefab(prefabIndex);
        if (refBossPrefabs == null)
        {
            Debug.Log("AddBossInfo BossPrefabs is null = " + prefabIndex);
        }

        int index = -1;
        if (Oracle.m_eGameType == MapType.SPAWN)
        {
            for (int i = maxCount; i < maxCount + bossMaxCount; ++i)
            {
                if (poolsList[i] == null)
                {
                    index = i;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < maxCount; ++i)
            {
                if (poolsList[i] == null)
                {
                    index = i;
                    break;
                }
            }
        }

        if (index < 0)
            return null;

        poolsList[index] = Instantiate(refBossPrefabs);
        BossBase addBoss = poolsList[index].GetComponent<BossBase>();

        BossBase outBoss = null;
        switch (bossIndex)
        {
            case 1:
                outBoss = poolsList[index].AddComponent<Boss1>();
                break;
            case 2:
                outBoss = poolsList[index].AddComponent<Boss2>();
                break;
            case 3:
                outBoss = poolsList[index].AddComponent<Boss3>();
                break;
            case 4:
                outBoss = poolsList[index].AddComponent<Boss4>();
                break;
            case 5:
                outBoss = poolsList[index].AddComponent<Boss5>();
                break;
            case 6:
                outBoss = poolsList[index].AddComponent<Boss6>();
                break;
            case 7:
                outBoss = poolsList[index].AddComponent<Boss7>();
                break;
            case 8:
                outBoss = poolsList[index].AddComponent<Boss8>();
                break;
            case 9:
                outBoss = poolsList[index].AddComponent<Boss9>();
                break;
            case 10:
                outBoss = poolsList[index].AddComponent<Boss10>();
                break;
        };

        if (bSetComponent && outBoss != null)
        {
            outBoss.SetComponent(addBoss);
            outBoss.poolIndex = index;
        }
        Destroy(addBoss);

        return outBoss;
    }

    public void SetMonsterInfo_Adv(StageInfo_Adv stageInfo, AttributeType eAttributeType)
    {
        MapBase pMapBase = MapManager.Instance.GetCurrentMapInfo();
        if (pMapBase is Map_Adventure == false)
            return;

        Map_Adventure pMap_Adventure = pMapBase as Map_Adventure;
        if (pMap_Adventure == null)
            return;

        Player MyPlayer = GameManager.Instance.GetPlayer();
        if (MyPlayer == null)
            return;

        // Select Position
        AdventureLevelType eAdventureLevelType = GameManager.Instance.GetAdventureLevel();
        SpeciesType eSpeciesType = (SpeciesType)Oracle.RandomDice(1, (int)SpeciesType.MAX);
        m_eCurrentWaveAttributeType = eAttributeType;

        List<GameObject> refMonsterPrefabs = ResourceAgent.Instance.GetPrefab(eSpeciesType);
        if (refMonsterPrefabs == null)
        {
            Debug.Log("MonsterPrefabs is null = " + eSpeciesType);
        }

        int prefabsIndex = monsterIndexDictionary[eSpeciesType] + 1;
        if (prefabsIndex >= refMonsterPrefabs.Count)
            prefabsIndex = 0;

        monsterPostList_Adv.Clear();
        monsterIndexDictionary[eSpeciesType] = prefabsIndex;

        bool bSpecialUnit = false;
        for (int i = 0; i < stageInfo.Count; ++i)
        {
            if (poolsList[i] != null)
                continue;

            bool bSpawnSpecialUnit = false;
            if (bSpecialUnit == false)
            {
                if (Oracle.PercentSuccess(iSpecialPercent))
                {
                    bSpawnSpecialUnit = true;
                    bSpecialUnit = true;
                    poolsList[i] = Instantiate(ResourceAgent.Instance.GetPrefab(SpeciesType.GOLDGOBLIN)[0]);
                }
            }

            if (!bSpawnSpecialUnit)
                poolsList[i] = Instantiate(refMonsterPrefabs[prefabsIndex]);

            Character tempCharacterInfo = poolsList[i].GetComponent<Character>();
            if (tempCharacterInfo != null)
            {
                /* Position
                 * 0~4  4층
                 * 5~6  3층
                 * 7~8  2층
                 * 9~10 1층
                */
                int iPos = 0;
                if (eAdventureLevelType == AdventureLevelType.LEVEL1)
                {
                    if (i < stageInfo.Count / 2)
                        iPos = 1;
                }
                else if (eAdventureLevelType == AdventureLevelType.LEVEL2)
                {
                    int shared = stageInfo.Count / 5;
                    if (i < shared)
                        iPos = 1;
                    else if (i < shared * 2)
                        iPos = 2;
                    else if (i < shared * 3)
                        iPos = 3;
                    else if (i < shared * 4)
                        iPos = 4;
                }
                else if (eAdventureLevelType == AdventureLevelType.LEVEL3)
                {
                    if (i < (stageInfo.Count * 30 / 100))
                        iPos = Oracle.RandomDice(5, 7);
                    else
                        iPos = Oracle.RandomDice(0, 5);
                }
                else
                {
                    if (i >= stageInfo.Count - 2)
                        iPos = Oracle.RandomDice(9, 11);
                    else if (i < (stageInfo.Count * 10 / 100))
                        iPos = Oracle.RandomDice(7, 9);
                    else if (i < (stageInfo.Count * 40 / 100))
                        iPos = Oracle.RandomDice(5, 7);
                    else
                        iPos = Oracle.RandomDice(0, 5);
                }

                monsterPostList_Adv.TryAdd(iPos, new List<int>());
                monsterPostList_Adv[iPos].Add(i);

                Vector3 pos = pMap_Adventure.GetSpawnPointPosition(eAdventureLevelType, iPos);
                ADVLayerType eADVLayerType = pMap_Adventure.GetSpawnPointADVLayerType(iPos);

                MonsterAdventure tempMonsterInfo = poolsList[i].AddComponent<MonsterAdventure>();
                tempMonsterInfo.SetComponent(tempCharacterInfo);
                tempMonsterInfo.m_eSpeciesType = bSpawnSpecialUnit ? SpeciesType.GOLDGOBLIN : eSpeciesType;
                tempMonsterInfo.m_eAttributeType = m_eCurrentWaveAttributeType;
                tempMonsterInfo.SetInfo_Adv(i, stageInfo.Hp, stageInfo.Defense, stageInfo.moveSpeed, stageInfo.Range, stageInfo.Attack, pos);
                tempMonsterInfo.SetFirstLayerFloor(eADVLayerType);
                tempMonsterInfo.gameObject.SetActive(false);
                Destroy(tempCharacterInfo); // MonsterBase 컴포넌트 제거

                MyPlayer.PlayerIgnoreCollision(tempMonsterInfo.GetCollider());

                // Pathfinder
                MonsterPathfinder tempMonsterPathfinder = poolsList[i].AddComponent<MonsterPathfinder>();
                tempMonsterPathfinder.SetTarget(tempMonsterInfo, prey);

                //UI Create
                GameObject HPBarPrefeb = Instantiate(UIManager.Instance.GetUIPrefeb(UIPrefebType.HPBAR));
                if (HPBarPrefeb)
                {
                    HPBarPrefeb.transform.SetParent(monsterCanvas);
                    HPBarPrefeb.transform.localScale = Vector3.one * 0.7f;
                    UI_HPBar UIHPBar = HPBarPrefeb.GetComponent<UI_HPBar>();
                    if (UIHPBar)
                    {
                        UIHPBar.SetUp(tempMonsterInfo);
                    }
                    HPBarPrefeb.gameObject.SetActive(false);
                    HPBarList.Add(HPBarPrefeb);
                }

                // Spawn Speed
                spawnTimer = 1f / stageInfo.moveSpeed;
            }

            ++RemoveMonsterByWaveCount;
        }
    }

    private void SetBossInfo_Adv(StageInfo_Adv stageInfo, AttributeType eAttributeType, int bossIndex)
    {
        MapBase pMapBase = MapManager.Instance.GetCurrentMapInfo();
        if (pMapBase is Map_Adventure == false)
            return;

        Map_Adventure pMap_Adventure = pMapBase as Map_Adventure;
        if (pMap_Adventure == null)
            return;

        Player MyPlayer = GameManager.Instance.GetPlayer();
        if (MyPlayer == null)
            return;

        m_eCurrentWaveAttributeType = eAttributeType;
        GameObject refBossPrefabs = ResourceAgent.Instance.GetAdvBossPrefab(AdventurePrefabsType.BOSS);
        if (refBossPrefabs == null)
        {
            Debug.Log("BossPrefabs is null stage = " + bossIndex);
            return;
        }

        poolsList[0] = Instantiate(refBossPrefabs);
        BossAdventure Boss = poolsList[0].GetComponent<BossAdventure>();
        if (Boss != null)
        {
            Vector3 pos;
            if (bossIndex == 9)
                pos = pMap_Adventure.GetSpawnPointPosition(AdventureLevelType.LEVEL4, Oracle.RandomDice(0, 9));
            else
                pos = pMap_Adventure.GetSpawnPointPosition(AdventureLevelType.LEVEL1, Oracle.RandomDice(0, 5));

            ADVLayerType eADVLayerType = pMap_Adventure.GetSpawnPointADVLayerType(0);

            Boss.SetComponent(Boss);
            Boss.SetInfo_Adv(bossIndex, stageInfo.Hp, stageInfo.Defense, stageInfo.moveSpeed, stageInfo.Range, stageInfo.Attack, pos);
            Boss.m_eSpeciesType = SpeciesType.MAX;
            Boss.m_eAttributeType = eAttributeType;
            Boss.SetFirstLayerFloor(eADVLayerType);

            Vector3 originScale = Boss.transform.localScale;
            originScale += new Vector3(3 * bossIndex / 100f, 3 * bossIndex / 100f, 1);
            Boss.transform.localScale = originScale;

            Boss.gameObject.SetActive(false);
            spawnTimer = 1f;

            //MyPlayer.PlayerIgnoreCollision(Boss.GetCollider());

            // Pathfinder
            MonsterPathfinder tempMonsterPathfinder = poolsList[0].AddComponent<MonsterPathfinder>();
            tempMonsterPathfinder.SetTarget(Boss, prey);
        }
    }

    private int CheckAlreadyUsedIndex(int index)
    {
        for (int i = 0; i < AlreadyUsedBossIndex.Count; ++i)
        {
            if (AlreadyUsedBossIndex[i] == index)
            {
                ++index;
                if (index >= Oracle.m_iBossPrefabCount)
                    index = 0;

                index = CheckAlreadyUsedIndex(index);
                break;
            }
        }

        return index;
    }

    public void UpdateBossPatternEvent()
    {
        OnUpdateBossPatternEvent?.Invoke();
    }

    // LOOP
    public void SetLoopStageInfo()
    {
        bInfiniteMode = true;
        RemoveIndexList_InfiniteMode = new List<int>();
        LastStageInfo = new StageInfo(0, 0, 0f, 0, 0, 0);
        LastStageInfo = ResourceAgent.Instance.GetStageInfo(99);

        LastStageInfo.nextStartTime = 40;

        int LoopMaxCount = maxCount * 3;
        for (int i = maxCount; i < LoopMaxCount; ++i)
        {
            poolsList.Add(null);
        }
    }

    private void SetLoopMonsterInfo(AttributeType eAttributeType)
    {
        // SetStat
        if (Oracle.m_eGameType == MapType.BUILD)
            LastStageInfo.Hp = (int)((float)LastStageInfo.Hp * 1.1);    // HP+10%
        else
            LastStageInfo.Hp = (int)((float)LastStageInfo.Hp * 1.06);   // HP+6%

        // 5단계마다
        bool bBoss = false;
        if (currentStage % 5 == 0)
        {
            LastStageInfo.moveSpeed *= 1.05f;
            LastStageInfo.Defense += 1;
        }
        // 10단계마다
        if (currentStage % 10 == 0)
        {
            LastStageInfo.moveSpeed += 1f;
            LastStageInfo.Defense += 1;
            LastStageInfo.nextStartTime -= (short)((float)LastStageInfo.nextStartTime * 0.1f);

            bBoss = Oracle.RandomDice(0, 2) == 0;
        }

        if (LastStageInfo.Defense > 90)
            LastStageInfo.Defense = 90;
        if (LastStageInfo.nextStartTime < maxCount)
        {
            LastStageInfo.nextStartTime = maxCount;
            ++LastStageInfo.nextStartTime;
        }

        StageInfo stageInfo = new StageInfo(LastStageInfo.Hp, LastStageInfo.Defense, LastStageInfo.moveSpeed, LastStageInfo.money, 0, LastStageInfo.nextStartTime);

        // SetData
        m_eCurrentWaveAttributeType = eAttributeType;

        SpeciesType eSpeciesType = (SpeciesType)Oracle.RandomDice(1, (int)SpeciesType.MAX);
        List<GameObject> refMonsterPrefabs = ResourceAgent.Instance.GetPrefab(eSpeciesType);
        if (refMonsterPrefabs == null)
        {
            Debug.Log("MonsterPrefabs is null = " + eSpeciesType);
        }

        int prefabsIndex = monsterIndexDictionary[eSpeciesType] + 1;
        if (prefabsIndex >= refMonsterPrefabs.Count)
            prefabsIndex = 0;

        bool bSpecialUnit = false;

        monsterIndexDictionary[eSpeciesType] = prefabsIndex;

        List<int> poolsIndexList = new List<int>();
        for (int i = 0; i < poolsList.Count; ++i)
        {
            if (poolsList[i] == null)
            {
                poolsIndexList.Add(i);
                if (poolsIndexList.Count >= maxCount)
                    break;
            }
        }

        if (poolsIndexList.Count != maxCount)
        {
            Debug.Log("LoopStage::poolsIndexList error");
            return;
        }

        for (int i = 0; i < maxCount; ++i)
        {
            int index = poolsIndexList[i];
            if (poolsList[index] != null)
                continue;

            bool bSpawnSpecialUnit = false;
            if (bSpecialUnit == false)
            {
                if (Oracle.PercentSuccess(iSpecialPercent))
                {
                    bSpawnSpecialUnit = true;
                    bSpecialUnit = true;
                    poolsList[index] = Instantiate(ResourceAgent.Instance.GetPrefab(SpeciesType.GOLDGOBLIN)[0]);
                }
            }

            if (!bSpawnSpecialUnit)
                poolsList[index] = Instantiate(refMonsterPrefabs[prefabsIndex]);

            Character tempCharacterInfo = poolsList[index].GetComponent<Character>();
            if (tempCharacterInfo != null)
            {
                MonsterBase tempMonsterInfo = poolsList[index].AddComponent<MonsterBase>();
                tempMonsterInfo.SetComponent(tempCharacterInfo);
                tempMonsterInfo.m_eSpeciesType = bSpawnSpecialUnit ? SpeciesType.GOLDGOBLIN : eSpeciesType;
                tempMonsterInfo.m_eAttributeType = m_eCurrentWaveAttributeType;
                tempMonsterInfo.SetInfo(index, stageInfo.Hp, stageInfo.Defense, stageInfo.moveSpeed, stageInfo.money);
                tempMonsterInfo.gameObject.SetActive(false);
                Destroy(tempCharacterInfo); // MonsterBase 컴포넌트 제거

                //UI Create
                GameObject HPBarPrefeb = Instantiate(UIManager.Instance.GetUIPrefeb(UIPrefebType.HPBAR));
                if (HPBarPrefeb)
                {
                    HPBarPrefeb.transform.SetParent(monsterCanvas);
                    HPBarPrefeb.transform.localScale = Vector3.one * 0.7f;
                    UI_HPBar UIHPBar = HPBarPrefeb.GetComponent<UI_HPBar>();
                    if (UIHPBar)
                    {
                        UIHPBar.SetUp(tempMonsterInfo);
                    }
                    HPBarPrefeb.gameObject.SetActive(false);
                    HPBarList.Add(HPBarPrefeb);
                }

                // Spawn Speed
                spawnTimer = 1f / stageInfo.moveSpeed;
            }

            ++RemoveMonsterByWaveCount;
        }
    }
    
    public void ReserveRemoveBoss(GameObject gameObject)
    {
        ReserveRemoveList.Add(gameObject);
    }

    public void AddMonsterInPool(GameObject addGameObject)
    {
        if (addGameObject == null)
            return;

        for (int i = 0; i < maxCount; ++i)
        {
            if (poolsList[i] == null)
            {
                poolsList[i] = addGameObject;
                break;
            }
        }
    }

    public void ChangeBossHP(int Hp, int HpPercent)
    {
        if (poolsList[0] == null)
            return;

        BossAdventure Boss = poolsList[0].GetComponent<BossAdventure>();
        if (Boss)
        {
            if (HpPercent > 0)
            {
                float percent = HpPercent / 100f;
                Boss.currentHP = (int)(Boss.Hp * percent);
            }
            else
            {
                Boss.currentHP = Hp;
            }
        }
    }
}
