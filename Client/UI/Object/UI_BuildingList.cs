using OptionDefines;
using GameDefines;
using UIDefines;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BuildingList : UIBase
{
    [SerializeField] private UI_FollowMousePositionBuilding followMousePositionBuilding;
    [SerializeField] private Button[] m_SpawnBtnList;
    [SerializeField] private Image[] m_SpawnBtnImageList;
    [SerializeField] private Button m_RerollBtn;
    [SerializeField] private Text m_RerollCountText;

    private int maxButtonCount = 5;
    //private Selectable.Transition[] originalTransition;
    private SpeciesType m_targetSpeciesType = SpeciesType.NONE;
    private AttributeType m_targetAttributeType = AttributeType.NONE;

    private Color possibleColor = new Color(55f / 255f, 1f, 11f / 255f, 1f);
    private Color impossibleColor = new Color(1f, 47f / 255f, 47f / 255f, 1f);
    private Button CurrentOverButton = null;
    private int overIndex = -1;
    private bool m_bCheck = false;
    private Vector3 originRollbackBtnSize = Vector3.zero;
    private bool[] m_ButtonsEnable = { true, true, false, false, false };

    protected override void Awake()
    {
        for (int i = 0; i < m_SpawnBtnList.Length; ++i)
        {
            ColorBlock colors = m_SpawnBtnList[i].colors;
            colors.highlightedColor = impossibleColor;
            colors.selectedColor = impossibleColor;
            m_SpawnBtnList[i].colors = colors;

            MouseOverHandler Spawnhandler = m_SpawnBtnList[i].GetComponent<MouseOverHandler>();
            Spawnhandler.ButtonMouseOver += HandleButtonOverEnter;
            Spawnhandler.ButtonMouseOut += HandleButtonOverExit;
        }

        if (m_RerollBtn)
        {
            MouseOverHandler Rerollhandler = m_RerollBtn.GetComponent<MouseOverHandler>();
            Rerollhandler.ButtonMouseOver += HandleButtonOverEnter;
            Rerollhandler.ButtonMouseOut += HandleButtonOverExit;
            originRollbackBtnSize = m_RerollBtn.transform.localScale;
        }
    }

    public void HandleButtonOverEnter(object sender, GameObject button)
    {
        CurrentOverButton = button.GetComponent<Button>();
        for (int i = 0; i < maxButtonCount; ++i)
        {
            if (CurrentOverButton == m_SpawnBtnList[i])
            {
                overIndex = i;
                BuildingInfo buildingInfo = ResourceAgent.Instance.GetBuildingInfo(m_targetSpeciesType, overIndex);
                m_bCheck = !CheckConditions(buildingInfo.Cost);
                return;
            }
        }

        if (m_RerollBtn == CurrentOverButton)
        {
            if (m_RerollBtn.enabled)
                m_RerollBtn.transform.localScale *= 1.1f;
        }
    }
    public void HandleButtonOverExit(object sender, GameObject button)
    {
        for (int i = 0; i < maxButtonCount; ++i)
        {
            if (CurrentOverButton == m_SpawnBtnList[i])
            {
                m_SpawnBtnList[i].interactable = false;
                m_SpawnBtnList[i].interactable = true;
                break;
            }
        }

        if (m_RerollBtn == CurrentOverButton)
        {
            if (m_RerollBtn.enabled)
                m_RerollBtn.transform.localScale = originRollbackBtnSize;
        }

        overIndex = -1;
        CurrentOverButton = null;
    }

    protected override void HandleWaveReadyEvent(SpeciesType eSpeciesType)
    {
        if (m_targetSpeciesType == eSpeciesType)
        {
            StartCoroutine(ChangeButtonProduction());
            return;
        }

        followMousePositionBuilding.Hide();
        m_targetSpeciesType = eSpeciesType;
        SetSpecies();
        UpdatePlayer();
    }
    protected override void HandleChangeSpeciesTypeEvent(SpeciesType eSpeciesType, AttributeType eAttributeType)
    {
        m_targetSpeciesType = eSpeciesType;
        m_targetAttributeType = eAttributeType;
        SetSpecies();
        UpdatePlayer();
    }

    protected override void HandleUpdatePlayerEvent(UIEventArgs e)
    {
        ChangeButtonMoneyLabel();
    }

    protected override void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) == false)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                OnClick_Spawn0();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                OnClick_Spawn1();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                OnClick_Spawn2();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                OnClick_Spawn3();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                OnClick_Spawn4();
            }
        }

        if (overIndex >= 0)
        {
            BuildingInfo buildingInfo = ResourceAgent.Instance.GetBuildingInfo(m_targetSpeciesType, overIndex);
            bool bCheck = CheckConditions(buildingInfo.Cost);
            if (m_bCheck != bCheck)
            {
                m_bCheck = bCheck;
                ColorBlock colors = CurrentOverButton.colors;
                if (m_bCheck)
                {
                    colors.highlightedColor = possibleColor;
                    colors.selectedColor = possibleColor;
                }
                else
                {
                    colors.highlightedColor = impossibleColor;
                    colors.selectedColor = impossibleColor;
                }

                CurrentOverButton.colors = colors;
            }
        }
    }

    private void OnClick_Spawn0()
    {
        BuildBuilding(0);
    }
    private void OnClick_Spawn1()
    {
        BuildBuilding(1);
    }
    private void OnClick_Spawn2()
    {
        BuildBuilding(2);
    }
    private void OnClick_Spawn3()
    {
        BuildBuilding(3);
    }
    private void OnClick_Spawn4()
    {
        BuildBuilding(4);
    }
    private void OnClick_Reroll()
    {
        Player MyPlayer = GameManager.Instance.GetPlayer();
        if (MyPlayer == null)
            return;

        MyPlayer.RerollSpeciesType();
        SoundManager.Instance.PlayUISound(UISoundType.SLOTCLICK);
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
            strNotice = "Not enough money... mine more money";
        }

        if (strNotice.Length > 0)
        {

        }

        return false;
    }

    private void BuildBuilding(int index)
    {
        BuildingInfo buildingInfo = ResourceAgent.Instance.GetBuildingInfo(m_targetSpeciesType, index);
        if (CheckConditions(buildingInfo.Cost))
        {
            string spriteName = Oracle.GetSpeciesTypeString(m_targetSpeciesType) + index;
            followMousePositionBuilding.SetBuilding(spriteName);
            followMousePositionBuilding.Show();

            GameManager.Instance.GetPlayer().selectedBuildingIndex = index;
        }

        SoundManager.Instance.PlayUISound(UISoundType.SLOTCLICK);
    }

    public void OffBuildBuilding()
    {
        followMousePositionBuilding.Hide();
    }

    public override void SetControlInfo()
    {
        base.SetControlInfo();

        maxButtonCount = m_SpawnBtnList.Length;
        //originalTransition = new Selectable.Transition[maxButtonCount];

        int index = 0;
        if (m_SpawnBtnList[index] != null)
        {
            m_SpawnBtnList[index].onClick.AddListener(OnClick_Spawn0);
            //originalTransition[index] = m_SpawnBtnList[index].transition;
            ++index;
        }
        if (m_SpawnBtnList[index] != null)
        {
            m_SpawnBtnList[index].onClick.AddListener(OnClick_Spawn1);
            //originalTransition[index] = m_SpawnBtnList[index].transition;
            ++index;
        }
        if (m_SpawnBtnList[index] != null)
        {
            m_SpawnBtnList[index].onClick.AddListener(OnClick_Spawn2);
            //originalTransition[index] = m_SpawnBtnList[index].transition;
            ++index;
        }
        if (m_SpawnBtnList[index] != null)
        {
            m_SpawnBtnList[index].onClick.AddListener(OnClick_Spawn3);
            //originalTransition[index] = m_SpawnBtnList[index].transition;
            ++index;
        }
        if (m_SpawnBtnList[index] != null)
        {
            m_SpawnBtnList[index].onClick.AddListener(OnClick_Spawn4);
            //originalTransition[index] = m_SpawnBtnList[index].transition;
            ++index;
        }

        if (m_RerollBtn != null)
            m_RerollBtn.onClick.AddListener(OnClick_Reroll);
    }

    private void SetSpecies()
    {
        string SpeciesText = Oracle.GetSpeciesTypeString(m_targetSpeciesType);
        for (int i = 0; i < maxButtonCount; ++i)
        {
            Button btn = m_SpawnBtnList[i];
            if (btn == null)
                continue;

            string text = "";
            bool iconVisible = false;

            BuildingInfo buildingInfo = ResourceAgent.Instance.GetBuildingInfo(m_targetSpeciesType, i);
            if (buildingInfo.Cost != 0)
            {
                iconVisible = true;
                text = Oracle.ConvertNumberDigit(buildingInfo.Cost);

                string spriteName = SpeciesText + i;
                btn.image.sprite = UIManager.Instance.GetSprite(spriteName);
            }

            btn.enabled = iconVisible;

            Text buttonText = btn.GetComponentInChildren<Text>();
            if (buttonText)
            {
                buttonText.text = text;
            }

            Image costIcon = btn.GetComponentInChildren<Image>();
            if (costIcon)
            {
                costIcon.enabled = iconVisible;
            }
        }

        ChangeButtonMoneyLabel();
        StartCoroutine(ChangeButtonProduction());
    }

    private void ChangeButtonMoneyLabel()
    {
        for (int i = 0; i < maxButtonCount; ++i)
        {
            Button btn = m_SpawnBtnList[i];
            if (btn == null)
                continue;

            Text buttonText = btn.GetComponentInChildren<Text>();
            if (buttonText)
            {
                BuildingInfo buildingInfo = ResourceAgent.Instance.GetBuildingInfo(m_targetSpeciesType, i);
                bool bCheck = CheckConditions(buildingInfo.Cost);
                string text = Oracle.ConvertNumberDigit(buildingInfo.Cost);
                if (!bCheck)
                {
                    text = string.Format("<color=red>{0}</color>", text);
                }

                buttonText.text = text;
                m_ButtonsEnable[i] = bCheck;
            }
        }
    }

    private void UpdatePlayer()
    {
        Player MyPlayer = GameManager.Instance.GetPlayer();
        if (MyPlayer == null)
            return;

        int RerollCount = MyPlayer.RerollSpeciesTypeCount;
        m_RerollBtn.enabled = RerollCount <= 0 ? false : true;
        if (RerollCount <= 0)
            m_RerollBtn.transform.localScale = originRollbackBtnSize;

        m_RerollCountText.text = string.Format("X{0}", RerollCount.ToString());
    }

    private IEnumerator ChangeButtonProduction()
    {
        float[] angles = new float[maxButtonCount];
        bool bStop = false;

        while (true) 
        {
            for (int i = 0; i < maxButtonCount; ++i)
            {
                Button btn = m_SpawnBtnList[i];
                if (btn == null)
                    continue;

                angles[i] += 450f * Time.deltaTime;
                btn.gameObject.transform.rotation = Quaternion.Euler(angles[i], 0f, 0f);

                if (angles[i] >= 360f) // 360도 회전했을 때
                {
                    btn.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    if (i == maxButtonCount - 1)
                    {
                        bStop = true;
                        break;
                    }
                }
            }

            if (bStop)
                break;
             
            yield return null;
        }

        StopCoroutine(ChangeButtonProduction());
    }
}
