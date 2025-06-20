using GameDefines;
using UIDefines;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OptionDefines;
using TMPro;

public class UI_GameInfo : UIBase
{
    [SerializeField] private TextMeshProUGUI m_TimeText = null;
    [SerializeField] private TextMeshProUGUI m_MoneyText = null;
    [SerializeField] private TextMeshProUGUI m_HpText = null;
    [SerializeField] private Button m_Pause = null;
    [SerializeField] private Button m_SpeedX = null;
    [SerializeField] private Button m_Help = null;
    [SerializeField] private Button[] m_CameraMove = null;

    [SerializeField] private Sprite[] m_SpeedXSpriteArray = null;

    private Vector3 originPauseBtnSize = Vector3.zero;
    private Button CurrentOverButton = null;

    private Camera mainCamera = null;

    void LateUpdate()
    {
        UpdatePlayTime();
    }
    public override void SetControlInfo()
    {
        base.SetControlInfo();

        if (m_Pause != null)
        {
            m_Pause.onClick.AddListener(OnClick_Pause);
            MouseOverHandler Rerollhandler = m_Pause.GetComponent<MouseOverHandler>();
            Rerollhandler.ButtonMouseOver += HandleButtonOverEnter;
            Rerollhandler.ButtonMouseOut += HandleButtonOverExit;
            originPauseBtnSize = m_Pause.transform.localScale;
        }

        if (m_SpeedX != null)
        {
            m_SpeedX.onClick.AddListener(OnClick_Speed);
        }

        if (m_Help != null)
        {
            m_Help.onClick.AddListener(OnClick_Help);
        }

        for (int i = 0; i < m_CameraMove.Length; ++i)
        {
            if (m_CameraMove == null)
                continue;

            bool bMove = i != 0;
            m_CameraMove[i].onClick.AddListener(() => OnClick_Camera(bMove));
        }

        mainCamera = Camera.main;
        CameraManager.Instance.OnCameraWalkingEvent += ChangeCameramoveIcon;
    }
    private void OnClick_Pause()
    {
        GameManager.Instance.SetPause();
    }
    private void OnClick_Speed()
    {
        GameManager.Instance.ChangeGameSpeed();
    }
    private void OnClick_Help()
    {

    }

    public void UpdateGameSpeed(OtherShopProductItemType eOtherShopProductItemType, bool bActive)
    {
        if (m_SpeedXSpriteArray.Length < 3)
            return;

        if (m_SpeedX.gameObject.activeSelf != bActive)
        {
            m_SpeedX.gameObject.SetActive(bActive);
        }

        int index = 0;
        switch (eOtherShopProductItemType)
        {
            case OtherShopProductItemType.GAMESPEED2X:
                index = 1;
                break;
            case OtherShopProductItemType.GAMESPEED3X:
                index = 2;
                break;
        }

        m_SpeedX.image.sprite = m_SpeedXSpriteArray[index];
    }

    private void OnClick_Camera(bool bMove)
    {
        CameraManager.Instance.cameramove = bMove;
        ChangeCameramoveIcon(bMove);
    }
    private void ChangeCameramoveIcon(bool bMove)
    {
        m_CameraMove[0].gameObject.SetActive(bMove);
        m_CameraMove[1].gameObject.SetActive(!bMove);
    }
    private void HandleButtonOverEnter(object sender, GameObject button)
    {
        CurrentOverButton = button.GetComponent<Button>();
        if (m_Pause == CurrentOverButton)
        {
            if (m_Pause.enabled)
                m_Pause.transform.localScale *= 1.1f;
        }
    }
    private void HandleButtonOverExit(object sender, GameObject button)
    {
        if (m_Pause == CurrentOverButton)
        {
            if (m_Pause.enabled)
                m_Pause.transform.localScale = originPauseBtnSize;
        }

        CurrentOverButton = null;
    }

    protected override void HandleUpdatePlayerEvent(UIEventArgs e)
    {
        UpdateMoney();
        UpdateHP();
    }

    private void UpdatePlayTime()
    {
        if (m_TimeText == null)
            return;

        if (GameManager.Instance == null)
            return;

        uint gameTimeSec = GameManager.Instance.gameTimeSec;
        m_TimeText.text = Oracle.ConvertSplitTime(gameTimeSec, true);
    }

    private void UpdateMoney()
    {
        if (m_MoneyText == null)
            return;

        int playerMoney = GameManager.Instance.GetPlayer().GetMoney();
        m_MoneyText.text = playerMoney.ToString();
    }

    private void UpdateHP()
    {
        if (m_HpText == null)
            return;

        int playerHp = GameManager.Instance.GetPlayer().GetHp();
        m_HpText.text = playerHp.ToString();
    }
}
