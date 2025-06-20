using UIDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDefines;

public class UI_Tooltip : UIBase
{
    [SerializeField] private GameObject[] m_BackPanel;
    [Header("String")]
    [SerializeField] private Text m_StringText = null;
    [Header("Building")]
    [SerializeField] private Text m_TitleText = null;
    [SerializeField] private Text m_DamageText = null;
    [SerializeField] private Text m_RangeText = null;
    [SerializeField] private Text m_AttackSpeedText = null;
    [SerializeField] private Image m_BuffIcon = null;
    [Header("SpawnBoss")]
    [SerializeField] private Text m_TitleText_b = null;
    [SerializeField] private Text m_HPText_b = null;
    [SerializeField] private Text m_RewardText_b = null;
    [SerializeField] private GameObject m_ColldownComponent_b = null;

    private Camera mainCamera = null;
    private UITooltipType enableTooltipType = UITooltipType.STRING;
    private Vector2[] vPivotList;

    protected override void Awake()
    {
        mainCamera = Camera.main;

        if (m_BackPanel.Length != (int)UITooltipType.MAX)
        {
            Debug.Log("Tooltip Init Data error");
        }

        vPivotList = new Vector2[m_BackPanel.Length];
        vPivotList[(int)UITooltipType.STRING] = m_BackPanel[(int)UITooltipType.STRING].GetComponent<RectTransform>().pivot;
        vPivotList[(int)UITooltipType.BUILDINGINFO] = m_BackPanel[(int)UITooltipType.BUILDINGINFO].GetComponent<RectTransform>().pivot;
        vPivotList[(int)UITooltipType.SPAWNBOSSINFO] = m_BackPanel[(int)UITooltipType.SPAWNBOSSINFO].GetComponent<RectTransform>().pivot;
    }

    protected override void Update()
    {
        if (enableTooltipType == UITooltipType.MAX)
            return;

        RectTransform BackPanelRect = m_BackPanel[(int)enableTooltipType].GetComponent<RectTransform>();
        if (BackPanelRect == null)
            return;

        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        if (enableTooltipType == UITooltipType.BUILDINGINFO)
            mousePosition.y += 1.5f;

        BackPanelRect.transform.position = mousePosition;

        // Change direction based on camera position
        //Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(mainCamera, BackPanelRect.position);
        //if (screenPos.x < 0 || screenPos.x > Screen.width ||
        //    screenPos.y < 0 || screenPos.y > Screen.height)
        //{
        //    Debug.Log("UI 요소가 화면 밖으로 나갔습니다.");
        //}
    }

    public override void SetControlInfo()
    {
        UIManager.Instance.OnUpdateTooltipEvent += HandleUpdateTooltipEvent;
    }

    protected override void PreShow()
    {
        if (enableTooltipType == UITooltipType.MAX)
            return;

        m_BackPanel[(int)enableTooltipType].gameObject.SetActive(true);
    }

    protected override void PreHide()
    {
        if (enableTooltipType == UITooltipType.MAX)
            return;

        RectTransform BackPanelRect = m_BackPanel[(int)enableTooltipType].GetComponent<RectTransform>();
        if (BackPanelRect == null)
            return;

        if (vPivotList != null)
        {
            BackPanelRect.pivot = vPivotList[(int)enableTooltipType];
        }
        enableTooltipType = UITooltipType.MAX;
    }

    public void SetStringTooltip(string strText, Vector2 vPivot)
    {
        m_BackPanel[(int)UITooltipType.BUILDINGINFO].gameObject.SetActive(false);
        m_BackPanel[(int)UITooltipType.SPAWNBOSSINFO].gameObject.SetActive(false);

        enableTooltipType = UITooltipType.STRING;
        m_StringText.text = strText;
        
        float fWidth = m_StringText.preferredWidth;
        float fHeight = m_StringText.preferredHeight;

        int ifontsize = m_StringText.fontSize;

        for (int i = 1; ; ++i)
        {
            if (ifontsize * i >= fHeight)
            {
                m_StringText.rectTransform.pivot = new Vector2(-0.05f, (float)(i - 1) * -0.5f);
                break;
            }
        }

        RectTransform BackPanelRect = m_BackPanel[(int)enableTooltipType].GetComponent<RectTransform>();
        if (BackPanelRect == null)
            return;

        BackPanelRect.sizeDelta = new Vector2(fWidth + 7f, fHeight + 1f);
        if (vPivot.x != 999)
        {
            BackPanelRect.pivot = vPivot;
        }
    }

    public void SetBuildingInfoTooltip(int index)
    {
        m_BackPanel[(int)UITooltipType.STRING].gameObject.SetActive(false);
        m_BackPanel[(int)UITooltipType.SPAWNBOSSINFO].gameObject.SetActive(false);

        enableTooltipType = UITooltipType.BUILDINGINFO;

        Player currentPlayer = GameManager.Instance.GetPlayer();
        BuildingInfo buildingInfo = ResourceAgent.Instance.GetBuildingInfo(currentPlayer.m_eSpeciesType, index);
        m_TitleText.text = Oracle.GetCharacterName(currentPlayer.m_eSpeciesType, index, true);
        m_DamageText.text = Oracle.ConvertNumberDigit(buildingInfo.Damage);
        m_RangeText.text = buildingInfo.Range.ToString("F1");
        m_AttackSpeedText.text = buildingInfo.AttackSpeed.ToString("F1");
    }

    public void SetSpawnBossInfoTooltip(int index)
    {
        m_BackPanel[(int)UITooltipType.STRING].gameObject.SetActive(false);
        m_BackPanel[(int)UITooltipType.BUILDINGINFO].gameObject.SetActive(false);

        enableTooltipType = UITooltipType.SPAWNBOSSINFO;

        ControlInfo controlInfo = ResourceAgent.Instance.GetControlInfoData();
        m_TitleText_b.text = string.Format("BOSS LEVEL {0}", index+1);
        m_HPText_b.text = Oracle.ConvertNumberDigit<int>(controlInfo.BossHPList[index], true);
        m_RewardText_b.text = "+" + Oracle.ConvertNumberDigit<int>(controlInfo.BossMoneyEarnedList[index]);
        //BossDefenseList[index]
        //BossSpeedList[index]
        //m_ColldownComponent_b.SetActive(bCooldown);
    }

    private void HandleUpdateTooltipEvent(UIEventArgs e)
    {
        if (e.STRING != string.Empty)
        {
            SetStringTooltip(e.STRING, e.VECTOR2D);
        }
        else if (e.key == (int)UITooltipType.BUILDINGINFO)
        {
            SetBuildingInfoTooltip(e.INT);
        }
        else if(e.key == (int)UITooltipType.SPAWNBOSSINFO)
        {
            SetSpawnBossInfoTooltip(e.INT);
        }
        else
        {
            Hide();
            return;
        }

        Show();
    }
}
