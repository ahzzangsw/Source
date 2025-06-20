using GameDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class UI_ControlPanel : UIBase
{
    [SerializeField] private Button[] m_UpgradeBtnList;
    [SerializeField] private Button[] m_BossSpawnBtnList;
    [SerializeField] private Button m_WorkmanBtn;

    [Header("BOSS")]
    [SerializeField] private Text m_BossSpawnDelayText;
    [SerializeField] private Slider m_BossSpawnDelayBar;
    [SerializeField] private float m_fBossSpawnDelay = 0f;
    [SerializeField] private Image m_BossSpawnDelayLockBG;
    [SerializeField] private Image m_BossSpawnDelaySliderUnlock;

    [Header("ETC")]
    [SerializeField] private Sprite[] m_GradeSpriteArray;
    [SerializeField] private Image[] m_GradeArray;

    private int maxButtonCount = 5;

    private Color possibleColor = new Color(55f / 255f, 1f, 11f / 255f, 1f);
    private Color impossibleColor = new Color(1f, 47f / 255f, 47f / 255f, 1f);
    private Color disableColor = new Color(237f / 255f, 235 / 255f, 235 / 255f, 1f);
    private Button CurrentOverButton = null;
    private int overIndex = -1;
    private int bossOverIndex = -1;
    private bool m_bCheck = false;
    private Vector3 originRollbackBtnSize = Vector3.zero;

    List<SpeciesType> deckSpeciesTypeList = null;
    private int[] BossSpriteIndexList;
    private int[] BossDefenseList;
    private float[] BossSpeedList;
    private bool[] BossSpawnComeList;

    private bool m_bBossSpawnWait = false;
    private int bossSpawnIndex = 0;
    private int maxBossDelayValue = 777;
    private int currentValue = 0;
    private RectTransform BossSpawnDelaySliderUnlockRT;

    protected override void Awake()
    {
        for (int i = 0; i < m_UpgradeBtnList.Length; ++i)
        {
            ColorBlock colors = m_UpgradeBtnList[i].colors;
            colors.highlightedColor = impossibleColor;
            colors.selectedColor = impossibleColor;
            m_UpgradeBtnList[i].colors = colors;

            MouseOverHandler Upgradehandler = m_UpgradeBtnList[i].GetComponent<MouseOverHandler>();
            Upgradehandler.ButtonMouseOver += HandleButtonOverEnter;
            Upgradehandler.ButtonMouseOut += HandleButtonOverExit;
        }

        if (m_WorkmanBtn)
        {
            m_WorkmanBtn.enabled = true;
            MouseOverHandler Workmanhandler = m_WorkmanBtn.GetComponent<MouseOverHandler>();
            Workmanhandler.ButtonMouseOver += HandleButtonOverEnter;
            Workmanhandler.ButtonMouseOut += HandleButtonOverExit;
            originRollbackBtnSize = m_WorkmanBtn.transform.localScale;
        }

        for (int i = 0; i < m_BossSpawnBtnList.Length; ++i)
        {
            ColorBlock colors = m_BossSpawnBtnList[i].colors;
            colors.highlightedColor = possibleColor;
            //colors.disabledColor = disableColor;
            colors.disabledColor = Color.gray;
            m_BossSpawnBtnList[i].colors = colors;

            MouseOverHandler Upgradehandler = m_BossSpawnBtnList[i].GetComponent<MouseOverHandler>();
            Upgradehandler.ButtonMouseOver += HandleButtonOverEnter;
            Upgradehandler.ButtonMouseOut += HandleButtonOverExit;
        }

        for (int i = 0; i < m_GradeArray.Length; ++i)
        {
            Image gradeImage = m_GradeArray[i];
            if (gradeImage == null)
                continue;

            gradeImage.enabled = false;
        }

        UIManager.Instance.OnChangeStartGame += HandleStartGameEvent;

        BossSpriteIndexList = new int[m_BossSpawnBtnList.Length];
        BossDefenseList = new int[] { 0, 1, 5, 7, 10, 15, 25, 30, 35 };
        BossSpeedList = new float[] { 0.5f, 0.5f, 0.7f, 0.7f, 0.9f, 0.9f, 1f, 1f, 2f };
        BossSpawnComeList = new bool[] { false, false, false, false, false, false, false, false, false };

        m_bBossSpawnWait = false;
        m_BossSpawnDelayLockBG.enabled = false;

        BossSpawnDelaySliderUnlockRT = m_BossSpawnDelaySliderUnlock.GetComponent<RectTransform>();

        if (m_BossSpawnBtnList.Length != BossDefenseList.Length || BossSpeedList.Length != BossSpawnComeList.Length)
        {
            Debug.Log("controlPanel UI Boss data error");
        }
    }

    public void HandleButtonOverEnter(object sender, GameObject button)
    {
        if (deckSpeciesTypeList == null || deckSpeciesTypeList.Count == 0)
            return;

        CurrentOverButton = button.GetComponent<Button>();
        for (int i = 0; i < maxButtonCount; ++i)
        {
            if (CurrentOverButton == m_UpgradeBtnList[i])
            {
                overIndex = i;

                Player MyPlayer = GameManager.Instance.GetPlayer();
                if (MyPlayer != null)
                {
                    ControlInfo controlInfoData = MyPlayer.GetCurrentUpgradeData(deckSpeciesTypeList[i]);
                    m_bCheck = !CheckConditions(controlInfoData.Cost);
                }
                return;
            }
        }

        for (int i = 0; i < m_BossSpawnBtnList.Length; ++i)
        {
            if (CurrentOverButton == m_BossSpawnBtnList[i])
            {
                bossOverIndex = i;

                Player MyPlayer = GameManager.Instance.GetPlayer();
                if (MyPlayer != null)
                {
                    //ControlInfo controlInfoData = MyPlayer.GetCurrentUpgradeData(deckSpeciesTypeList[i]);
                    //m_bCheck = !CheckConditions(controlInfoData.Cost);
                }
                return;
            }
        }

        if (m_WorkmanBtn == CurrentOverButton)
        {
            if (m_WorkmanBtn.enabled)
                m_WorkmanBtn.transform.localScale *= 1.1f;
        }
    }
    public void HandleButtonOverExit(object sender, GameObject button)
    {
        for (int i = 0; i < maxButtonCount; ++i)
        {
            if (CurrentOverButton == m_UpgradeBtnList[i])
            {
                m_UpgradeBtnList[i].interactable = false;
                m_UpgradeBtnList[i].interactable = true;
                break;
            }
        }

        if (m_WorkmanBtn == CurrentOverButton)
        {
            if (m_WorkmanBtn.enabled)
                m_WorkmanBtn.transform.localScale = originRollbackBtnSize;
        }

        overIndex = -1;
        bossOverIndex = -1;
        CurrentOverButton = null;
    }

    public override void SetControlInfo()
    {
        base.SetControlInfo();

        maxButtonCount = m_UpgradeBtnList.Length;

        deckSpeciesTypeList = null;
        deckSpeciesTypeList = BuildingPool.Instance.GetDeckSpeciesTypeList();

        for (int i = 0; i < m_UpgradeBtnList.Length; ++i)
        {
            Button item = m_UpgradeBtnList[i];
            if (item == null)
                continue;

            int index = i;
            item.onClick.AddListener(() => OnClick_Upgrade(index));
        }

        for (int i = 0; i < m_BossSpawnBtnList.Length; ++i)
        {
            Button item = m_BossSpawnBtnList[i];
            if (item == null)
                continue;

            int index = i;
            item.onClick.AddListener(() => OnClick_BossSpawn(index));
        }

        if (m_WorkmanBtn != null)
            m_WorkmanBtn.onClick.AddListener(OnClick_CreateWorkman);
    }

    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            OnClick_Upgrade(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            OnClick_Upgrade(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            OnClick_Upgrade(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            OnClick_Upgrade(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            OnClick_Upgrade(4);
        }

        if (overIndex >= 0)
        {
            Player MyPlayer = GameManager.Instance.GetPlayer();
            if (MyPlayer == null)
                return;

            ControlInfo controlInfoData = MyPlayer.GetCurrentUpgradeData(deckSpeciesTypeList[overIndex]);
            bool bCheck = CheckConditions(controlInfoData.Cost);
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

    private void OnClick_Upgrade(int index)
    {
        RunUpgrade(index);
    }

    private void OnClick_BossSpawn(int index)
    {
        RunBossSpawn(index);
    }

    private void OnClick_CreateWorkman()
    {
        Player MyPlayer = GameManager.Instance.GetPlayer();
        if (MyPlayer == null)
            return;

        if (MyPlayer.CreateWorkman() == true)
        {
            m_WorkmanBtn.enabled = false;

            Text buttonText = m_WorkmanBtn.GetComponentInChildren<Text>();
            if (buttonText)
            {
                buttonText.text = "X";
            }

            MouseOverHandler Rerollhandler = m_WorkmanBtn.GetComponent<MouseOverHandler>();
            if (Rerollhandler)
            {
                Rerollhandler.SetStringTooltip("<color=red>NO T.O</color>");
            }
        }
    }
    private void HandleStartGameEvent()
    {
        SetSpecies();
        SetBoss();
    }
    protected override void HandleUpdatePlayerEvent(UIEventArgs e)
    {
        ChangeButtonMoneyLabel();
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
    private void SetSpecies()
    {
        if (deckSpeciesTypeList.Count != maxButtonCount)
        {
            Debug.Log("deckSpeciesTypeList Count Not Same UI ControlPanel Button Count");
            return;
        }

        if (deckSpeciesTypeList == null)
            return;

        if (deckSpeciesTypeList.Count != maxButtonCount)
            return;

        Player MyPlayer = GameManager.Instance.GetPlayer();
        if (MyPlayer == null)
            return;

        for (int i = 0; i < maxButtonCount; ++i)
        {
            SpeciesType eSpeciesType = deckSpeciesTypeList[i];
            string SpeciesText = Oracle.GetSpeciesTypeString(eSpeciesType);
            Button btn = m_UpgradeBtnList[i];
            if (btn == null)
                continue;

            ControlInfo controlInfoData = MyPlayer.GetCurrentUpgradeData(eSpeciesType);
            string text = Oracle.ConvertNumberDigit(controlInfoData.Cost.ToString());
            string spriteName = SpeciesText + 4;
            btn.image.sprite = UIManager.Instance.GetSprite(spriteName);

            Text buttonText = btn.GetComponentInChildren<Text>();
            if (buttonText)
            {
                buttonText.text = text;
            }

            // Tooltip
            MouseOverHandler Rerollhandler = btn.GetComponent<MouseOverHandler>();
            if (Rerollhandler)
            {
                string strTooltip = string.Format("{0} Upgrade damage", Oracle.GetSpeciesTypeString(eSpeciesType));
                Rerollhandler.SetStringTooltip(strTooltip);
            }
        }

        ChangeButtonMoneyLabel();
    }

    private void SetBoss()
    {
        for (int i = 0; i < m_BossSpawnBtnList.Length; ++i)
        {
            Button btn = m_BossSpawnBtnList[i];
            if (btn == null)
                continue;

            // Sprite
            int index = 0;
            btn.image.sprite = UIManager.Instance.GetEtcSprite_Random(false, out index);
            BossSpriteIndexList[i] = index;

            // Tooltip
            MouseOverHandler Rerollhandler = btn.GetComponent<MouseOverHandler>();
            if (Rerollhandler)
            {
                //string strTooltip = string.Format("{0} Upgrade damage", Oracle.GetSpeciesTypeString(eSpeciesType));
                //Rerollhandler.SetStringTooltip(strTooltip);
            }
        }
    }

    private void ChangeButtonMoneyLabel()
    {
        if (deckSpeciesTypeList == null)
            return;

        if (m_GradeArray.Length != maxButtonCount)
        {
            Debug.Log("GradeArray Length error");
            return;
        }

        for (int i = 0; i < maxButtonCount; ++i)
        {
            Button btn = m_UpgradeBtnList[i];
            if (btn == null)
                continue;

            Player MyPlayer = GameManager.Instance.GetPlayer();
            if (MyPlayer == null)
                continue;

            ControlInfo controlInfoData = MyPlayer.GetCurrentUpgradeData(deckSpeciesTypeList[i]);

            Text buttonText = btn.GetComponentInChildren<Text>();
            if (buttonText)
            {
                bool bCheck = CheckConditions(controlInfoData.Cost);
                string text = Oracle.ConvertNumberDigit(controlInfoData.Cost.ToString());
                if (!bCheck)
                {
                    text = string.Format("<color=red>{0}</color>", text);
                }
                buttonText.text = text;
            }

            if (controlInfoData.Grade > 0 && controlInfoData.Grade <= m_GradeSpriteArray.Length)
            {
                Image GradeImage = m_GradeArray[i];
                if (GradeImage)
                {
                    if (GradeImage.enabled == false)
                        GradeImage.enabled = true;

                    Sprite sprite = m_GradeSpriteArray[controlInfoData.Grade - 1];
                    if (sprite)
                    {
                        Vector2 size = sprite.rect.size;

                        RectTransform rectTransform = GradeImage.GetComponent<RectTransform>();
                        rectTransform.sizeDelta = size;

                        GradeImage.sprite = sprite;
                    }
                }
            }
        }

        if (m_WorkmanBtn)
        {
            if (m_WorkmanBtn.enabled)
            {
                Text buttonText = m_WorkmanBtn.GetComponentInChildren<Text>();
                if (buttonText)
                {
                    ControlInfo controlInfo = ResourceAgent.Instance.GetControlInfoData();

                    bool bCheck = CheckConditions(controlInfo.WorkmanCost);
                    string text = Oracle.ConvertNumberDigit(controlInfo.WorkmanCost.ToString());
                    if (!bCheck)
                    {
                        text = string.Format("<color=red>{0}</color>", text);
                    }
                    buttonText.text = text;
                }
            }
        }
    }

    private void RunUpgrade(int index)
    {
        Player MyPlayer = GameManager.Instance.GetPlayer();
        if (MyPlayer == null)
            return;

        MyPlayer.UpgradeComplete(deckSpeciesTypeList[index]);
    }

    private void RunBossSpawn(int index)
    {
        if (m_bBossSpawnWait)
            return;

        int prefabIndex = BossSpriteIndexList[index];
        BossBase spawnBoss = MonsterPool.Instance.AddBossInfo(prefabIndex, index + 1, true);
        if (spawnBoss == null)
        {
            Debug.Log("RunBossSpawn is boss null");
            return;
        }

        m_bBossSpawnWait = true;
        StartCoroutine(RunBossSpawnDelay());

        spawnBoss.m_eSpeciesType = SpeciesType.MAX;
        spawnBoss.m_eAttributeType = MonsterPool.Instance.m_eCurrentWaveAttributeType;
        spawnBoss.prefabIndex = prefabIndex;

        ControlInfo controlInfo = ResourceAgent.Instance.GetControlInfoData();
        spawnBoss.SetInfo(bossSpawnIndex++, controlInfo.BossHPList[index], BossDefenseList[index], BossSpeedList[index], (short)controlInfo.BossMoneyEarnedList[index]);

        spawnBoss.SetSpawnMapProduction(BossSpawnComeList[index] == false);
        BossSpawnComeList[index] = true;
    }

    private IEnumerator RunBossSpawnDelay()
    {
        float timer = 0f; // 경과 시간
        m_BossSpawnDelayBar.value = 0f;
        m_BossSpawnDelayLockBG.enabled = true;
        float fBossSpawnLastDelay = m_fBossSpawnDelay - 2;

        BossSpawnButtonColorChange(false);

        while (timer < m_fBossSpawnDelay)
        {
            currentValue = (int)Mathf.Lerp(0, maxBossDelayValue, timer / m_fBossSpawnDelay);
            
            m_BossSpawnDelayText.text = string.Format("{0} / {1}", currentValue.ToString(), maxBossDelayValue.ToString());
            m_BossSpawnDelayBar.value = currentValue;

            float alpha = Mathf.Lerp(0f, 1f, timer / m_fBossSpawnDelay);
            m_BossSpawnDelayLockBG.color = new Color(m_BossSpawnDelayLockBG.color.r, m_BossSpawnDelayLockBG.color.g, m_BossSpawnDelayLockBG.color.b, alpha);

            timer += Time.deltaTime;
            yield return null;
        }

        BossSpawnButtonColorChange(true);
        m_BossSpawnDelayText.text = string.Format("{0} / {1}", maxBossDelayValue.ToString(), maxBossDelayValue.ToString());
        BossSpawnDelaySliderUnlockRT.DORotate(new Vector3(0f, 0f, 360f), 1.5f, RotateMode.FastBeyond360);
        BossSpawnDelaySliderUnlockRT.DOAnchorPos(new Vector2(0f, -67f), 1.5f).OnComplete(() =>
        {
            BossSpawnDelaySliderUnlockRT.DOAnchorPosX(187, 0.5f).OnComplete(() =>
            {
                ClearSpawnBossData();
            });
        });
    }

    private void ClearSpawnBossData()
    {
        currentValue = maxBossDelayValue;
        m_bBossSpawnWait = false;
        m_BossSpawnDelayLockBG.enabled = false;

        BossSpawnDelaySliderUnlockRT.DOAnchorPos(new Vector2(-432f, -270f), 0f);
    }

    private void BossSpawnButtonColorChange(bool bEnable)
    {
        for (int i = 0; i < m_BossSpawnBtnList.Length; ++i)
        {
            if (bEnable)
            {
                m_BossSpawnBtnList[i].interactable = true;
            }
            else
            {
                m_BossSpawnBtnList[i].interactable = false;
            }

            LayoutRebuilder.MarkLayoutForRebuild(m_BossSpawnBtnList[i].GetComponent<RectTransform>());
        }
    }
}
