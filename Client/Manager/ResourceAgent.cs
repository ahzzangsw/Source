using GameDefines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static BuffContainer;
using static UnityEngine.Rendering.DebugUI;

public class ResourceAgent : Singleton<ResourceAgent>
{
    [SerializeField] private bool bForceCsvData = false;

    private BuildingDatabase B_Database = new BuildingDatabase();
    private MonsterDatabase M_Database = new MonsterDatabase();
    private BuildingDatabase2 B2_Database = new BuildingDatabase2();
    private MonsterDatabase2 M2_Database = new MonsterDatabase2();
    private BuildingDatabase3 B3_Database = new BuildingDatabase3(); 
    private MonsterDatabase3 M3_Database = new MonsterDatabase3();
    private ControlDatabase Etc_Database = new ControlDatabase();
    private UnitAchievementDatabase UnitAchievement_Database = new UnitAchievementDatabase();
    private LevelUpSystemDatabase LevelUpSystem_Database = new LevelUpSystemDatabase();

    [SerializeField] public GameObject[] CharacterPrefabsArray;
    private Dictionary<SpeciesType, List<GameObject>> CharacterPrefabDictionary;

    [SerializeField] public GameObject[] BossPrefabsArray;
    private List<GameObject> BossPrefabList;

    [SerializeField] public GameObject[] AdventureBossPrefabsArray;
    private List<GameObject> AdventureBossPrefabList;

    [SerializeField] public GameObject[] WeaponPrefabsArray;
    private Dictionary<WeaponType, GameObject> WeaponPrefabDictionary;

    [SerializeField] public GameObject[] BuffPrefabsArray;
    private Dictionary<BuffType, GameObject> BuffPrefabDictionary;

    [SerializeField] public GameObject[] ETCPrefabsArray;

    private List<StageInfo> StageInfoList;
    private List<StageInfo> StageInfoList2;
    private List<StageInfo_Adv> StageInfoList3;
    private Dictionary<SpeciesType, List<BuildingInfo>> BuildingInfoList;
    private Dictionary<SpeciesType, List<BuildingInfo>> BuildingInfoList2;
    private Dictionary<SpeciesType, List<BuildingInfo>> BuildingInfoList3;
    private ControlInfo ControlInfo_Game2;
    private List<KeyAchievementInfo> KeyAchievementInfoList;
    private List<UnitAchievementInfo> UnitAchievementInfoList;
    private Dictionary<AdventureLevelUpItemType, List<LevelUpSystemInfo>> LevelUpSystemInfoList;

    [SerializeField] public Sprite[] CursorSpriteArray;

    protected override void Awake()
    {
        base.Awake();

        BuildingInfoList = new Dictionary<SpeciesType, List<BuildingInfo>>();
        BuildingInfoList2 = new Dictionary<SpeciesType, List<BuildingInfo>>();
        BuildingInfoList3 = new Dictionary<SpeciesType, List<BuildingInfo>>();
        StageInfoList = new List<StageInfo>();
        StageInfoList2 = new List<StageInfo>();
        StageInfoList3 = new List<StageInfo_Adv>();
        KeyAchievementInfoList = new List<KeyAchievementInfo>();
        UnitAchievementInfoList = new List<UnitAchievementInfo>();
        LevelUpSystemInfoList = new Dictionary<AdventureLevelUpItemType, List<LevelUpSystemInfo>>();

        BuildingInfoFileRead();
        MonsterInfoFileRead();
        ControlInfoFileRead();
        UnitAchievementFileRead();
        LevelUpSystemFileRead();

        SavePrefabs();
    }

    void SavePrefabs()
    {
        CharacterPrefabDictionary = new Dictionary<SpeciesType, List<GameObject>>();
        foreach (GameObject prefab in CharacterPrefabsArray)
        {
            if (prefab == null)
                continue;

            Character character = prefab.GetComponent<Character>();
            if (character == null)
                continue;

            if (GameManager.Instance.bTest == false)
            {
                if (character.m_eSpeciesType == SpeciesType.NONE)
                    continue;
            }

            List<GameObject> value = null;
            if (CharacterPrefabDictionary.TryGetValue(character.m_eSpeciesType, out value))
            {
                value.Add(prefab);
            }
            else
            {
                value = new List<GameObject> { prefab };
                CharacterPrefabDictionary.Add(character.m_eSpeciesType, value);
            }
        }

        BossPrefabList = new List<GameObject>();
        foreach (GameObject prefab in BossPrefabsArray)
        {
            if (prefab == null)
                continue;

            BossBase Boss = prefab.GetComponent<BossBase>();
            if (Boss == null)
                continue;

            BossPrefabList.Add(prefab);
        }
        Oracle.SetBossPrefabCount(BossPrefabList.Count - 1);    // Troll¡¶ø‹

        AdventureBossPrefabList = new List<GameObject>();
        foreach (GameObject prefab in AdventureBossPrefabsArray)
        {
            if (prefab == null)
                continue;

            AdventureBossPrefabList.Add(prefab);
        }

        WeaponPrefabDictionary = new Dictionary<WeaponType, GameObject>();
        foreach (GameObject prefab in WeaponPrefabsArray)
        {
            if (prefab == null)
                continue;

            EquipWeapon weapon = prefab.GetComponent<EquipWeapon>();
            if (weapon == null)
                continue;

            WeaponPrefabDictionary.Add(weapon.eWeaponType, prefab);
        }

        BuffPrefabDictionary = new Dictionary<BuffType, GameObject>();
        foreach (GameObject prefab in BuffPrefabsArray)
        {
            if (prefab == null)
                continue;

            Buff buff = prefab.GetComponent<Buff>();
            if (buff == null)
                continue;

            if (buff.m_eBuffType == BuffType.NONE)
                continue;

            BuffPrefabDictionary.Add(buff.m_eBuffType, prefab);
        }
    }

    public GameObject GetPrefab(SpeciesType eKey, int iKey)
    {
        List<GameObject> value;
        if (CharacterPrefabDictionary.TryGetValue(eKey, out value))
        {
            if (iKey < value.Count && iKey >= 0)
            {
                GameObject prefab = value[iKey];
                return prefab;
            }
        }
     
        return null;
    }

    public List<GameObject> GetPrefab(SpeciesType eKey)
    {
        if (CharacterPrefabDictionary.ContainsKey(eKey) == false)
            return null;

        return CharacterPrefabDictionary[eKey];
    }

    public GameObject GetBossPrefab(int index)
    {
        if (index < 0 || index >= BossPrefabList.Count)
            return null;

        return BossPrefabList[index];
    }
    public GameObject GetRandomBossPrefab()
    {
        int index = Oracle.RandomDice(0, BossPrefabList.Count);
        return BossPrefabList[index];
    }
    public GameObject GetAdvBossPrefab(AdventurePrefabsType eAdventurePrefabsType)
    {
        if ((int)eAdventurePrefabsType >= AdventureBossPrefabList.Count)
            return null;

        return AdventureBossPrefabList[(int)eAdventurePrefabsType];
    }
    public GameObject GetWeaponPrefab(WeaponType eKey)
    {
        if (WeaponPrefabDictionary.ContainsKey(eKey) == false)
            return null;

        return WeaponPrefabDictionary[eKey];
    }

    public GameObject GetBuffPrefab(BuffType eKey)
    {
        if (BuffPrefabDictionary.ContainsKey(eKey) == false)
            return null;

        return BuffPrefabDictionary[eKey];
    }

    public GameObject GetEtcPrefab(int iKey)
    {
        if (iKey < 0 || iKey >= ETCPrefabsArray.Length)
            return null;

        return ETCPrefabsArray[iKey];
    }
    
    public GameObject GetRandomEtcPrefab()
    {
        if (ETCPrefabsArray.Length == 0)
            return null;

        int iKey = Oracle.RandomDice(0, ETCPrefabsArray.Length);
        return ETCPrefabsArray[iKey];
    }

    void BuildingInfoFileRead()
    {
        if (bForceCsvData)
        {
            BuildingInfoFileRead_CSV("BuildingData.csv", 0);
            BuildingInfoFileRead_CSV("BuildingData2.csv", 1);
            BuildingInfoFileRead_CSV("BuildingData3.csv", 2);
        }
        else
        {
            B_Database.SetData();
            BuildingInfoList = B_Database.BuildingInfoList;
            B2_Database.SetData();
            BuildingInfoList2 = B2_Database.BuildingInfoList;
            B3_Database.SetData();
            BuildingInfoList3 = B3_Database.BuildingInfoList;
        }
    }

    void MonsterInfoFileRead()
    {
        if (bForceCsvData)
        {
            MonsterInfoFileRead_CSV("MonsterData.csv", 0);
            MonsterInfoFileRead_CSV("MonsterData2.csv", 1);
            MonsterInfoFileRead_CSV("MonsterData3.csv", 2);
        }
        else
        {
            M_Database.SetData();
            StageInfoList = M_Database.StageInfoList;
            M2_Database.SetData();
            StageInfoList2 = M2_Database.StageInfoList;
            M3_Database.SetData();
            StageInfoList3 = M3_Database.StageInfoList;
        }
    }

    void BuildingInfoFileRead_CSV(string fileName, int gameIndex)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        FileInfo fileInfo = new FileInfo(filePath);
        string Fulltext = "";

        if (fileInfo.Exists)
        {
            StreamReader reader = new StreamReader(filePath);
            Fulltext = reader.ReadToEnd();
            reader.Close();
        }
        else
            return;

        int MaxInfoLength = 6; //(Damage/Range/AttackSpeed/Cost/Count/Buff)
        int lineIndex = -1;

        Dictionary<SpeciesType, List<BuildingInfo>> tempBuildingInfoList;
        if (gameIndex == 0)
        {
            BuildingInfoList = new Dictionary<SpeciesType, List<BuildingInfo>>();
            tempBuildingInfoList = BuildingInfoList;
        }
        else if (gameIndex == 1)
        {
            BuildingInfoList2 = new Dictionary<SpeciesType, List<BuildingInfo>>();
            tempBuildingInfoList = BuildingInfoList2;
        }
        else if(gameIndex == 2)
        {
            BuildingInfoList3 = new Dictionary<SpeciesType, List<BuildingInfo>>();
            tempBuildingInfoList = BuildingInfoList3;
        }
        else
            return;

        SpeciesType eSpeciesType = SpeciesType.NONE;
        string[] lines = Fulltext.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        foreach (string line in lines)
        {
            ++lineIndex;
            if (!line.Trim().StartsWith("//"))
            {
                if (line.StartsWith("["))
                {
                    int indexOfEnd = line.IndexOf("]");
                    string key = line.Substring(1, indexOfEnd - 1);

                    eSpeciesType = (SpeciesType)(Enum.Parse(typeof(SpeciesType), key, true));
                    List<BuildingInfo> InfoList = new List<BuildingInfo>();
                    tempBuildingInfoList.Add(eSpeciesType, InfoList);
                    continue;
                }

                string[] valueArray = line.Split(',');
                if (valueArray[0] == "end;")
                {
                    break;
                }

                if (valueArray.Length != MaxInfoLength)
                {
                    Debug.LogError("BuildingData Error LineIndex = " + lineIndex);
                    break;
                }

                int Damage = int.Parse(valueArray[0]);
                float Range = float.Parse(valueArray[1]);
                float AttackSpeed = float.Parse(valueArray[2]);
                int Cost = int.Parse(valueArray[3]);
                byte targetCount = byte.Parse(valueArray[4]);
                BuffType eBuffType = BuffType.NONE;
                if (valueArray[5].Length > 0)
                {
                    eBuffType = (BuffType)(Enum.Parse(typeof(BuffType), valueArray[5], true));
                }

                BuildingInfo buildingInfo = new BuildingInfo(Damage, Range, AttackSpeed, Cost, targetCount, eBuffType);
                tempBuildingInfoList[eSpeciesType].Add(buildingInfo);
            }
        }
    }

    void MonsterInfoFileRead_CSV(string fileName, int gameIndex)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        FileInfo fileInfo = new FileInfo(filePath);
        string Fulltext = "";

        if (fileInfo.Exists)
        {
            StreamReader reader = new StreamReader(filePath);
            Fulltext = reader.ReadToEnd();
            reader.Close();
        }
        else
            return;

        int MaxInfoLength = 6; //(HP/Def/MoveSpeed/Money/StageMoney/NextTime)
        int lineIndex = 0;
        string[] lines = Fulltext.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

        List<StageInfo> tempStageInfoList;
        if (gameIndex == 0)
        {
            StageInfoList = new List<StageInfo>();
            tempStageInfoList = StageInfoList;
        }
        else if (gameIndex == 1)
        {
            StageInfoList2 = new List<StageInfo>();
            tempStageInfoList = StageInfoList2;
        }
        else if(gameIndex == 2)
        {
            StageInfoList3 = new List<StageInfo_Adv>();
            
            foreach (string line in lines)
            {
                if (!line.Trim().StartsWith("//"))
                {
                    string[] valueArray = line.Split(',');
                    if (valueArray[0] == "end;")
                        break;

                    if (valueArray.Length != MaxInfoLength)
                    {
                        Debug.LogError("MonsterData Error LineIndex = " + lineIndex);
                        break;
                    }

                    int Hp = int.Parse(valueArray[0]);
                    int Defense = int.Parse(valueArray[1]);
                    float moveSpeed = float.Parse(valueArray[2]);
                    float range = short.Parse(valueArray[3]);
                    int attack = short.Parse(valueArray[4]);
                    int count = int.Parse(valueArray[5]);
                    StageInfo_Adv stageInfo = new StageInfo_Adv(Hp, Defense, moveSpeed, range, attack, count);
                    StageInfoList3.Add(stageInfo);
                }

                ++lineIndex;
            }
            return;
        }
        else
            return;

        foreach (string line in lines)
        {
            if (!line.Trim().StartsWith("//"))
            {
                string[] valueArray = line.Split(',');
                if (valueArray[0] == "end;")
                    break;

                if (valueArray.Length != MaxInfoLength)
                {
                    Debug.LogError("MonsterData Error LineIndex = " + lineIndex);
                    break;
                }

                int Hp = int.Parse(valueArray[0]);
                int Defense = int.Parse(valueArray[1]);
                float moveSpeed = float.Parse(valueArray[2]);
                short money = short.Parse(valueArray[3]);
                short stageMoney = short.Parse(valueArray[4]);
                short nextStartTime = short.Parse(valueArray[5]);
                StageInfo stageInfo = new StageInfo(Hp, Defense, moveSpeed, money, stageMoney, nextStartTime);
                tempStageInfoList.Add(stageInfo);
            }

            ++lineIndex;
        }
    }

    void ControlInfoFileRead()
    {
        if (bForceCsvData)
        {
            ControlInfoFileRead_CSV("ControlData.csv");
        }
        else
        {
            Etc_Database.SetData();
            ControlInfo_Game2 = Etc_Database.ControlInfoData;
        }
    }
    void ControlInfoFileRead_CSV(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        FileInfo fileInfo = new FileInfo(filePath);
        string Fulltext = "";

        if (fileInfo.Exists)
        {
            StreamReader reader = new StreamReader(filePath);
            Fulltext = reader.ReadToEnd();
            reader.Close();
        }
        else
            return;

        int MaxInfoLength = 25; //(Cost/1Atk/2Atk/3Atk/4Atk/5Atk/WorkmanCost/BossMoneyEarned0~8/BossHP0~8)
        string[] lines = Fulltext.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        foreach (string line in lines)
        {
            if (!line.Trim().StartsWith("//"))
            {
                string[] valueArray = line.Split(',');
                if (valueArray[0] == "end;")
                    break;

                if (valueArray.Length != MaxInfoLength)
                {
                    Debug.LogError("ControlData Error");
                    break;
                }

                int CostUp = int.Parse(valueArray[0]);
                int AttackUp0 = int.Parse(valueArray[1]);
                int AttackUp1 = int.Parse(valueArray[2]);
                int AttackUp2 = int.Parse(valueArray[3]);
                int AttackUp3 = int.Parse(valueArray[4]);
                int AttackUp4 = int.Parse(valueArray[5]);
                int WorkmanCost = int.Parse(valueArray[6]);

                int[] BossMoneyEarnedList = new int[9];
                for (int i = 0; i < 9; ++i)
                {
                    int BossMoneyEarned = int.Parse(valueArray[i+7]);
                    BossMoneyEarnedList[i] = BossMoneyEarned;
                }

                int[] BossHPList = new int[9];
                for (int i = 0; i < 9; ++i)
                {
                    int BossHP = int.Parse(valueArray[i + 16]);
                    BossHPList[i] = BossHP;
                }

                ControlInfo controlInfo = new ControlInfo(CostUp, AttackUp0, AttackUp1, AttackUp2, AttackUp3, AttackUp4, WorkmanCost, BossMoneyEarnedList, BossHPList);
                ControlInfo_Game2 = controlInfo;
            }
        }
    }

    void UnitAchievementFileRead()
    {
        if (bForceCsvData)
        {
            UnitAchievementFileRead_CSV("UnitAchievement.csv");
        }
        else
        {
            UnitAchievement_Database.SetData();
            KeyAchievementInfoList = UnitAchievement_Database.KeyAchievementInfoList;
            UnitAchievementInfoList = UnitAchievement_Database.UnitAchievementInfoList;
        }
    }
    void UnitAchievementFileRead_CSV(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        FileInfo fileInfo = new FileInfo(filePath);
        string Fulltext = "";

        if (fileInfo.Exists)
        {
            StreamReader reader = new StreamReader(filePath);
            Fulltext = reader.ReadToEnd();
            reader.Close();
        }
        else
            return;

        int MaxInfoLength = 4; //(Name/Type/Money/SteamName.............)
        int lineIndex = 0;
        string[] lines = Fulltext.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        foreach (string line in lines)
        {
            if (!line.Trim().StartsWith("//"))
            {
                string[] valueArray = line.Split(',');
                if (valueArray[0] == "end;")
                    break;

                if (valueArray.Length <= MaxInfoLength)
                {
                    Debug.LogError("UnitAchievementInfo Error LineIndex = " + lineIndex);
                    break;
                }

                string name = valueArray[0];
                string type = valueArray[1];
                int money = int.Parse(valueArray[2]);
                string steamname = valueArray[3];

                int enumIndex = 0;  // 0: KeyCode, 1:SpeciesType
                if (type.Equals("KeyCode", StringComparison.OrdinalIgnoreCase))
                    enumIndex = 0;
                else if (type.Equals("SpeciesType", StringComparison.OrdinalIgnoreCase))
                    enumIndex = 1;
                else
                {
                    Debug.LogError("UnitAchievementInfo Type Error LineIndex = " + lineIndex);
                    break;
                }

                List<KeyCode> ekeyCodeList = new List<KeyCode>();
                List<SpeciesType> eSpeciesTypeList = new List<SpeciesType>();
                for (int i = MaxInfoLength; i < valueArray.Length; ++i)
                {
                    string value = valueArray[i];
                    value = value.Trim();

                    if (value.Length == 0)
                        break;

                    if (enumIndex == 0)
                    {
                        KeyCode eKeycode = (KeyCode)(Enum.Parse(typeof(KeyCode), value, true));
                        ekeyCodeList.Add(eKeycode);
                    }
                    else if (enumIndex == 1)
                    {
                        SpeciesType eSpeciesType = (SpeciesType)(Enum.Parse(typeof(SpeciesType), value, true));
                        eSpeciesTypeList.Add(eSpeciesType);
                    }
                }

                if (ekeyCodeList.Count > 0)
                {
                    KeyAchievementInfo tempKeyAchievementInfo = new KeyAchievementInfo(name, steamname, money, ekeyCodeList.ToArray());
                    KeyAchievementInfoList.Add(tempKeyAchievementInfo);
                }
                else if (eSpeciesTypeList.Count > 0)
                {
                    UnitAchievementInfo tempUnitAchievementInfo = new UnitAchievementInfo(name, steamname, money, eSpeciesTypeList.ToArray());
                    UnitAchievementInfoList.Add(tempUnitAchievementInfo);
                }

                ++lineIndex;
            }
        }
    }
    void LevelUpSystemFileRead()
    {
        if (bForceCsvData)
        {
            LevelUpSystemFileRead_CSV("LevelUP.csv");
        }
        else
        {
            LevelUpSystem_Database.SetData();
            LevelUpSystemInfoList = LevelUpSystem_Database.LevelUpSystemInfoList;
        }
    }
    void LevelUpSystemFileRead_CSV(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        FileInfo fileInfo = new FileInfo(filePath);
        string Fulltext = "";

        if (fileInfo.Exists)
        {
            StreamReader reader = new StreamReader(filePath);
            Fulltext = reader.ReadToEnd();
            reader.Close();
        }
        else
            return;

        int MaxInfoLength = 8; //(KeyType/ValueType/Desc/1/2/3/4/5)
        int lineIndex = 0;
        string[] lines = Fulltext.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

        foreach (string line in lines)
        {
            if (!line.Trim().StartsWith("//"))
            {
                string[] valueArray = line.Split(',');
                if (valueArray[0] == "end;")
                    break;

                if (valueArray.Length < MaxInfoLength)
                {
                    Debug.LogError("LevelUpSystemInfo Error LineIndex = " + lineIndex);
                    break;
                }

                string keyType = valueArray[0];
                string valueType = valueArray[1];
                string Desc = valueArray[2];
                float f0 = float.Parse(valueArray[3]);
                float f1 = float.Parse(valueArray[4]);
                float f2 = float.Parse(valueArray[5]);
                float f3 = float.Parse(valueArray[6]);
                float f4 = float.Parse(valueArray[7]);
                if (Enum.TryParse(keyType, true, out AdventureLevelUpItemType eAdventureLevelUpItemType) == false)
                    continue;

                LevelUpSystemInfoList.TryAdd(eAdventureLevelUpItemType, new List<LevelUpSystemInfo>());
                if (eAdventureLevelUpItemType == AdventureLevelUpItemType.CHARACTER)
                    LevelUpSystemInfoList[eAdventureLevelUpItemType].Add(new LevelUpSystemInfo(AdventureLevelUpItemType.CHARACTER, Desc));
                else if (eAdventureLevelUpItemType == AdventureLevelUpItemType.STAT)
                {
                    AdventureLevelUpStatType eAdventureLevelUpStatType = (AdventureLevelUpStatType)(Enum.Parse(typeof(AdventureLevelUpStatType), valueType, true));
                    LevelUpSystemInfoList[eAdventureLevelUpItemType].Add(new LevelUpSystemInfo(AdventureLevelUpItemType.STAT, eAdventureLevelUpStatType, Desc, f0, f1, f2, f3, f4));
                }
                else if (eAdventureLevelUpItemType == AdventureLevelUpItemType.UTIL)
                    LevelUpSystemInfoList[eAdventureLevelUpItemType].Add(new LevelUpSystemInfo(AdventureLevelUpItemType.UTIL, int.Parse(valueType), Desc, f0, f1, f2, f3, f4));
                else
                    continue;

                ++lineIndex;
            }
        }
    }

    private BuildingInfo GetEmptyBuildingInfo() { return new BuildingInfo(0, 0f, 0f, 0, 0, 0); }

    public BuildingInfo GetBuildingInfo(SpeciesType eSpeciesType, int index)
    {
        if (Oracle.m_eGameType == MapType.BUILD)
        {
            if (BuildingInfoList.ContainsKey(eSpeciesType) == false)
                return GetEmptyBuildingInfo();

            if (index < 0 || index >= BuildingInfoList[eSpeciesType].Count)
                return GetEmptyBuildingInfo();

            return BuildingInfoList[eSpeciesType][index];
        }
        else if(Oracle.m_eGameType == MapType.SPAWN)
        {
            if (BuildingInfoList2.ContainsKey(eSpeciesType) == false)
                return GetEmptyBuildingInfo();

            if (index < 0 || index >= BuildingInfoList2[eSpeciesType].Count)
                return GetEmptyBuildingInfo();

            return BuildingInfoList2[eSpeciesType][index];
        }
        else
        {
            if (BuildingInfoList3.ContainsKey(eSpeciesType) == false)
                return GetEmptyBuildingInfo();

            if (index < 0 || index >= BuildingInfoList3[eSpeciesType].Count)
                return GetEmptyBuildingInfo();

            return BuildingInfoList3[eSpeciesType][index];
        }
    }

    public StageInfo GetStageInfo(byte index)
    {
        if (Oracle.m_eGameType == MapType.BUILD)
        {
            if (index < 0 || index >= StageInfoList.Count)
                return new StageInfo(0, 0, 0f, 0, 0, 0);

            return StageInfoList[index];
        }
        else
        {
            if (index < 0 || index >= StageInfoList2.Count)
                return new StageInfo(0, 0, 0f, 0, 0, 0);

            return StageInfoList2[index];
        }
    }

    public StageInfo_Adv GetStageInfo_Adv(byte index)
    {
        if (index < 0 || index >= StageInfoList3.Count)
            return new StageInfo_Adv(0, 0, 0f, 0f, 0, 0);

        return StageInfoList3[index];
    }

    public Sprite GetCursorSprite(int index)
    {
        if (index < 0 || index >= CursorSpriteArray.Length)
            return null;

        if (UnlockManager.Instance.CheckCursorItem(index) == false)
            return null;

        return CursorSpriteArray[index];
    }

    public ControlInfo GetControlInfoData()
    {
        ControlInfo controlInfo = new ControlInfo(ControlInfo_Game2.Cost, ControlInfo_Game2.UnitAtk[0], ControlInfo_Game2.UnitAtk[1], ControlInfo_Game2.UnitAtk[2], ControlInfo_Game2.UnitAtk[3], ControlInfo_Game2.UnitAtk[4], ControlInfo_Game2.WorkmanCost, ControlInfo_Game2.BossMoneyEarnedList, ControlInfo_Game2.BossHPList);
        return controlInfo;
    }

    public void RefillUnitAchievementInfo(List<KeyAchievementInfo> VolatilityKeyAchievementInfoList, List<UnitAchievementInfo> VolatilityUnitAchievementInfoList)
    {
        if (VolatilityKeyAchievementInfoList == null || VolatilityUnitAchievementInfoList == null)
            return;

        VolatilityKeyAchievementInfoList.Clear();
        for (int i = 0; i < KeyAchievementInfoList.Count; ++i)
        {
            KeyAchievementInfo tempKeyAchievementInfo = new KeyAchievementInfo(KeyAchievementInfoList[i]);
            tempKeyAchievementInfo.ShuffleChoiceKey();
            VolatilityKeyAchievementInfoList.Add(tempKeyAchievementInfo);
        }

        VolatilityUnitAchievementInfoList.Clear();
        for (int i = 0; i < UnitAchievementInfoList.Count; ++i)
        {
            VolatilityUnitAchievementInfoList.Add(UnitAchievementInfoList[i]);
        }
    }

    public List<LevelUpSystemInfo> GetLevelUpSystemDataList(AdventureLevelUpItemType eAdventureLevelUpItemType)
    {
        if (LevelUpSystemInfoList.ContainsKey(eAdventureLevelUpItemType) == false)
            return null;

        return LevelUpSystemInfoList[eAdventureLevelUpItemType];
    }
    public LevelUpSystemInfo GetLevelUpSystemData(AdventureLevelUpItemType eAdventureLevelUpItemType, int index)
    {
        if (LevelUpSystemInfoList.ContainsKey(eAdventureLevelUpItemType) == false)
            return new LevelUpSystemInfo(AdventureLevelUpItemType.MAX, "");

        switch (eAdventureLevelUpItemType)
        {
            case AdventureLevelUpItemType.CHARACTER:
                return LevelUpSystemInfoList[eAdventureLevelUpItemType][0];
            case AdventureLevelUpItemType.UTIL:
                for (int i = 0; i < LevelUpSystemInfoList[eAdventureLevelUpItemType].Count; ++i)
                {
                    if (LevelUpSystemInfoList[eAdventureLevelUpItemType][i].ValueType == index)
                    {
                        index = i;
                        break;
                    }
                }
                
                break;
        }

        if (index < 0 || index >= LevelUpSystemInfoList[eAdventureLevelUpItemType].Count)
            return new LevelUpSystemInfo(AdventureLevelUpItemType.MAX, "");

        return LevelUpSystemInfoList[eAdventureLevelUpItemType][index];
    }
}
