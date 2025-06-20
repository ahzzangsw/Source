using GameDefines;
using OptionDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Option : UIBase
{
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Slider SFXSlider;

    [SerializeField] private Button BGMMuteBtn;
    [SerializeField] private Button SFXMuteBtn;
    [SerializeField] private Button FlashEffectBtn;
    [SerializeField] private Button HitEffectBtn;
    [SerializeField] private Button ExitBtn;

    [SerializeField] private Button[] CursorBtnList;

    [SerializeField] private Image BGMMuteIcon;
    [SerializeField] private Image SFXMuteIcon;
    [SerializeField] private Image FlashEffectIcon;
    [SerializeField] private Image HitEffectIcon;

    [SerializeField] private Dropdown ScreenDropdown;
    [SerializeField] private Dropdown ResolutionDropdown;

    protected override void Awake()
    {
        BGMSlider.value = SoundManager.Instance.bgmVolume;
        BGMSlider.onValueChanged.AddListener(OnSliderEvent_BGM);
        SFXSlider.value = SoundManager.Instance.sfxVolume;
        SFXSlider.onValueChanged.AddListener(OnSliderEvent_SFX);

        BGMMuteIcon.enabled = SoundManager.Instance.bBGMMute == false;
        SFXMuteIcon.enabled = SoundManager.Instance.bSFXMute == false;
        FlashEffectIcon.enabled = OptionManager.Instance.bFlashEffect;
        HitEffectIcon.enabled = OptionManager.Instance.bHitEffect;

        BGMMuteBtn.onClick.AddListener(() => OnClickMute(SoundType.BGM));
        SFXMuteBtn.onClick.AddListener(() => OnClickMute(SoundType.SFX));
        FlashEffectBtn.onClick.AddListener(OnClickFlash);
        HitEffectBtn.onClick.AddListener(OnClickHit);

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

        for (int i = 0; i < CursorBtnList.Length; ++i)
        {
            Sprite CursorSprite = ResourceAgent.Instance.GetCursorSprite(i);

            Image[] images = CursorBtnList[i].GetComponentsInChildren<Image>(true);
            for (int j = 0; j < images.Length; ++j)
            {
                if (images[j].gameObject.name == "Icon")
                {
                    if (CursorSprite == null)
                    {
                        images[j].gameObject.SetActive(false);
                    }
                    else
                    {
                        images[j].gameObject.SetActive(true);
                        images[j].sprite = CursorSprite;
                    }
                    break;
                }
            }

            int index = i;
            CursorBtnList[i].onClick.AddListener(() => OnButtonClick_Cursor(index));
            CursorUnlock(index);
        }
        UnlockManager.Instance.OnCursorUnlockEvent += HandleCursorUnlock;

        ExitBtn.onClick.AddListener(OnButtonClick_Exit);
    }

    private void OnSliderEvent_BGM(float fVolume)
    {
        SoundManager.Instance.RefreshVolume(SoundType.BGM, fVolume);
    }
    private void OnSliderEvent_SFX(float fVolume)
    {
        SoundManager.Instance.RefreshVolume(SoundType.SFX, fVolume);
    }
    private void OnClickMute(SoundType eSoundType)
    {
        bool bMute = SoundManager.Instance.MuteVolume(eSoundType);

        if (eSoundType == SoundType.BGM)
            BGMMuteIcon.enabled = !bMute;
        else
            SFXMuteIcon.enabled = !bMute;

        if(bMute == false)
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
    private void OnDropDownEvent_Screen(int index)
    {
        OptionManager.Instance.SetFullScreen(index == 0);
    }
    private void OnDropDownEvent_Resolution(int index)
    {
        OptionManager.Instance.SetResolution(index);
    }
    private void OnButtonClick_Cursor(int index)
    {
        OptionManager.Instance.ChangeCursor(index);
    }
    private void OnButtonClick_Exit()
    {
        SoundManager.Instance.PlayUISound(UISoundType.BACK);

        OptionManager.Instance.SaveOptionData();
        Hide();
    }

    public void CursorUnlock(int index)
    {
        if (index < 0 || index >= CursorBtnList.Length)
            return;

        Sprite CursorSprite = ResourceAgent.Instance.GetCursorSprite(index);
        if (CursorSprite == null)
            return;

        Button currentBtn = CursorBtnList[index];
        Image[] images = currentBtn.GetComponentsInChildren<Image>(true);
        for (int j = 0; j < images.Length; ++j)
        {
            if (images[j].gameObject.name == "Icon")
            {
                images[j].gameObject.SetActive(true);
                images[j].sprite = CursorSprite;
                break;
            }
        }

        MouseOverHandler Rerollhandler = currentBtn.GetComponent<MouseOverHandler>();
        if (Rerollhandler)
        {
            CursorInfo cursorInfo = UnlockManager.Instance.GetCursorInfoList(index);
            Rerollhandler.SetStringTooltip(cursorInfo.tooltip);
        }
    }

    private void HandleCursorUnlock(int index)
    {
        CursorUnlock(index);
    }
}