using GameDefines;
using OptionDefines;
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UnlockManager : Singleton<UnlockManager>
{
    [SerializeField] public bool bSteamDeactivate = false;

    private List<int> UnlockCursorList;
    private List<int> UnlockShopProductList;
    private List<SpeciesType> UnlockSpeciesTypeList;
    private List<OtherShopProductItemType> UnlockOtherShopProductItemList;

    private CursorDatabase C_Database = new CursorDatabase();
    private ShopDatabase S_Database = new ShopDatabase();
    private List<CursorInfo> CursorInfoList;
    private List<ShopInfo> ShopInfoList;

    public Action<int> OnCursorUnlockEvent;

    //private string m_CloudFileName = "";
    private string m_SaveKey = "";
    private bool bNewrecord = false;

    private struct STEAMCLOUDDATA          // Cloud
    {
        public string ruby;
        public string unlockCharacter;
        public STEAMCLOUDDATA(bool init)
        {
            ruby = "0";
            unlockCharacter = "0";
        }
    }
    private STEAMCLOUDDATA m_SteamCloudData;
    private struct STEAMACHIEVEMENTDATA     // Achievement
    {
        public int iCompleteStageIndex;
        public int iLobbyDropCharacterCount;
        public int iCriticalCount;
        public int iNPNG;
        public int iBowMasters;
        public int iMaxCompleteStage;
        public int iCompleteStageTime;

        public bool bPARATROOPER;
        public bool bLOSER;

        public Dictionary<string, bool> SpawnMapAchievement;

        public STEAMACHIEVEMENTDATA(bool init)
        {
            iCompleteStageIndex = 0;
            iLobbyDropCharacterCount = 0;
            iCriticalCount = 0;
            iNPNG = 0;
            iBowMasters = 0;

            bPARATROOPER = false;
            bLOSER = false;

            SpawnMapAchievement = new Dictionary<string, bool>();

            iMaxCompleteStage = 0;
            iCompleteStageTime = 0;
        }
    }
    private STEAMACHIEVEMENTDATA m_SteamAchievementData;

    private KeyValuePair<SpeciesType, int>[] m_archersCache =
    {
        new KeyValuePair<SpeciesType, int>(SpeciesType.ELF, 0),
        new KeyValuePair<SpeciesType, int>(SpeciesType.ELF, 1),
        new KeyValuePair<SpeciesType, int>(SpeciesType.ELF, 2),
        new KeyValuePair<SpeciesType, int>(SpeciesType.ELF, 3),
        new KeyValuePair<SpeciesType, int>(SpeciesType.ELF, 4),
        new KeyValuePair<SpeciesType, int>(SpeciesType.DARKELF, 3),
        new KeyValuePair<SpeciesType, int>(SpeciesType.GOBLIN, 1),
        new KeyValuePair<SpeciesType, int>(SpeciesType.GOBLIN, 3),
        new KeyValuePair<SpeciesType, int>(SpeciesType.HUMAN, 1),
        new KeyValuePair<SpeciesType, int>(SpeciesType.HUMAN, 3)
    };

    protected override void Awake()
    {
        base.Awake();

        if (Application.runInBackground == false)
            Application.runInBackground = true;

        UnlockCursorList = new List<int>();
        UnlockShopProductList = new List<int>();
        UnlockSpeciesTypeList = new List<SpeciesType>();
        UnlockOtherShopProductItemList = new List<OtherShopProductItemType>();

        CursorInfoFileRead();
        ShopInfoFileRead();

        m_SteamCloudData = new STEAMCLOUDDATA(true);
        m_SteamAchievementData = new STEAMACHIEVEMENTDATA(true);
    }

    void Start()
    {
        SteamWorksInitialized();
    }

    private void CursorInfoFileRead()
    {
        C_Database.SetData();
        CursorInfoList = C_Database.CursorInfoList;

        UnlockCursorList.Clear();
        UnlockCursorList.Add(0);
        UnlockCursorList.Add(1);
        UnlockCursorList.Add(2);
        UnlockCursorList.Add(3);

        UnlockSpeciesTypeList.Clear();
        UnlockSpeciesTypeList.Add(SpeciesType.ELF);
        UnlockSpeciesTypeList.Add(SpeciesType.HUMAN);
        UnlockSpeciesTypeList.Add(SpeciesType.DRAKE);
        UnlockSpeciesTypeList.Add(SpeciesType.ORC);
        UnlockSpeciesTypeList.Add(SpeciesType.SAMURAI);
        UnlockSpeciesTypeList.Add(SpeciesType.ANDROID);
        UnlockSpeciesTypeList.Add(SpeciesType.DEMON);
        UnlockSpeciesTypeList.Add(SpeciesType.GOBLIN);
        UnlockSpeciesTypeList.Add(SpeciesType.NECROMANCER);
        UnlockSpeciesTypeList.Add(SpeciesType.ZOMBIE);

        if (bSteamDeactivate)
        {
            AddAllCharacter();
            AddOtherShopProductItem(0);
        }
    }

    private void ShopInfoFileRead()
    {
        S_Database.SetData();
        ShopInfoList = S_Database.ShopInfoList;
    }

    private void SteamWorksInitialized()
    {
        if (bSteamDeactivate)
            return;

        m_SaveKey = "c_";
        // Cloud
        string CloudData = FBPP.GetString(m_SaveKey, "");
        ParseSteamCloudData(CloudData);

        if (SteamManager.Initialized == false)
            return;

        SteamUserStats.GetStat("CLEARTIME_STAT", out m_SteamAchievementData.iCompleteStageTime);
        SteamUserStats.GetStat("COMPLETE_STAGE", out m_SteamAchievementData.iMaxCompleteStage);

        // Achievement
        bool bAchievementCompleted = false;
        for (int i = 1; i <= 10; ++i)
        {
            string APIKey = string.Format("CURSOR{0}0", i);
            SteamUserStats.GetAchievement(APIKey, out bAchievementCompleted);
            if (bAchievementCompleted)
            {
                int index = -1;
                switch (APIKey)
                {
                    case "CURSOR10":
                        index = 4;
                        m_SteamAchievementData.iCompleteStageIndex = 11;
                        break;
                    case "CURSOR20":
                        m_SteamAchievementData.iCompleteStageIndex = 21;
                        index = 5;
                        break;
                    case "CURSOR30":
                        m_SteamAchievementData.iCompleteStageIndex = 31;
                        index = 6;
                        break;
                    case "CURSOR40":
                        m_SteamAchievementData.iCompleteStageIndex = 41;
                        index = 7;
                        break;
                    case "CURSOR50":
                        m_SteamAchievementData.iCompleteStageIndex = 51;
                        index = 8;
                        break;
                    case "CURSOR60":
                        m_SteamAchievementData.iCompleteStageIndex = 61;
                        index = 9;
                        break;
                    case "CURSOR70":
                        m_SteamAchievementData.iCompleteStageIndex = 71;
                        index = 10;
                        break;
                    case "CURSOR80":
                        m_SteamAchievementData.iCompleteStageIndex = 81;
                        index = 11;
                        break;
                    case "CURSOR90":
                        m_SteamAchievementData.iCompleteStageIndex = 91;
                        index = 12;
                        break;
                    case "CURSOR100":
                        m_SteamAchievementData.iCompleteStageIndex = 101;
                        index = 18;
                        break;
                };

                UnlockCursor(index);
            }
        }

        if (SteamUserStats.GetAchievement("BOWMASTERS", out bAchievementCompleted))
        {
            if (bAchievementCompleted)
            {
                m_SteamAchievementData.iBowMasters = 7;
                UnlockCursor(13);
            }
        }

        if (SteamUserStats.GetAchievement("PARATROOPER", out bAchievementCompleted))
        {
            if (bAchievementCompleted)
            {
                m_SteamAchievementData.iLobbyDropCharacterCount = 50;
                UnlockCursor(17);
            }
        }

        if (SteamUserStats.GetAchievement("LOSER", out bAchievementCompleted))
        {
            if (bAchievementCompleted)
            {
                m_SteamAchievementData.bLOSER = true;
                UnlockCursor(15);
            }
        }

        if (SteamUserStats.GetAchievement("LOSER", out bAchievementCompleted))
        {
            if (bAchievementCompleted)
            {
                m_SteamAchievementData.iNPNG = 50;
                UnlockCursor(16);
            }
        }

        if (SteamUserStats.GetAchievement("CRITICAL", out bAchievementCompleted))
        {
            if (bAchievementCompleted)
            {
                m_SteamAchievementData.iCriticalCount = 1000;
                UnlockCursor(14);
            }
        }

        /*SpawnMapAchievement*/
        m_SteamAchievementData.SpawnMapAchievement.Clear();
        m_SteamAchievementData.SpawnMapAchievement.Add("KEYCHOICE", false);
        m_SteamAchievementData.SpawnMapAchievement.Add("RIVAL", false);
        m_SteamAchievementData.SpawnMapAchievement.Add("RELIGION", false);
        m_SteamAchievementData.SpawnMapAchievement.Add("VILLAIN", false);
        m_SteamAchievementData.SpawnMapAchievement.Add("MONSTER", false);
        m_SteamAchievementData.SpawnMapAchievement.Add("DEATH", false);
        m_SteamAchievementData.SpawnMapAchievement.Add("HOTTER", false);
        m_SteamAchievementData.SpawnMapAchievement.Add("REVIL", false);
        m_SteamAchievementData.SpawnMapAchievement.Add("DND", false);
        m_SteamAchievementData.SpawnMapAchievement.Add("UNTOUCHABLE", false);

        if (SteamUserStats.GetAchievement("KEYCHOICE", out bAchievementCompleted))
        {
            m_SteamAchievementData.SpawnMapAchievement["KEYCHOICE"] = bAchievementCompleted;
        }
        if (SteamUserStats.GetAchievement("RIVAL", out bAchievementCompleted))
        {
            m_SteamAchievementData.SpawnMapAchievement["RIVAL"] = bAchievementCompleted;
        }
        if (SteamUserStats.GetAchievement("RELIGION", out bAchievementCompleted))
        {
            m_SteamAchievementData.SpawnMapAchievement["RELIGION"] = bAchievementCompleted;
        }
        if (SteamUserStats.GetAchievement("VILLAIN", out bAchievementCompleted))
        {
            m_SteamAchievementData.SpawnMapAchievement["VILLAIN"] = bAchievementCompleted;
        }
        if (SteamUserStats.GetAchievement("MONSTER", out bAchievementCompleted))
        {
            m_SteamAchievementData.SpawnMapAchievement["MONSTER"] = bAchievementCompleted;
        }
        if (SteamUserStats.GetAchievement("DEATH", out bAchievementCompleted))
        {
            m_SteamAchievementData.SpawnMapAchievement["DEATH"] = bAchievementCompleted;
        }
        if (SteamUserStats.GetAchievement("HOTTER", out bAchievementCompleted))
        {
            m_SteamAchievementData.SpawnMapAchievement["HOTTER"] = bAchievementCompleted;
        }
        if (SteamUserStats.GetAchievement("REVIL", out bAchievementCompleted))
        {
            m_SteamAchievementData.SpawnMapAchievement["REVIL"] = bAchievementCompleted;
        }
        if (SteamUserStats.GetAchievement("DND", out bAchievementCompleted))
        {
            m_SteamAchievementData.SpawnMapAchievement["DND"] = bAchievementCompleted;
        }
        if (SteamUserStats.GetAchievement("UNTOUCHABLE", out bAchievementCompleted))
        {
            m_SteamAchievementData.SpawnMapAchievement["UNTOUCHABLE"] = bAchievementCompleted;
        }

        OptionManager.Instance.FirstSetCursor();

        // Rank
        CSteamLeaderboards.Instance.DownloadScores();
    }

    public void SetSteamCloudFile()
    {
        if (GameManager.Instance.gameMoney >= 0)
        {
            m_SteamCloudData.ruby = GameManager.Instance.gameMoney.ToString();
        }
    }

    private void ParseSteamCloudData(string data)
    {
        if (data.Length == 0)
            return;

        string[] valueArray = data.Split('/');
        if (valueArray.Length < 2)
        {
            Debug.LogError("ParseSteamCloudData error");
            return;
        }

        m_SteamCloudData.ruby = valueArray[0];
        m_SteamCloudData.unlockCharacter = valueArray[1];

        uint loadedValue = uint.Parse(m_SteamCloudData.unlockCharacter);
        for (int i = 0; i < ShopInfoList.Count; ++i)
        {
            if ((loadedValue & (1 << i)) != 0)
            {
                UnlockShopProductList.Add(i);

                SpeciesType eSpeciesType = ShopInfoList[i].eSpeciesType;
                if (eSpeciesType == SpeciesType.NONE || eSpeciesType == SpeciesType.MAX)
                {
                    AddOtherShopProductItem(ShopInfoList[i].SlotId);
                }
                else
                {
                    AddUnlockSpeciesType(eSpeciesType);
                }
            }
        }
        GameManager.Instance.SetGameMoney(int.Parse(m_SteamCloudData.ruby));
    }

    public void WriteToSteamCloud()
    {
        string data = m_SteamCloudData.ruby + "/";

        int combinedValue = 0;
        for (int i = 0; i < UnlockShopProductList.Count; ++i)
        {
            combinedValue |= (1 << UnlockShopProductList[i]);
        }
        data += combinedValue;

        FBPP.SetString(m_SaveKey, data);
        FBPP.Save();
    }

    public void SetAchievement(string APIKey)
    {
        if (bSteamDeactivate)
            return;

        if (APIKey.Length == 0)
            return;

        if (SteamManager.Initialized)
        {
            Steamworks.SteamUserStats.GetAchievement(APIKey, out bool bAchievementCompleted);
            if (!bAchievementCompleted)
            {
                SteamUserStats.SetAchievement(APIKey);
            }
        }
    }

    public void SellShopProductItem(int index)
    {
        if (index < 0 || index >= ShopInfoList.Count)
            return;

        if (UnlockManager.Instance.CheckShopProductItem(index) == false)
        {
            int cost = ShopInfoList[index].Cost;
            if (GameManager.instance.CheckSell(-cost))
            {
                SendShopProductItemPacket(index);
            }
        }
    }

    public void SendShopProductItemPacket(int index)
    {
        Debug.Log("Packet > SendShopProductItemPacket");
        RequestShopProductItemPacket(index);
    }
    public void RequestShopProductItemPacket(int index)
    {
        Debug.Log("Packet > RequestShopProductItemPacket");

        if (index < 0 || index >= ShopInfoList.Count)
        {
            Debug.Log("RequestShopProductItemPacket error = " + index);
            return;
        }

        UnlockShopProductList.Add(index);

        SpeciesType eSpeciesType = ShopInfoList[index].eSpeciesType;
        if (eSpeciesType == SpeciesType.NONE || eSpeciesType == SpeciesType.MAX)
        {
            AddOtherShopProductItem(ShopInfoList[index].SlotId);
        }
        else
        {
            AddUnlockSpeciesType(eSpeciesType);
        }

        GameManager.Instance.AddGameMoney(-(ShopInfoList[index].Cost));
        UIManager.Instance.SellShopItem(index);
        SoundManager.Instance.PlayUISound(UISoundType.SUCCESS);

        SetSteamCloudFile();
    }

    public bool CheckCursorItem(int index)
    {
        for (int i = 0; i < UnlockCursorList.Count; ++i)
        {
            if (UnlockCursorList[i] == index)
                return true;
        }

        return false;
    }

    public bool CheckShopProductItem(int index)
    {
        for (int i = 0; i < UnlockShopProductList.Count; ++i)
        {
            if (UnlockShopProductList[i] == index)
                return true;
        }

        return false;
    }

    public CursorInfo GetCursorInfoList(int index)
    {
        if (index < 0 || index >= CursorInfoList.Count)
            return new CursorInfo(0, 0f, 0f, 0f, "");

        return CursorInfoList[index];
    }

    public ShopInfo GetShopProductItemInfo(int index)
    {
        if (index < 0 || index >= ShopInfoList.Count)
            return new ShopInfo("", "", 0, SpeciesType.NONE, 0);

        return ShopInfoList[index];
    }

    public List<SpeciesType> GetUseableSpeciesTypeList()
    {
        List<SpeciesType> UseableSpeciesTypeList = new List<SpeciesType>();
        for (int i = 0; i < UnlockSpeciesTypeList.Count; ++i)
        {
            UseableSpeciesTypeList.Add(UnlockSpeciesTypeList[i]);
        }

        return UseableSpeciesTypeList;
    }

    public SpeciesType GetRandomSpeciesType()
    {
        int maxCount = UnlockSpeciesTypeList.Count;
        if (maxCount == 0)
            return SpeciesType.NONE;

        int index = Oracle.RandomDice(0, maxCount);
        return UnlockSpeciesTypeList[index];
    }

    public bool CheckUnlockOtherShopProductItemList(OtherShopProductItemType eOtherShopProductItemType)
    {
        for (int i = 0; i < UnlockOtherShopProductItemList.Count; ++i)
        {
            if (UnlockOtherShopProductItemList[i] == eOtherShopProductItemType)
                return true;
        }

        return false;
    }

    public void AddOtherShopProductItem(int id)
    {
        OtherShopProductItemType eOtherShopProductItemType = (OtherShopProductItemType)id;
        UnlockOtherShopProductItemList.Add(eOtherShopProductItemType);

        if (eOtherShopProductItemType == OtherShopProductItemType.GAMESPEED2X || eOtherShopProductItemType == OtherShopProductItemType.GAMESPEED3X)
            GameManager.Instance.AddTimeScaleSpeed(eOtherShopProductItemType);
    }

    public void AddUnlockSpeciesType(SpeciesType eSpeciesType)
    {
        if (eSpeciesType == SpeciesType.NONE || eSpeciesType == SpeciesType.MAX)
            return;

        for (int i = 0; i < UnlockSpeciesTypeList.Count; ++i)
        {
            if (UnlockSpeciesTypeList[i] == eSpeciesType)
                return;
        }

        UnlockSpeciesTypeList.Add(eSpeciesType);
    }

    public void UnlockCursor(int index)
    {
        if (CheckCursorItem(index))
            return;

        UnlockCursorList.Add(index);
        OnCursorUnlockEvent?.Invoke(index);
    }

    public void CheckStageAchievement(byte stageIndex)
    {
        if (m_SteamAchievementData.iCompleteStageIndex >= stageIndex)
            return;

        SetAchievement(Oracle.GetAchievementAPIName_Stage(stageIndex));
    }
    public void CheckBowMasters(SpeciesType eSpeciesType, int index)
    {
        if (m_SteamAchievementData.iBowMasters >= 7)
            return;

        for (int i = 0; i < m_archersCache.Length; ++i)
        {
            if (m_archersCache[i].Key == eSpeciesType && m_archersCache[i].Value == index)
            {
                m_archersCache[i] = new KeyValuePair<SpeciesType, int>(SpeciesType.NONE, -1);
                m_SteamAchievementData.iBowMasters++;
                break;
            }
        }

        if (m_SteamAchievementData.iBowMasters >= 7)
        {
            SetAchievement("BOWMASTERS");
        }
    }
    public void AddLobbyDropCharacterCount()
    {
        if (m_SteamAchievementData.iLobbyDropCharacterCount >= 50)
            return;

        m_SteamAchievementData.iLobbyDropCharacterCount++;
        if (m_SteamAchievementData.iLobbyDropCharacterCount >= 50)
        {
            SetAchievement("PARATROOPER");
        }
    }
    public void CheckLoser()
    {
        if (m_SteamAchievementData.bLOSER)
            return;

        SetAchievement("LOSER");
    }
    public void AddSurviveWithHP1()
    {
        if (m_SteamAchievementData.iNPNG >= 50)
            return;

        m_SteamAchievementData.iNPNG++;
        if (m_SteamAchievementData.iNPNG >= 50)
        {
            SetAchievement("NPNG");
        }
    }
    public void AddCriticalCount()
    {
        if (m_SteamAchievementData.iCriticalCount >= 1000)
            return;

        m_SteamAchievementData.iCriticalCount++;
        if (m_SteamAchievementData.iCriticalCount >= 1000)
        {
            SetAchievement("CRITICAL");
        }
    }

    public void SetSpawnMapAchievement(string AchievementName)
    {
        if (AchievementName.Length == 0)
            return;

        if (m_SteamAchievementData.SpawnMapAchievement.ContainsKey(AchievementName))
        {
            if (m_SteamAchievementData.SpawnMapAchievement[AchievementName] == false)
            {
                SetAchievement(AchievementName);
                m_SteamAchievementData.SpawnMapAchievement[AchievementName] = true;
            }
        }
    }

    
    public void SetSteamStat_STAGE(int iStage, int iSeconds)
    {
        if (bSteamDeactivate)
            return;

        if (iStage == 0)
            return;

        if (SteamManager.Initialized == false)
            return;

        if (m_SteamAchievementData.iMaxCompleteStage < iStage)
        {
            m_SteamAchievementData.iMaxCompleteStage = iStage;
            m_SteamAchievementData.iCompleteStageTime = iSeconds;
            SteamUserStats.SetStat("COMPLETE_STAGE", iStage);
            SteamUserStats.SetStat("CLEARTIME_STAT", iSeconds);

            if (UIManager.Instance && !bNewrecord)
            {
                bNewrecord = true;
                UIManager.Instance.ShowNoticeText("NEW RECORD STAGE", true);
            }
        }
    }

    protected override void OnDestroy()
    {
        UnlockManager.Instance.WriteToSteamCloud();
        base.OnDestroy();
    }

    // Cmd
    public void AddAllCursor()
    {
        for (int i = UnlockCursorList.Count; i < CursorInfoList.Count; ++i)
        {
            UnlockCursor(i);
        }
    }

    public void AddAllCharacter()
    {
        for (int i = 0; i < (int)SpeciesType.MAX; ++i)
        {
            AddUnlockSpeciesType((SpeciesType)i);
        }
    }

    public void AddAllShopItem()
    {
        AddAllCharacter();

        for (int i = 0; i < ShopInfoList.Count; ++i)
        {
            SpeciesType eSpeciesType = ShopInfoList[i].eSpeciesType;
            if (eSpeciesType == SpeciesType.NONE || eSpeciesType == SpeciesType.MAX)
            {
                AddOtherShopProductItem(ShopInfoList[i].SlotId);
            }
        }
    }
}
