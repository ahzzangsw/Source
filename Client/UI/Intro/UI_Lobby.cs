using OptionDefines;
using GameDefines;
using UIDefines;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Lobby : UIBase
{
    [SerializeField] private GameObject m_ButtonGroup;
    [SerializeField] private Text m_TextNotice;
    [SerializeField] private Button m_StartButton;
    [SerializeField] private GameObject m_MapSelectComponent;
    [SerializeField] private Button[] m_MapBtnList;
    [SerializeField] private Text m_TextVersion;

    private bool bTextNoticeBlink = false;

    protected override void Awake()
    {
        m_ButtonGroup.SetActive(Oracle.m_bIntro == false);
        m_TextNotice.gameObject.SetActive(Oracle.m_bIntro);
        bTextNoticeBlink = Oracle.m_bIntro;
        if (bTextNoticeBlink)
        {
            StartCoroutine(AlphaPulseRoutineTextNotice());
        }
        else
        {
            SoundManager.Instance.PlayBGM(BGMType.LOBBY);
        }

        Oracle.m_bIntro = false;

        if (m_TextVersion)
        {
            string buildVersion = Application.version;
            m_TextVersion.text = string.Format("VER.{0}", buildVersion);
        }
    }

    protected override void Update()
    {
        if (bTextNoticeBlink)
        {
            if (Input.anyKeyDown)
            {
                bTextNoticeBlink = false;
                m_ButtonGroup.SetActive(true);
                m_TextNotice.gameObject.SetActive(false);
                SoundManager.Instance.PlayUISound(UISoundType.GAMESTART);
                SoundManager.Instance.PlayBGM(BGMType.LOBBY);
            }
        }
    }

    public override void SetControlInfo()
    {
        for (int i = 0; i < m_MapBtnList.Length; ++i)
        {
            int index = i;
            m_MapBtnList[i].onClick.AddListener(() => OnClickMapSelect(index));
        }

        m_StartButton.onClick.AddListener(OnClickGame);

        m_MapSelectComponent.SetActive(false);
    }

    public void OnClickQuit()
    {
        LobbyManager.Instance.Exit();
    }
    private void OnClickGame()
    {
        //OnClickMapSelect(1);
        SoundManager.Instance.PlayUISound(UISoundType.BUTTONCLICK);
        m_MapSelectComponent.SetActive(true);
    }
    public void OnClickUnlock()
    {
        SoundManager.Instance.PlayUISound(UISoundType.BUTTONCLICK);
        SceneManager.LoadScene("ShopScene");
    }
    private void OnClickOption()
    {
        SoundManager.Instance.PlayUISound(UISoundType.BUTTONCLICK);
        m_MapSelectComponent.SetActive(false);
        UIManager.Instance.ShowUI(UIIndexType.OPTION);
    }
    private void OnClickMapSelect(int index)
    {
        SoundManager.Instance.PlayUISound(UISoundType.BUTTONCLICK);
        MapType eMapType = (MapType)index;
        if (eMapType == MapType.MAX)
            return;

        LobbyManager.Instance.StartGame(eMapType);
    }

    IEnumerator AlphaPulseRoutineTextNotice()
    {
        while (true)
        {
            yield return StartCoroutine(ChangeAlpha(0f, 1f, 1f));
            yield return StartCoroutine(ChangeAlpha(1f, 0f, 1f));
        }
    }

    IEnumerator ChangeAlpha(float startAlpha, float targetAlpha, float duration)
    {
        float currentTime = 0.0f;
        Color startColor = m_TextNotice.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float normalizedTime = currentTime / duration;
            m_TextNotice.color = Color.Lerp(startColor, targetColor, normalizedTime);
            yield return null;
        }

        m_TextNotice.color = targetColor;
    }
}
