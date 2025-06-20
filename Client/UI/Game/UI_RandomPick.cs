using System.Collections.Generic;
using GameDefines;
using OptionDefines;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UI_RandomPick : UIBase
{
    [SerializeField] private Button[] m_Items;
    [SerializeField] private Sprite[] m_GradeSpriteArray;

    private List<(AdventureLevelUpItemType, int)> m_CurrentItemList;

    public override void SetControlInfo()
    {
        base.SetControlInfo();

        m_CurrentItemList = new List<(AdventureLevelUpItemType, int)>();
        UIManager.Instance.OnRoundPeriodic += HandleRoundPeriodicEvent;

        int btnIndex = 0;
        if (m_Items[btnIndex] != null)
        {
            m_Items[btnIndex].onClick.AddListener(OnClick_Pick0);
            ++btnIndex;
        }
        if (m_Items[btnIndex] != null)
        {
            m_Items[btnIndex].onClick.AddListener(OnClick_Pick1);
            ++btnIndex;
        }
        if (m_Items[btnIndex] != null)
        {
            m_Items[btnIndex].onClick.AddListener(OnClick_Pick2);
            ++btnIndex;
        }
    }
    protected override void PreShow()
    {
        SoundManager.Instance.PlayUISound(UISoundType.UPGRADE);
    }

    private void OnClick_Pick0()
    {
        SelectedDeck(0);
    }
    private void OnClick_Pick1()
    {
        SelectedDeck(1);
    }
    private void OnClick_Pick2()
    {
        SelectedDeck(2);
    }

    private void SelectedDeck(int index)
    {
        if (index < 0 || index >= m_CurrentItemList.Count)
            return;

        Player MyPlayer = GameManager.Instance.GetPlayer();
        if (MyPlayer != null)
        {
            MyPlayer.SetSelectedPick(m_CurrentItemList[index].Item1, m_CurrentItemList[index].Item2, false);
        }

        SoundManager.Instance.PlayUISound(UISoundType.DECLINE);
        UIManager.Instance.HideUI(eUIIndexType);
    }

    private void HandleRoundPeriodicEvent(int iStage)
    {
        SetRandomPick(iStage);
        UIManager.Instance.ShowUI(eUIIndexType);
    }

    private void SetRandomPick(int iStage)
    {
        m_CurrentItemList.Clear();
        if (m_Items.Length < 3)
            return;

        Player MyPlayer = GameManager.Instance.GetPlayer();
        if (MyPlayer == null)
            return;

        // 조건만들기
        int[] iTotal_Type = new int[(int)AdventureLevelUpItemType.MAX];
        List<AdventureLevelUpItemType> ConditionList = new List<AdventureLevelUpItemType>();
        List<SpeciesType> useableSpeciesTypeList = UnlockManager.Instance.GetUseableSpeciesTypeList();
        //캐릭터
        if (MyPlayer.AdventureCharacterPickCount < MyPlayer.AdventureCharacterMaxCount)
        {
            if (useableSpeciesTypeList.Count > 0)
            {
                iTotal_Type[0] = MyPlayer.AdventureCharacterMaxCount - MyPlayer.AdventureCharacterPickCount;
                ConditionList.Add(AdventureLevelUpItemType.CHARACTER);
            }
        }
        //스탯
        List<AdventureLevelUpStatType> UseStatList = new List<AdventureLevelUpStatType>();
        for (int i = 0; i < (int)(AdventureLevelUpStatType.MAX); ++i)
        {
            int iLevelStat = MyPlayer.GetAdventureLevelUpStat((AdventureLevelUpStatType)i);
            if (iLevelStat < 5)
                UseStatList.Add((AdventureLevelUpStatType)i);
        }

        if (UseStatList.Count > 0)
        {
            iTotal_Type[1] = UseStatList.Count;
            ConditionList.Add(AdventureLevelUpItemType.STAT);
        }
        //유틸
        int iUtil = 1;
        iTotal_Type[2] = 1;
        //if (MyPlayer.selectedBuildingIndex < 4)
        //{
        //    iUtil |= 2;
        //    iTotal_Type[2] += 1;
        //}

        if (iTotal_Type[2] > 0)
        {
            ConditionList.Add(AdventureLevelUpItemType.UTIL);
        }
        /////////////////////////////////////////////////////////////////////

        int maxCount = 2;
        int iTotal = iTotal_Type.Sum();
        if (iTotal > 2 && iStage > 0)
        {
            maxCount = iStage % 10 == 0 ? 3 : Oracle.PercentSuccess(10f) ? 3 : 2;
        }

        m_Items[0].gameObject.SetActive(true);
        m_Items[1].gameObject.SetActive(true);
        if (maxCount == 2)
            m_Items[2].gameObject.SetActive(false);
        else
            m_Items[2].gameObject.SetActive(true);

        if (iTotal == 0)
        {
            UIManager.Instance.HideUI(eUIIndexType);
            return;
        }

        for (int i = 0; i < maxCount; ++i)
        {
            int iGrade = 0;
            CharacterDeck deck = m_Items[i].GetComponent<CharacterDeck>();
            if (deck == null)
                continue;

            // 확률 구하기
            AdventureLevelUpItemType eAdventureLevelUpItemType = AdventureLevelUpItemType.MAX;
            if (iStage == 5)   // 5탄은 무조건 캐릭터
            {
                eAdventureLevelUpItemType = AdventureLevelUpItemType.CHARACTER;
            }
            else
            {
                for (int j = 0; j < iTotal_Type.Length; ++j)
                {
                    if (iTotal_Type[j] == 0)
                        continue;

                    if (Oracle.PercentSuccess((float)(iTotal_Type[j]) / (float)iTotal * 100f))
                    {
                        eAdventureLevelUpItemType = (AdventureLevelUpItemType)j;
                        break;
                    }
                    else if (j == iTotal_Type.Length - 1)
                    {
                        Debug.Log("PercentSuccess is error");
                    }

                    iTotal -= iTotal_Type[j];
                }
            }

            int iRandomValue;
            if (eAdventureLevelUpItemType == AdventureLevelUpItemType.CHARACTER)
            {
                int iCharIdx = Oracle.RandomDice(0, useableSpeciesTypeList.Count);
                SpeciesType eSpeciesType = useableSpeciesTypeList[iCharIdx];
                iRandomValue = (int)eSpeciesType;
                useableSpeciesTypeList.RemoveAt(iCharIdx);
                iTotal_Type[0] -= 1;
                iTotal -= 1;

                BuildingInfo buildingInfo = ResourceAgent.Instance.GetBuildingInfo(eSpeciesType, 0);
                iGrade = buildingInfo.Cost;

                if (useableSpeciesTypeList.Count == 0)
                {
                    iTotal_Type[0] = 0;
                    ConditionList.Remove(AdventureLevelUpItemType.CHARACTER);
                }
            }
            else if (eAdventureLevelUpItemType == AdventureLevelUpItemType.STAT)
            {
                iRandomValue = (int)(UseStatList[Oracle.RandomDice(0, UseStatList.Count)]);
            }
            else if (eAdventureLevelUpItemType == AdventureLevelUpItemType.UTIL)
            {
                List<int> UtilIndexRandomList = new List<int>();
                UtilIndexRandomList.Add(1);
                if ((iUtil & (1 << 1)) != 0)
                    UtilIndexRandomList.Add(2);

                iRandomValue = UtilIndexRandomList[Oracle.RandomDice(0, UtilIndexRandomList.Count)];
            }
            else
            {
                m_Items[i].gameObject.SetActive(false);
                continue;
            }

            m_CurrentItemList.Add((eAdventureLevelUpItemType, iRandomValue));
            deck.SetAdventureItem(eAdventureLevelUpItemType, iGrade, iRandomValue);

            if (iGrade > 3)
            {
                int gradeSpriteIndex = iGrade - 4;
                if (m_GradeSpriteArray != null && m_GradeSpriteArray.Length > gradeSpriteIndex)
                    deck.SetGradeImage(m_GradeSpriteArray[gradeSpriteIndex]);
            }
            else
                deck.SetGradeImage(null);
        }
    }
}
