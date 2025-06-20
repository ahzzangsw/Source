using UIDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameDefines;

public class UIBase : MonoBehaviour
{
    [SerializeField] public UIIndexType eUIIndexType = UIIndexType.NONE;

    protected bool isShow { get; private set; } = false;

    protected virtual void Awake()
    {
        // Do not write code here
    }

    protected virtual void Update()
    {
        // Do not write code here
    }

    public virtual void SetControlInfo()
    {
        UIManager.Instance.OnCharacterUIEvent += HandleCharacterEvent;
        UIManager.Instance.OnWaveReadyUIEvent += HandleWaveReadyEvent;
        UIManager.Instance.OnChangeSpeciesTypeUIEvent += HandleChangeSpeciesTypeEvent;

        Player player = GameManager.Instance.GetPlayer();
        if (player != null)
        {
            player.OnUpdatePlayerEvent += HandleUpdatePlayerEvent;
            player.OnDiePlayerEvent += HandleDiePlayerEvent;
        }
    }

    public virtual void ToggleShow()
    {
        if (isShow)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public virtual void Show()
    {
        PreShow();

        isShow = true;
        gameObject.SetActive(true);
    }
    public virtual void Hide()
    {
        PreHide();

        isShow = false;
        gameObject.SetActive(false);
    }

    protected virtual void PreShow()
    {
        // Do not write code here
    }
    protected virtual void PreHide()
    {
        // Do not write code here
    }

    public UIIndexType GetUIIndexType()
    {
        return eUIIndexType;
    }

    public bool IsShow()
    {
        return isShow;
    }

    protected virtual void HandleWaveReadyEvent(SpeciesType eSpeciesType)
    {
        // Do not write code here
    }

    protected virtual void HandleCharacterEvent(Character pTarget, bool update)
    {
        // Do not write code here
    }

    protected virtual void HandleUpdatePlayerEvent(UIEventArgs e)
    {
        // Do not write code here
    }

    protected virtual void HandleDiePlayerEvent(UIEventArgs e)
    {
        // Do not write code here
    }

    protected virtual void HandleChangeSpeciesTypeEvent(SpeciesType eSpeciesType, AttributeType eAttributeType)
    {
        // Do not write code here
    }
}
