using GameDefines;
using UIDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_TargetDataViewer : UIBase
{
    [SerializeField] private UI_AttackRangeVisualization attackRangeVisualizationClass;
    [SerializeField] private Image[] m_BuffIcon = null;         ///< 버프 아이콘
    [SerializeField] private Text[] m_BText0 = null;            ///< 공격력
    [SerializeField] private Text[] m_BText1 = null;            ///< 사정거리
    [SerializeField] private Text[] m_BText2 = null;            ///< 공격속도
    [SerializeField] private Text[] m_BText3 = null;            ///< 종족 - 빌딩
    [SerializeField] private Text[] m_MText0 = null;            ///< HP
    [SerializeField] private Text[] m_MText1 = null;            ///< 방어력
    [SerializeField] private Text[] m_MText2 = null;            ///< 이동속도
    [SerializeField] private Text[] m_MText3 = null;            ///< 종족 - 몬스터
    [SerializeField] private Button[] m_SellBtn = null;         ///< 판매 버튼
    [SerializeField] private Button m_SpawnBtn = null;          ///< 스폰 버튼
    [SerializeField] private Button m_UpdateBtn = null;         ///< 업그레이드 버튼

    [Header("#Control")]
    [SerializeField] private GameObject[] m_TargetBuildingCanvas;
    [SerializeField] private GameObject[] m_TargetMonsterCanvas;
    [SerializeField] private GameObject m_TargetSpawnCanvas;
    [SerializeField] private GameObject m_TargetUpgradeCanvas;
    [SerializeField] private Image[] m_TargetBuildArrow;
    [SerializeField] private Image[] m_TargetMonsterArrow;

    private Character m_pTarget = null;
    private SpawnSphere m_pSpawnTarget = null;
    private RectTransform panelRectTransform = null;
    private RectTransform panelRectTransform2 = null;

    [SerializeField] private float YPos = 0f;
    private UITooltipSortType eUITooltipSortType = UITooltipSortType.HORIZONTAL;

    public int minFontSize = 10;
    public int maxFontSize = 25;

    protected override void Awake()
    {
        for (int i = 0; i < m_MText0.Length; ++i)
        {
            m_MText0[i].resizeTextForBestFit = true;
            m_MText0[i].resizeTextMinSize = 23;
            m_MText0[i].resizeTextMaxSize = 25;
        }

        eUITooltipSortType = (UITooltipSortType)(OptionManager.Instance.iTooltipSortType);
        UIManager.Instance.OnChangeTooltipSortType += HandleChangeTooltipSortType;
    }
    protected override void HandleCharacterEvent(Character pTarget, bool update)
    {
        if (update)
        {
            if (m_pTarget == pTarget)
            {
                SetTargetInfo(m_pTarget);
            }
        }
        else
        {
            SetTargetInfo(pTarget);
        }
    }

    private void HandleChangeTooltipSortType()
    {
        if (m_TargetBuildingCanvas == null || m_TargetMonsterCanvas == null || m_TargetSpawnCanvas == null || m_TargetUpgradeCanvas == null)
            return;

        m_TargetBuildingCanvas[(int)eUITooltipSortType].SetActive(false);
        m_TargetMonsterCanvas[(int)eUITooltipSortType].SetActive(false);
        m_TargetSpawnCanvas.SetActive(false);
        m_TargetUpgradeCanvas.SetActive(false);

        eUITooltipSortType = (UITooltipSortType)(OptionManager.Instance.iTooltipSortType);
    }

    private void OnClick_Sell()
    {
        if (Oracle.m_eGameType != MapType.BUILD)
            return;

        if (m_pTarget == null)
            return;

        if (m_pTarget.m_eClickTargetType == ClickTargetType.BUILDING)
        {
            Player MyPlayer = GameManager.Instance.GetPlayer();
            if (MyPlayer == null)
                return;

            MyPlayer.Refund(m_pTarget as Building);
            Hide();
        }
    }

    private void OnClick_Spawn()
    {
        if (m_pSpawnTarget == null)
            return;

        if (CheckConditions(m_pSpawnTarget.Cost))
        {
            BuildingPool.Instance.SpawnSpawn(m_pSpawnTarget);
            UIManager.Instance.HideUI(UIIndexType.TOOLTIP);
            Hide();
        }
    }

    private void OnClick_Upgrade()
    {
        if (m_TargetUpgradeCanvas == null)
            return;

        if (m_TargetUpgradeCanvas.activeSelf == false)
            return;

        if (m_pTarget == null)
            return;

        Building pBuilding = m_pTarget as Building;
        if (pBuilding == null)
            return;

        if (BuildingPool.Instance.CheckUpgrade(pBuilding))
        {
            BuildingPool.Instance.UpgradeBuilding(pBuilding);
            UIManager.Instance.HideUI(UIIndexType.TOOLTIP);
            Hide();
        }
    }

    public override void SetControlInfo()
    {
        base.SetControlInfo();

        for (int i = 0; i < m_SellBtn.Length; ++i)
        {
            if (m_SellBtn[i])
            {
                MouseOverHandler Spawnhandler = m_SellBtn[i].GetComponent<MouseOverHandler>();
                Spawnhandler.ButtonMouseOver += HandleButtonOverEnter;
                Spawnhandler.ButtonMouseOut += HandleButtonOverExit;

                m_SellBtn[i].onClick.AddListener(OnClick_Sell);
            }
        }

        if (m_SpawnBtn)
        {
            MouseOverHandler Spawnhandler = m_SpawnBtn.GetComponent<MouseOverHandler>();
            Spawnhandler.ButtonMouseOver += HandleButtonOverEnter;
            Spawnhandler.ButtonMouseOut += HandleButtonOverExit;
            m_SpawnBtn.onClick.AddListener(OnClick_Spawn);
        }

        if (m_UpdateBtn)
        {
            MouseOverHandler Spawnhandler = m_UpdateBtn.GetComponent<MouseOverHandler>();
            Spawnhandler.ButtonMouseOver += HandleButtonOverEnter;
            Spawnhandler.ButtonMouseOut += HandleButtonOverExit;
            m_UpdateBtn.onClick.AddListener(OnClick_Upgrade);
        }
    }

    private void HandleButtonOverEnter(object sender, GameObject button) {}
    private void HandleButtonOverExit(object sender, GameObject button) {}

    private void SetTargetInfo(Character target)
    {
        // Awake보다 Event가 먼저 호출되는 경우가 있음
        if (eUITooltipSortType != (UITooltipSortType)(OptionManager.Instance.iTooltipSortType))
            eUITooltipSortType = (UITooltipSortType)(OptionManager.Instance.iTooltipSortType);

        m_pSpawnTarget = null;
        m_pTarget = target;
        if (m_pTarget.m_eClickTargetType == ClickTargetType.BUILDING || m_pTarget.m_eClickTargetType == ClickTargetType.PLAYER)
        {
            SetBuildInfo();
        }
        else if(m_pTarget.m_eClickTargetType == ClickTargetType.MONSTER)
        {
            SetMonsterInfo();
        }
        else
        {

        }
    }

    protected override void PreHide()
    {
        m_pTarget = null;
        m_pSpawnTarget = null;
        attackRangeVisualizationClass.Hide();

        if (m_TargetBuildingCanvas != null && m_TargetBuildingCanvas.Length > 0)
            m_TargetBuildingCanvas[(int)eUITooltipSortType].SetActive(false);

        if (m_TargetMonsterCanvas != null && m_TargetMonsterCanvas.Length > 0)
            m_TargetMonsterCanvas[(int)eUITooltipSortType].SetActive(false);

        if (m_TargetSpawnCanvas)
            m_TargetSpawnCanvas.SetActive(false);

        if(m_TargetUpgradeCanvas)
            m_TargetUpgradeCanvas.SetActive(false);
        
        panelRectTransform = null;
        panelRectTransform2 = null;
    }

    protected override void Update()
    {
        if (Input.GetKeyUp(KeyCode.C))
        {
            OnClick_Spawn();
            OnClick_Upgrade();
        }
        else if (Input.GetKeyUp(KeyCode.X))
        {
            OnClick_Sell();
        }

        if (panelRectTransform)
        {
            Vector3 targetPosition = Vector3.back;
            if (m_pTarget)
            {
                targetPosition = m_pTarget.transform.position;
            }
            else if (m_pSpawnTarget)
            {
                targetPosition = m_pSpawnTarget.transform.position;
            }

            if (targetPosition == Vector3.back)
                return;

            targetPosition.y += YPos;
            panelRectTransform.position = targetPosition;
        }

        if (panelRectTransform2 && m_pTarget)
        {
            Vector3 targetPosition = m_pTarget.transform.position;
            targetPosition.y += 1.5f;
            panelRectTransform2.position = targetPosition;
        }
    }

    void SetBuildInfo()
    {
        Building pBuilding = m_pTarget as Building;
        if (pBuilding == null)
            return;

        int addDamage = 0;
        Player MyPlayer = GameManager.Instance.GetPlayer();
        if (MyPlayer != null)
        {
            ControlInfo controlInfoData = MyPlayer.GetCurrentUpgradeData(pBuilding.m_eSpeciesType);
            addDamage = controlInfoData.UnitAtk[pBuilding.m_CharacterIndex];
        }

        string strDamage;
        if (addDamage > 0)
            strDamage = string.Format("{0}<color=green>+{1}</color>", Oracle.ConvertNumberDigit(pBuilding.Damage), Oracle.ConvertNumberDigit(addDamage));
        else
            strDamage = Oracle.ConvertNumberDigit(pBuilding.Damage);

        m_BText0[(int)eUITooltipSortType].text = strDamage;
        m_BText1[(int)eUITooltipSortType].text = pBuilding.Range.ToString("F1");
        m_BText2[(int)eUITooltipSortType].text = pBuilding.AttackSpeed.ToString("F1");
        m_BText3[(int)eUITooltipSortType].text = Oracle.GetSpeciesTypeString(pBuilding.m_eSpeciesType);

        //m_BuffIcon[(int)eUITooltipSortType].sprite = UIManager.Instance.GetBuffSprite(pBuilding.eBuffType);

        if (pBuilding.m_eSpeciesType == SpeciesType.GOBLIN && pBuilding.m_CharacterIndex == 4)
        {
            attackRangeVisualizationClass.Hide();
        }
        else
        {
            if (Oracle.m_eGameType == MapType.ADVENTURE)
                attackRangeVisualizationClass.OnVisualization(m_pTarget, pBuilding.Range);
            else
                attackRangeVisualizationClass.OnVisualization(m_pTarget.transform.position + new Vector3(0f, 0.5f, 0), pBuilding.Range);
            attackRangeVisualizationClass.Show();
        }

        panelRectTransform = m_TargetBuildingCanvas[(int)eUITooltipSortType].GetComponent<RectTransform>();
        YPos = -0.6f;
        if (Oracle.m_eGameType == MapType.ADVENTURE)
        {
            if (m_pTarget.transform.position.y < -6)
            {
                YPos = 4.4f - (1.1f - m_pTarget.transform.localScale.y);

                m_TargetBuildArrow[0].gameObject.SetActive(false);
                m_TargetBuildArrow[1].gameObject.SetActive(true);
            }
            else
            {
                m_TargetBuildArrow[0].gameObject.SetActive(true);
                m_TargetBuildArrow[1].gameObject.SetActive(false);
            }
        }

        m_TargetBuildingCanvas[(int)eUITooltipSortType].SetActive(true);
        m_TargetMonsterCanvas[(int)eUITooltipSortType].SetActive(false);
        
        if (m_TargetSpawnCanvas)
            m_TargetSpawnCanvas.SetActive(false);

        if (m_TargetUpgradeCanvas)
        {
            if (BuildingPool.Instance.CheckUpgrade(pBuilding))
            {
                panelRectTransform2 = m_TargetUpgradeCanvas.GetComponent<RectTransform>();
                m_TargetUpgradeCanvas.SetActive(true);
            }
            else
            {
                panelRectTransform2 = null;
                m_TargetUpgradeCanvas.SetActive(false);
            }
        }
        else
            panelRectTransform2 = null;

        AdjustFontSize();
    }

    void SetMonsterInfo()
    {
        MonsterBase pMonster = m_pTarget as MonsterBase;
        if (pMonster == null)
            return;

        m_MText0[(int)eUITooltipSortType].text = string.Format("{0}/{1}", pMonster.currentHP, pMonster.Hp);
        m_MText1[(int)eUITooltipSortType].text = pMonster.GetDefenseString();
        m_MText2[(int)eUITooltipSortType].text = pMonster.GetMoveSpeedString();
        m_MText3[(int)eUITooltipSortType].text = Oracle.GetSpeciesTypeString(pMonster.m_eSpeciesType);

        attackRangeVisualizationClass.Hide();

        panelRectTransform = m_TargetMonsterCanvas[(int)eUITooltipSortType].GetComponent<RectTransform>();
        YPos = -0.6f;
        if (Oracle.m_eGameType == MapType.ADVENTURE)
        {
            if (m_pTarget.transform.position.y < -6)
            {
                YPos = 4.4f - (1.1f - m_pTarget.transform.localScale.y);

                m_TargetMonsterArrow[0].gameObject.SetActive(false);
                m_TargetMonsterArrow[1].gameObject.SetActive(true);
            }
            else
            {
                m_TargetMonsterArrow[0].gameObject.SetActive(true);
                m_TargetMonsterArrow[1].gameObject.SetActive(false);
            }
        }

        m_TargetMonsterCanvas[(int)eUITooltipSortType].SetActive(true);
        m_TargetBuildingCanvas[(int)eUITooltipSortType].SetActive(false);

        if (m_TargetSpawnCanvas)
            m_TargetSpawnCanvas.SetActive(false);

        if (m_TargetUpgradeCanvas)
            m_TargetUpgradeCanvas.SetActive(false);
    }

    public void SetSpawnInfo(SpawnSphere target)
    {
        // Awake보다 Event가 먼저 호출되는 경우가 있음
        if (eUITooltipSortType != (UITooltipSortType)(OptionManager.Instance.iTooltipSortType))
            eUITooltipSortType = (UITooltipSortType)(OptionManager.Instance.iTooltipSortType);

        if (target == null)
            return;

        m_pTarget = null;
        m_pSpawnTarget = target;
        
        attackRangeVisualizationClass.Hide();

        YPos = -0.5f;
        if (m_TargetSpawnCanvas)
        {
            panelRectTransform = m_TargetSpawnCanvas.GetComponent<RectTransform>();
            m_TargetSpawnCanvas.SetActive(true);
        }

        if (m_TargetUpgradeCanvas)
            m_TargetUpgradeCanvas.SetActive(false);

        m_TargetBuildingCanvas[(int)eUITooltipSortType].SetActive(false);
        m_TargetMonsterCanvas[(int)eUITooltipSortType].SetActive(false);
        
    }

    private bool CheckConditions(int Cost)
    {
        string strNotice = "";
        if (Cost <= 0)
            return false;

        Player MyPlayer = GameManager.Instance.GetPlayer();
        if (MyPlayer == null)
            return false;

        int myMoney = MyPlayer.GetMoney();
        if (myMoney - Cost >= 0)
        {
            return true;
        }
        else
        {
            strNotice = "Not enough money... make money";
        }

        if (strNotice.Length > 0)
        {

        }

        return false;
    }

    private void AdjustFontSize()
    {
        int currentFontSize = m_BText0[(int)eUITooltipSortType].fontSize;

        // UI 사이즈를 벗어나는지 확인
        if (m_BText0[(int)eUITooltipSortType].preferredWidth > m_BText0[(int)eUITooltipSortType].rectTransform.rect.width ||
            m_BText0[(int)eUITooltipSortType].preferredHeight > m_BText0[(int)eUITooltipSortType].rectTransform.rect.height)
        {
            for (int size = currentFontSize; size >= minFontSize; size--)
            {
                m_BText0[(int)eUITooltipSortType].fontSize = size;
                if (m_BText0[(int)eUITooltipSortType].preferredWidth <= m_BText0[(int)eUITooltipSortType].rectTransform.rect.width &&
                    m_BText0[(int)eUITooltipSortType].preferredHeight <= m_BText0[(int)eUITooltipSortType].rectTransform.rect.height)
                {
                    break;
                }
            }
        }
        else
        {
            m_BText0[(int)eUITooltipSortType].fontSize = maxFontSize;
        }
    }
}
