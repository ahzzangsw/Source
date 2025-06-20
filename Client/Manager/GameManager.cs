using GameDefines;
using UIDefines;
using System.Collections.Generic;
using UnityEngine;
using OptionDefines;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [Header("Test")]
    [SerializeField] public bool bTest = false;
    [SerializeField] public bool bForceTest = false;
    [SerializeField] public SpeciesType m_TestSpeciesType = SpeciesType.NONE;

    [Header("")]
    [SerializeField] private int gameStartDelay = 0;

    public bool isPause { get; private set; } = false;

    public byte stageIndex { get; set; } = 1;

    public bool isDie = false;
    private bool isComplete = false;

    // GameInfo
    private bool isConnectedInGame = false;
    private bool isPlayGame = false;
    private float fPlayTime = 0f;
    private float fTimeScale = 1f;
    private List<float> TimeScaleList = new List<float>();
    private bool m_bRetryLast = false;
    public uint gameTimeSec { get; private set; } = 0;
    public uint gameClearTimeSec { get; private set; } = 86400;
    public int gameMoney { get; private set; } = 0;    // Ruby

    public byte TimeScaleIndex = 0;
    public float gameSpeed = 0f;

    // 저장
    public int m_SaveCurrentHP { get; private set; } = 0;
    public int m_Life { get; private set; } = 0;
    private List<SpeciesType> SaveAdventureModePlayerListTemp = null;
    private List<int> PlayerAdventureLevelUpStat = null;

    public void OnLoadComplete(AsyncOperation operation)
    {
        Clear();
        InGame();
    }
    
    private void Clear()
    {
        fTimeScale = 1f;
        Time.timeScale = fTimeScale;

        isConnectedInGame = false;
        isPlayGame = false;

        stageIndex = 1;
        if (m_bRetryLast)
            stageIndex = 100;

        fPlayTime = 0f;
        gameTimeSec = 0;
    }

    public void InGame()
    {
        MapManager.Instance.LoadMap(Oracle.m_eGameType);
        if (ItemManager.Instance == null)
        {
            Debug.Log("ItemManager is null");
        }

        bool bEditorTest = false;
#if UNITY_EDITOR
        bEditorTest = true;
#endif

        TimeScaleList.Clear();
        TimeScaleList.Add(1f);
        if (bEditorTest || UnlockManager.Instance.CheckUnlockOtherShopProductItemList(OtherShopProductItemType.GAMESPEED2X))
        {
            TimeScaleList.Add(2f);
        }
        if (bEditorTest || UnlockManager.Instance.CheckUnlockOtherShopProductItemList(OtherShopProductItemType.GAMESPEED3X))
        {
            if (Oracle.m_eGameType != MapType.ADVENTURE)
                TimeScaleList.Add(3f);
        }
        TimeScaleList.Sort();

        UIBase ui = UIManager.Instance.GetUI(UIIndexType.GAMEINFO);
        if (ui)
        {
            UI_GameInfo gameInfoUI = ui as UI_GameInfo;
            if (gameInfoUI)
            {
                gameInfoUI.UpdateGameSpeed(OtherShopProductItemType.MAX, TimeScaleList.Count > 1);
            }
        }

        CameraManager.Instance.moveSpeed = OptionManager.Instance.fCameraSpeed;

        if (Oracle.m_eGameType == MapType.SPAWN)
        {
            UIManager.Instance.ShowUI(UIIndexType.CHARACTERDECK);
            CameraManager.Instance.bComplete = true;
            return;
        }

        SoundManager.Instance.PlayUISound(UISoundType.INGAMESTART);
        ForceGameStart();
    }

    void Update()
    {
        if (isComplete)
            return;

        if (isDie || !isConnectedInGame)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPause();
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            ChangeGameSpeed();
        }

        //Test
#if UNITY_EDITOR
        if (bForceTest)
        {
            Player MyPlayer = GetPlayer();
            MyPlayer.m_eSpeciesType = m_TestSpeciesType;
            UIManager.Instance.UpdateSpeciesType(m_TestSpeciesType, AttributeType.NONE);
            bForceTest = false;
        }
        if (Input.GetKeyUp(KeyCode.N))
        {
            //NextWave();
        }
#endif

        if (gameSpeed > 0f)
        {
            if (Time.timeScale != gameSpeed)
                Time.timeScale = gameSpeed;

            gameSpeed = 0f;
        }
    }

    void LateUpdate()
    {
        if (isComplete)
            return;

        if (isDie || !isConnectedInGame)
            return;

        fPlayTime += Time.deltaTime;
        if (fPlayTime >= 1f)
        {
            fPlayTime = 0f;
            ++gameTimeSec;

            // 처음 시작
            if (gameTimeSec > gameStartDelay && !isPlayGame)
            {
                isPlayGame = true;
                StartWave();
            }
        }
    }
    public void SetPause()
    {
        if (UIManager.Instance.bShowDialog)
            return;

        if (Oracle.m_eGameType == MapType.ADVENTURE)
        {
            if (UIManager.Instance.IsShow(UIIndexType.PICKER))
                return;
        }

        isPause = UIManager.Instance.GetUI(UIIndexType.EXITMENU).IsShow();
        if (!isPause)
        {
            UIManager.Instance.ShowUI(UIIndexType.EXITMENU);
            GameStop(true);
            SoundManager.Instance.PlayUISound(UISoundType.PAUSE);
        }
        else
        {
            UIManager.Instance.HideUI(UIIndexType.EXITMENU);
            GameStop(false);
            SoundManager.Instance.PlayUISound(UISoundType.UNPAUSE);
        }
    }

    public void NextWave()
    {
        ++stageIndex;
        SetWaveInfo();

        isPlayGame = false;
    }

    public Player GetPlayer()
    {
        GameObject PlayerObject = GameObject.FindWithTag("Player");
        if (PlayerObject != null)
        {
            Player MyPlayer = PlayerObject.GetComponent<Player>();
            if (MyPlayer != null)
                return MyPlayer;
        }

        return null;
    }

    void SetWaveInfo()  // Wave마다 셋팅되는 랜덤값
    {
        Player MyPlayer = GetPlayer();
        if (MyPlayer == null)
            return;

        // Achievement
        if (Oracle.m_eGameType == MapType.BUILD)
        {
            UnlockManager.Instance.CheckStageAchievement(stageIndex);
            if (GetPlayer().GetHp() == 1)
            {
                UnlockManager.Instance.AddSurviveWithHP1();
            }

            if (bTest)
                MyPlayer.m_eSpeciesType = m_TestSpeciesType;
            else
                MyPlayer.m_eSpeciesType = UnlockManager.Instance.GetRandomSpeciesType();
        }

        // Set
        MyPlayer.SetWave();
        MonsterPool.Instance.SetWaveInfo(stageIndex);
        ImpedimentsPool.Instance.SetWaveInfo(stageIndex);

        BGMType eBGMType = stageIndex % 10 == 0 ? BGMType.BOSS : BGMType.STAGE;
        if (Oracle.m_eGameType == MapType.SPAWN)
        {
            eBGMType = BGMType.STAGE;
            if (stageIndex == 101)
            {
                UIManager.Instance.ShowUI(UIIndexType.COMPLETE);
            }
        }
        else
        {
            if (stageIndex == 100)
            {
                SoundManager.Instance.StopBGM();
                return;
            }

            if (stageIndex > 100)
            {
                eBGMType = BGMType.ENDING;
            }
        }

        SoundManager.Instance.PlayBGM(eBGMType, stageIndex);

        UnlockManager.Instance.SetSteamStat_STAGE(stageIndex, ((int)gameTimeSec));
    }

    private void StartWave()
    {
        MonsterPool.Instance.StartSpawn();
    }

    public bool CheckSell(int newMoney)
    {
        if (gameMoney + newMoney >= 0)
        {
            return true;
        }

        SoundManager.Instance.PlayUISound(UISoundType.FAIL);
        return false;
    }

    public bool RefreshGameMoney(int newMoney)
    {
        if (gameMoney + newMoney >= 0)
        {
            gameMoney += newMoney;
            return true;
        }

        SoundManager.Instance.PlayUISound(UISoundType.FAIL);
        return false;
    }

    public void AddGameMoney(int newMoney)
    {
        gameMoney += newMoney;
        UnlockManager.Instance.SetSteamCloudFile();
    }
    
    public void SetGameMoney(int newMoney)
    {
        gameMoney = newMoney;
        UnlockManager.Instance.SetSteamCloudFile();
    }

    public void ReStart(bool bRetryLast)
    {
        SoundManager.Instance.ReStart();
        SoundManager.Instance.PlayUISound(UISoundType.USEITEM);

        ClearAdventureMode(bRetryLast);

        if (Oracle.m_eGameType == MapType.BUILD)
            SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single).completed += OnLoadComplete;
        else if (Oracle.m_eGameType == MapType.SPAWN)
            SceneManager.LoadSceneAsync("GameScene2", LoadSceneMode.Single).completed += OnLoadComplete;
        else if (Oracle.m_eGameType == MapType.ADVENTURE)
            SceneManager.LoadSceneAsync("GameScene3", LoadSceneMode.Single).completed += OnLoadComplete;
    }

    public void Exit()
    {
        SoundManager.Instance.ReStart();
        SoundManager.Instance.PlayUISound(UISoundType.BACK);

        ClearAdventureMode(false);

        Clear();
        OptionDefines.CSetOption.GameQuit();
        SceneManager.LoadScene("LobbyScene");
        isComplete = false;
    }

    public void ChangeGameSpeed()
    {
        //Adventure모드에서 사용안함
        //if (Oracle.m_eGameType == MapType.ADVENTURE)
        //    return;

        ++TimeScaleIndex;
        fTimeScale = GetTimeScale();

        UIBase ui = UIManager.Instance.GetUI(UIIndexType.GAMEINFO);
        if (ui)
        {
            UI_GameInfo gameInfoUI = ui as UI_GameInfo;
            if (gameInfoUI)
            {
                OtherShopProductItemType eOtherShopProductItemType = OtherShopProductItemType.MAX;
                if (fTimeScale == 2f)
                    eOtherShopProductItemType = OtherShopProductItemType.GAMESPEED2X;
                else if(fTimeScale == 3f)
                    eOtherShopProductItemType = OtherShopProductItemType.GAMESPEED3X;

                gameInfoUI.UpdateGameSpeed(eOtherShopProductItemType, TimeScaleList.Count > 1);
            }
        }

        Time.timeScale = fTimeScale;
    }

    public float GetTimeScale()
    {
        if (TimeScaleIndex < 0 || TimeScaleIndex >= TimeScaleList.Count)
            TimeScaleIndex = 0;

        return TimeScaleList[TimeScaleIndex];
    }

    public void AddTimeScaleSpeed(OtherShopProductItemType eOtherShopProductItemType)
    {
        float addValue = 0f;
        if (eOtherShopProductItemType == OtherShopProductItemType.GAMESPEED2X)
            addValue = 2f;
        if (eOtherShopProductItemType == OtherShopProductItemType.GAMESPEED3X)
            addValue = 3f;

        if (addValue == 0f)
            return;

        for (int i = 0; i < TimeScaleList.Count; ++i)
        {
            if (TimeScaleList[i] == addValue)
                return;
        }

        TimeScaleList.Add(addValue);
        TimeScaleList.Sort();
    }

    public void OnGameComplete()
    {
        isComplete = true;
        if(gameClearTimeSec > gameTimeSec)
            gameClearTimeSec = gameTimeSec;

        Time.timeScale = 1f;
        Player MyPlayer = GetPlayer();
        if (MyPlayer)
            MyPlayer.GameComplete(true);

        CameraManager.Instance.bComplete = true;

        UnlockManager.Instance.SetSteamCloudFile();
        UnlockManager.Instance.CheckStageAchievement(101);
    }

    public void GamLoopStart()
    {
        isComplete = false;
        gameTimeSec = gameClearTimeSec;
        Player MyPlayer = GetPlayer();
        if (MyPlayer)
            MyPlayer.GameComplete(false);

        CameraManager.Instance.bComplete = false;
        MonsterPool.instance.SetLoopStageInfo();
        NextWave();
    }

    public void ForceGameStart()
    {
        CameraManager.Instance.bComplete = false;
        isConnectedInGame = true;
        isPlayGame = false;
        isDie = false;
        gameStartDelay = 3;
        SetWaveInfo();

        UIManager.Instance.ShowRank();
    }

    public void GameOver()
    {
        if (Oracle.m_eGameType == MapType.SPAWN)
        {
            CSteamLeaderboards.Instance.UploadScores(stageIndex - 1, (int)gameTimeSec);
        }
    }

    public AdventureLevelType GetAdventureLevel()
    {
        if (stageIndex < 10)
        {
            return AdventureLevelType.LEVEL1;
        }
        else if (stageIndex < 25)
        {
            return AdventureLevelType.LEVEL2;
        }
        else if (stageIndex < 50)
        {
            return AdventureLevelType.LEVEL3;
        }
        else
        {
            return AdventureLevelType.LEVEL4;
        }
    }

    public void GameStop(bool bStop)
    {
        Time.timeScale = bStop ?  0f : fTimeScale;
    }

    public void SaveLastBossInfo()
    {
        if (m_Life > 0)
            return;

        Player MyPlayer = GetPlayer();
        if (MyPlayer == null)
            return;

        m_SaveCurrentHP = MyPlayer.GetHp();
        m_Life = 3;
    }

    public bool CheckActivateLastBoss(bool bAction)
    {
        if (bAction)
            --m_Life;

        return m_Life > 0;
    }

    public void SaveAdventureModePlayerList()
    {
        if (Oracle.m_eGameType != MapType.ADVENTURE)
            return;

        Player MyPlayer = GetPlayer();
        if (MyPlayer == null)
            return;

        SaveAdventureModePlayerListTemp = new List<SpeciesType>();
        for (int i = 0; ; ++i)
        {
            Player_Adventure tempPlayerAdventure = MyPlayer.GetAdventureMainPlayer(i);
            if (tempPlayerAdventure == null)
                break;

            SaveAdventureModePlayerListTemp.Add(tempPlayerAdventure.m_eSpeciesType);
        }

        PlayerAdventureLevelUpStat = new List<int>();
        for (int i = 0; i < (int)(AdventureLevelUpStatType.MAX); ++i)
        {
            int value = MyPlayer.GetAdventureLevelUpStat((AdventureLevelUpStatType)i);
            PlayerAdventureLevelUpStat.Add(value);
        }
    }

    public List<SpeciesType> GetAdventureModePlayerList()
    {
        if (SaveAdventureModePlayerListTemp == null)
            return null;

        if (SaveAdventureModePlayerListTemp.Count == 0)
            return null;

        return SaveAdventureModePlayerListTemp;
    }

    public List<int> GetPlayerAdventureLevelUpStat()
    {
        if (PlayerAdventureLevelUpStat == null)
            return null;

        if (PlayerAdventureLevelUpStat.Count == 0 || PlayerAdventureLevelUpStat.Count < (int)(AdventureLevelUpStatType.MAX))
            return null;

        return PlayerAdventureLevelUpStat;
    }

    private void ClearAdventureMode(bool RetryLast)
    {
        if (Oracle.m_eGameType != MapType.ADVENTURE)
            return;

        m_bRetryLast = RetryLast;
        if (m_bRetryLast == false)
        {
            if (SaveAdventureModePlayerListTemp != null)
                SaveAdventureModePlayerListTemp.Clear();
            if (PlayerAdventureLevelUpStat != null)
                PlayerAdventureLevelUpStat.Clear();

            SaveAdventureModePlayerListTemp = null;
            PlayerAdventureLevelUpStat = null;
            m_Life = 0;
        }
    }
}
