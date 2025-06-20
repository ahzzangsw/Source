using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using OptionDefines;
using GameDefines;
using UIDefines;

public class UI_ExitMenu : UIBase
{
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Slider CameraSlider;

    [SerializeField] private Button BGMMuteBtn;
    [SerializeField] private Button SFXMuteBtn;
    [SerializeField] private Button FlashEffectBtn;
    [SerializeField] private Button HitEffectBtn;
    [SerializeField] private Button HorizontalBtn;
    [SerializeField] private Button VerticalBtn;

    [SerializeField] private Image BGMMuteIcon;
    [SerializeField] private Image SFXMuteIcon;
    [SerializeField] private Image FlashEffectIcon;
    [SerializeField] private Image HitEffectIcon;
    [SerializeField] private Image HorizontalIcon;
    [SerializeField] private Image VerticalIcon;

    [SerializeField] private Dropdown ScreenDropdown;
    [SerializeField] private Dropdown ResolutionDropdown;

    [SerializeField] private Button RetryBtn;
    [SerializeField] private Button ExitBtn;
    [SerializeField] private Button PauseBtn;
    [SerializeField] private Button UIExitBtn;

    private Vector3 originPauseBtnSize = Vector3.zero;
    private Button CurrentOverButton = null;
    private UITooltipSortType m_eUITooltipSortType = UITooltipSortType.HORIZONTAL;

    protected override void Awake()
    {
        PauseBtn.onClick.AddListener(OnClick_Pause);
        UIExitBtn.onClick.AddListener(OnClick_Pause);
        MouseOverHandler Rerollhandler = PauseBtn.GetComponent<MouseOverHandler>();
        Rerollhandler.ButtonMouseOver += HandleButtonOverEnter;
        Rerollhandler.ButtonMouseOut += HandleButtonOverExit;
        originPauseBtnSize = PauseBtn.transform.localScale;

        BGMSlider.value = SoundManager.Instance.bgmVolume;
        BGMSlider.onValueChanged.AddListener(OnSliderEvent_BGM);
        SFXSlider.value = SoundManager.Instance.sfxVolume;
        SFXSlider.onValueChanged.AddListener(OnSliderEvent_SFX);
        CameraSlider.value = OptionManager.Instance.fCameraSpeed;
        CameraSlider.onValueChanged.AddListener(OnSliderEvent_CameraSpeed);

        BGMMuteIcon.enabled = SoundManager.Instance.bBGMMute == false;
        SFXMuteIcon.enabled = SoundManager.Instance.bSFXMute == false;
        FlashEffectIcon.enabled = OptionManager.Instance.bFlashEffect;
        HitEffectIcon.enabled = OptionManager.Instance.bHitEffect;
        HorizontalIcon.enabled = OptionManager.Instance.iTooltipSortType == 0;
        VerticalIcon.enabled = OptionManager.Instance.iTooltipSortType == 1;
        m_eUITooltipSortType = (UITooltipSortType)(OptionManager.Instance.iTooltipSortType);

        BGMMuteBtn.onClick.AddListener(() => OnClickMute(SoundType.BGM));
        SFXMuteBtn.onClick.AddListener(() => OnClickMute(SoundType.SFX));
        FlashEffectBtn.onClick.AddListener(OnClickFlash);
        HitEffectBtn.onClick.AddListener(OnClickHit);
        HorizontalBtn.onClick.AddListener(() => OnClickTooltipSortType(UITooltipSortType.HORIZONTAL));
        VerticalBtn.onClick.AddListener(() => OnClickTooltipSortType(UITooltipSortType.VERTICAL));

        ScreenDropdown.options.Clear();
        ScreenDropdown.options.Add(new Dropdown.OptionData("Full Screen"));
        ScreenDropdown.options.Add(new Dropdown.OptionData("Windowed"));
        ScreenDropdown.onValueChanged.AddListener(OnDropDownEvent_Screen);
        ScreenDropdown.value = OptionManager.Instance.bFullScreen ? 0 : 1;

        ResolutionDropdown.options.Clear();
        foreach (Vector2 vResolution in OptionManager.Instance.vResolutionList)
        {
            string strResolution = string.Format("{0} x {1}", (int)(vResolution.x), (int)(vResolution.y));
            ResolutionDropdown.options.Add(new Dropdown.OptionData(strResolution));
        }
        ResolutionDropdown.onValueChanged.AddListener(OnDropDownEvent_Resolution);
        ResolutionDropdown.value = OptionManager.Instance.iResolutionIndex;
    }
    
    protected override void PreShow()
    {
        Player player = GameManager.Instance.GetPlayer();
        if (player)
            player.isUIMouseOver = true;
    }
    protected override void PreHide()
    {
        OptionManager.Instance.SaveOptionData();

        if (PauseBtn == CurrentOverButton)
        {
            if (PauseBtn.enabled)
                PauseBtn.transform.localScale = originPauseBtnSize;
        }

        CurrentOverButton = null;

        Player player = GameManager.Instance.GetPlayer();
        if (player)
            player.isUIMouseOver = false;
    }

    private void OnClick_Pause()
    {
        GameManager.Instance.SetPause();
    }
    private void HandleButtonOverEnter(object sender, GameObject button)
    {
        CurrentOverButton = button.GetComponent<Button>();
        if (PauseBtn == CurrentOverButton)
        {
            if (PauseBtn.enabled)
                PauseBtn.transform.localScale *= 1.1f;
        }
    }
    private void HandleButtonOverExit(object sender, GameObject button)
    {
        if (PauseBtn == CurrentOverButton)
        {
            if (PauseBtn.enabled)
                PauseBtn.transform.localScale = originPauseBtnSize;
        }

        CurrentOverButton = null;
    }

    private void OnSliderEvent_BGM(float fVolume)
    {
        SoundManager.Instance.RefreshVolume(SoundType.BGM, fVolume);
    }
    private void OnSliderEvent_SFX(float fVolume)
    {
        SoundManager.Instance.RefreshVolume(SoundType.SFX, fVolume);
    }
    private void OnSliderEvent_CameraSpeed(float fSpeed)
    {
        OptionManager.Instance.RefreshCameraSpeed(fSpeed);
    }

    private void OnClickMute(SoundType eSoundType)
    {
        bool bMute = SoundManager.Instance.MuteVolume(eSoundType);

        if (eSoundType == SoundType.BGM)
            BGMMuteIcon.enabled = !bMute;
        else
            SFXMuteIcon.enabled = !bMute;

        if (bMute == false)
            SoundManager.Instance.PlayUISound(UISoundType.EQUIP);
    }
    private void OnClickFlash()
    {
        FlashEffectIcon.enabled = OptionManager.Instance.ChangeEffect(false);
        if (FlashEffectIcon.enabled)
            SoundManager.Instance.PlayUISound(UISoundType.EQUIP);
    }
    private void OnClickHit()
    {
        HitEffectIcon.enabled = OptionManager.Instance.ChangeEffect(true);
        if (HitEffectIcon.enabled)
            SoundManager.Instance.PlayUISound(UISoundType.EQUIP);
    }
    private void OnClickTooltipSortType(UITooltipSortType eUITooltipSortType)
    {
        if (m_eUITooltipSortType == eUITooltipSortType)
            return;

        m_eUITooltipSortType = eUITooltipSortType;
        HorizontalIcon.enabled = eUITooltipSortType == UITooltipSortType.HORIZONTAL;
        VerticalIcon.enabled = eUITooltipSortType == UITooltipSortType.VERTICAL;
        OptionManager.Instance.ChangeTooltipSortType(eUITooltipSortType);
    }

    private void OnDropDownEvent_Screen(int index)
    {
        OptionManager.Instance.SetFullScreen(index == 0);
    }
    private void OnDropDownEvent_Resolution(int index)
    {
        OptionManager.Instance.SetResolution(index);
    }

    public void OnClickRetry()
    {
        ShowDialog(false, "Retry?");
    }

    public void OnClickExit()
    {
        ShowDialog(true, "Stop?");
    }

    private void ShowDialog(bool bExit, string text)
    {
        GameObject popupObject = UIManager.Instance.GetPopup(transform);
        if (popupObject == null)
            return;

        UI_Popup popupInfo = popupObject.GetComponent<UI_Popup>();
        if (popupInfo == null)
            return;

        popupInfo.SetText(text);

        UIEventArgs args = new UIEventArgs();
        if (bExit)
            popupInfo.OnPopUpOK += HandleExit;
        else
            popupInfo.OnPopUpOK += HandleRetry;

        GameManager.Instance.GameOver();
    }

    private void HandleRetry(UIEventArgs args)
    {
        GameManager.Instance.ReStart(false);
    }
    private void HandleExit(UIEventArgs args)
    {
        GameManager.Instance.Exit();
    }
}
