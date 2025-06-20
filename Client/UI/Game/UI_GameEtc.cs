using GameDefines;
using OptionDefines;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_GameEtc : UIBase
{
    [SerializeField] private Text WaveText;
    [SerializeField] private Text DieWaveText;
    [SerializeField] private Text DieTimeText;
    [SerializeField] private Text LimitText;
    [SerializeField] private Text NoticeText;
    [SerializeField] private Button ExitBtn;
    [SerializeField] private Button RetryBtn;
    [SerializeField] private GameObject DiePanel;
    [SerializeField] private GameObject RankPanel;
    [SerializeField] private Text[] NameText;
    [SerializeField] private Text[] StageText;
    [SerializeField] private Text[] ClearTimeText;
    [SerializeField] private Text MyRankText;
    [SerializeField] private Text MyNameText;
    [SerializeField] private Text MyStageText;
    [SerializeField] private Text MyClearTimeText;
    [SerializeField] private Text RetryText;

    private bool bWaveTextMove = false;
    private float fWaveSpeed = 200f;

    private Camera mainCamera;
    private Vector2 ScreenBounds = Vector2.zero;
    private float WaveTextStartPosX = 0f;
    private float NoticeStartPosY = 0f;

    //pendding
    private struct NoticeData
    {
        public string text;
        public bool sound;
    }

    private bool bPendingNotice = false;
    private List<NoticeData> ReserveNoticeList;

    protected override void Awake()
    {
        mainCamera = Camera.main;
        ScreenBounds = GetComponent<RectTransform>().sizeDelta;
        WaveTextStartPosX = WaveText.rectTransform.anchoredPosition.x;

        if (NoticeText)
            NoticeStartPosY = NoticeText.rectTransform.anchoredPosition.y;

        DiePanel.gameObject.SetActive(false);

        ExitBtn.onClick.AddListener(OnClickExit);
        if (RetryBtn)
        {
            RetryBtn.onClick.AddListener(OnClickRetry);
            RetryBtn.gameObject.SetActive(false);
        }

        UIManager.Instance.OnNoticeEvent += HandleNoticeEvent;
        UIManager.Instance.OnRankEvent += HandleRankEvent;

        ReserveNoticeList = new List<NoticeData>();
    }

    protected override void Update()
    {
        if (bWaveTextMove)
        {
            Vector3 newPosition = WaveText.rectTransform.anchoredPosition;
            newPosition += Vector3.right * fWaveSpeed * Time.deltaTime;
            newPosition.y = WaveText.rectTransform.anchoredPosition.y;

            WaveText.rectTransform.anchoredPosition = newPosition;

            if (WaveText.rectTransform.anchoredPosition.x > ScreenBounds.x)
            {
                WaveText.enabled = false;
                bWaveTextMove = false;
            }
        }

        if (bPendingNotice)
        {
            SetNoticeText();
        }
    }

    protected override void HandleWaveReadyEvent(SpeciesType eSpeciesType)
    {
        SetWaveText();
    }

    protected override void HandleDiePlayerEvent(UIEventArgs e)
    {
        if (e.INT > 0)
        {
            ShowDieAndRetryUI();
        }
        else
        {
            ShowDieUI();
        }
    }

    private void HandleNoticeEvent(string text, bool bSound)
    {
        ReserveNoticeText(text, bSound);
    }

    private void HandleRankEvent()
    {
        ShowRank();
    }

    void SetWaveText()
    {
        int waveIndex = GameManager.Instance.stageIndex;
        WaveText.text = "WAVE " + waveIndex;

        if (WaveText.enabled == false)
            WaveText.enabled = true;

        WaveText.rectTransform.anchoredPosition = new Vector3(WaveTextStartPosX, WaveText.rectTransform.position.x, 0f);
        bWaveTextMove = true;
    }
    private void ShowDieAndRetryUI()
    {
        ShowDieUI();

        if (RetryBtn)
        {
            RetryBtn.gameObject.SetActive(true);
        }

        if (RetryText)
        {
            int remainLife = GameManager.Instance.m_Life;
            RetryText.text = "Final Stage Retry " + remainLife;
        }
    }

    private void ShowDieUI()
    {
        DiePanel.gameObject.SetActive(true);

        int waveIndex = GameManager.Instance.stageIndex;
        DieWaveText.text = "WAVE " + waveIndex;

        uint gameTimeSec = GameManager.Instance.gameTimeSec;
        DieTimeText.text = Oracle.ConvertSplitTime(gameTimeSec, true);

        if (Oracle.RandomDice(0, 2) > 0)
            SoundManager.Instance.PlayUISound(UISoundType.GAMEOVER);
        else
            SoundManager.Instance.PlayUISound(UISoundType.GAMEOVER1);
    }

    private void UpdateLimitTime()
    {
        //if (limitText <= 0)
        //{

        //}

        string limitText = Oracle.ConvertSplitTime(0, true);
        
    }

    private void OnClickExit()
    {
        GameManager.Instance.Exit();
    }

    private void OnClickRetry()
    {
        GameManager.Instance.ReStart(true);
    }

    private void ReserveNoticeText(string text, bool bSound)
    {
        NoticeData notice = new NoticeData();
        notice.text = text;
        notice.sound = bSound;

        ReserveNoticeList.Add(notice);
        if(ReserveNoticeList.Count == 1)
            bPendingNotice = true;
    }
    
    private void SetNoticeText()
    {
        bPendingNotice = false;
        if (NoticeText == null)
            return;

        if (ReserveNoticeList.Count == 0)
            return;

        if (NoticeText.enabled == false)
            NoticeText.enabled = true;

        NoticeText.text = ReserveNoticeList[0].text;
        bool bSound = ReserveNoticeList[0].sound;

        Vector2 cameraCenter = mainCamera.transform.position;
        RectTransform rectTransform = NoticeText.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector3(rectTransform.position.x, NoticeStartPosY, 0f);

        if (bSound)
        {
            SoundManager.Instance.PlayUISound(UISoundType.ACHIEVEMENT);
        }

        rectTransform.DOAnchorPosY(cameraCenter.y, 1f).OnComplete(() =>
        {
            rectTransform.DOAnchorPosY(NoticeStartPosY, 1f).SetDelay(1f).OnComplete(() =>
            {
                NoticeText.enabled = false;

                Player MyPlayer = GameManager.Instance.GetPlayer();
                MyPlayer.isShowNotice = false;
                ReserveNoticeList.RemoveAt(0);
                if (ReserveNoticeList.Count > 0)
                    bPendingNotice = true;
            });
        });
    }

    public void ShowRank()
    {
        if (Oracle.m_eGameType != MapType.SPAWN)
            return;

        if (RankPanel == null)
            return;

        int length = NameText.Length;
        for (int i = 0; i < NameText.Length; ++i)
        {
            RankInfo_Spawn rankData = CSteamLeaderboards.Instance.GetRankInfoSpawn(i);
            if (rankData.score == 0)
                break;

            NameText[i].text = rankData.name;
            StageText[i].text = rankData.score.ToString();
            ClearTimeText[i].text = Oracle.ConvertSplitTime((uint)(rankData.time), true);
            --length;
        }

        if (length != 0)
        {
            RankPanel.gameObject.SetActive(false);
            return;
        }

        SoundManager.Instance.PlayUISound(UISoundType.FANFARE);
        RankPanel.gameObject.SetActive(true);

        RectTransform rectTransform = RankPanel.GetComponent<RectTransform>();
        rectTransform.DOAnchorPosY(800, 1f).SetDelay(3f).OnComplete(() =>
        {
            RankPanel.gameObject.SetActive(false);
        });


    }
}
