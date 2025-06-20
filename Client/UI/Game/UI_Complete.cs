using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UIDefines;

public class UI_Complete : UIBase
{
    [SerializeField] private Image m_ModalImage;
    [SerializeField] private RectTransform m_Title;
    [SerializeField] private Text CompleteTimeText;
    [SerializeField] private Text m_Text;
    [SerializeField] private TextMeshProUGUI m_QuestionmarkText;
    //[SerializeField] private Button m_ContinueBtn;
    [SerializeField] private Button m_ExitBtn;

    private bool m_bEnable = false;
    private bool m_bBossDrop = false;
    private enum CompleteUIState { NONE, TITLE, TEXT, BUTTON, TITLEDOWN, MAX, NOTDIRECTING, };
    private CompleteUIState m_eState = CompleteUIState.NONE;

    private float m_ModalImageAlpha = 0f;
    private float m_AlphaValue = 0.001f;
    private float TitlePositionYValue = 7f;
    private float m_TextAlpha = 0f;

    public event Action OnEventTitleDown;

    protected override void Awake()
    {
        //m_ContinueBtn.onClick.AddListener(OnClickContinue);
        //m_ContinueBtn.gameObject.SetActive(false);
        m_ExitBtn.onClick.AddListener(OnClickExit);
        m_ExitBtn.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        OnEventTitleDown -= OnEventTitleDown;
    }

    protected override void PreShow()
    {
        if (Oracle.m_eGameType == GameDefines.MapType.BUILD)
        {
            m_eState = CompleteUIState.NONE;
            m_ModalImageAlpha = 0f;
            m_TextAlpha = 0f;
        }
        else if (Oracle.m_eGameType == GameDefines.MapType.SPAWN)
        {
            m_eState = CompleteUIState.TITLEDOWN;
        }
        else
        {
            m_eState = CompleteUIState.NOTDIRECTING;
            CompleteTimeText.gameObject.SetActive(false);
        }

        m_bEnable = true;
    }

    protected override void Update()
    {
        if (m_bEnable == false)
            return;

        switch (m_eState)
        { 
            case CompleteUIState.NONE:
                {
                    if (m_ModalImageAlpha >= 1f)
                    {
                        break;
                    }

                    Color currentColor = m_ModalImage.color;
                    currentColor.a = m_ModalImageAlpha;
                    m_ModalImage.color = currentColor;
                    m_ModalImageAlpha += m_AlphaValue;
                }
                break;
            case CompleteUIState.TITLE:
                {
                    Vector2 TitlePosition = m_Title.anchoredPosition;
                    TitlePosition.y -= TitlePositionYValue;
                    m_Title.anchoredPosition = TitlePosition;

                    if (!m_bBossDrop && TitlePosition.y <= 110)
                    {
                        OnEventTitleDown?.Invoke();
                        m_bBossDrop = true;
                    }

                    if (TitlePosition.y <= 87)
                    {
                        NextStep();
                        break;
                    }
                }
                break;
            case CompleteUIState.TEXT:
                {
                    if (m_TextAlpha >= 1f)
                    {
                        NextStep();
                        break;
                    }

                    Color currentColor = m_Text.color;
                    currentColor.a = m_TextAlpha;
                    m_Text.color = currentColor;
                    m_TextAlpha += m_AlphaValue;
                }
                break;
            case CompleteUIState.BUTTON:
                {
                    m_bEnable = false;

                    uint gameTimeSec = GameManager.Instance.gameTimeSec;
                    CompleteTimeText.text = "Complete Time  " + Oracle.ConvertSplitTime(gameTimeSec, true);

                    CompleteTimeText.gameObject.SetActive(true);
                    //m_ContinueBtn.gameObject.SetActive(true);
                    m_ExitBtn.gameObject.SetActive(true);
                }
                break;
            case CompleteUIState.TITLEDOWN:
                {
                    Vector2 TitlePosition = m_Title.anchoredPosition;
                    TitlePosition.y -= TitlePositionYValue;
                    m_Title.anchoredPosition = TitlePosition;

                    if (TitlePosition.y < -1000)
                    {
                        m_bEnable = false;
                        UIManager.Instance.HideUI(UIIndexType.COMPLETE);
                    }
                }
                break;
            case CompleteUIState.NOTDIRECTING:
                {
                    if (m_ModalImageAlpha >= 1f)
                    {
                        uint gameTimeSec = GameManager.Instance.gameTimeSec;
                        CompleteTimeText.text = "Complete Time  " + Oracle.ConvertSplitTime(gameTimeSec, true);
                        CompleteTimeText.gameObject.SetActive(true);
                        m_ExitBtn.gameObject.SetActive(true);
                        m_bEnable = false;
                        break;
                    }

                    Color currentColor = m_ModalImage.color;
                    currentColor.a = m_ModalImageAlpha;
                    m_ModalImage.color = currentColor;
                    m_ModalImageAlpha += m_AlphaValue;
                }
                break;
        }
    }

    private void OnClickContinue()
    {
        UIManager.Instance.HideUI(UIIndexType.COMPLETE);
        GameManager.Instance.GamLoopStart();
    }

    private void OnClickExit()
    {
        GameManager.Instance.Exit();
    }

    public void NextStep()
    {
        if (m_eState == CompleteUIState.MAX)
            return;

        m_eState = (CompleteUIState)((int)m_eState + 1);
    }

    public void EnableQuestionmark(bool bShow)
    {
        m_QuestionmarkText.gameObject.SetActive(bShow);
    }
}
