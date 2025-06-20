using UnityEngine;
using UnityEngine.UI;
using GameDefines;
using OptionDefines;
using System;

public class CharacterDeck : MonoBehaviour
{
    [SerializeField] private Image arrow = null;
    [SerializeField] private Image characterSlot = null;
    [SerializeField] private Image GradeImage = null;
    [SerializeField] private Text characterName = null;
    [SerializeField] private Text characterDesc = null;

    public bool selected { get; private set; } = false;
    private int maxSelectCount = 0;

    private enum EtcSpriteType : int
    {
        DAMAGE,
        ATTACKSPEED,
        RANGE,
        MOVESPEED,
        JUMP,
        HEAL,
        LEVELUP,
        NONE,
    }

    public int ToggleArrow(int curSelectCount)
    {
        if (!selected && maxSelectCount <= curSelectCount)
            return 0;

        selected = !selected;
        arrow.gameObject.SetActive(selected);

        if (selected)
        {
            SoundManager.Instance.PlayUISound(UISoundType.PICK);
        }

        return selected ? 1 : -1;
    }

    public void SetSpeciesType(SpeciesType eSpeciesType, int maxCount)
    {
        string SpeciesText = Oracle.GetSpeciesTypeString(eSpeciesType);
        characterName.text = SpeciesText;

        string spriteName = SpeciesText + 4;
        characterSlot.sprite = UIManager.Instance.GetSprite(spriteName);

        if (Oracle.m_eGameType == MapType.ADVENTURE)
            arrow.gameObject.SetActive(true);
        else
            arrow.gameObject.SetActive(false);
        maxSelectCount = maxCount;
    }

    // -Adventure
    private void SetDesc(string sDescText)
    {
        characterDesc.text = sDescText;
    }

    private void SetSpriteAndName(EtcSpriteType eEtcSpriteType)
    {
        if (eEtcSpriteType == EtcSpriteType.NONE)
            return;

        characterName.text = GetName(eEtcSpriteType);
        characterSlot.sprite = UIManager.Instance.GetEtcSprite((int)eEtcSpriteType);
        arrow.gameObject.SetActive(true);
    }

    private string GetName(EtcSpriteType eEtcSpriteType)
    {
        switch(eEtcSpriteType)
        {
            case EtcSpriteType.DAMAGE:          return "REPAIR";
            case EtcSpriteType.ATTACKSPEED:     return "AGILITY";
            case EtcSpriteType.RANGE:           return "GOOD EYES";
            case EtcSpriteType.MOVESPEED:       return "CHANGE SHOES";
            case EtcSpriteType.JUMP:            return "DIET";
            case EtcSpriteType.HEAL:            return "FEEDING";
            case EtcSpriteType.LEVELUP:         return "GET A JOB";
        }

        return "";
    }

    public void SetAdventureItem(AdventureLevelUpItemType eAdventureLevelUpItemType, int grade, int value)
    {
        Player MyPlayer = GameManager.Instance.GetPlayer();
        if (MyPlayer == null)
            return;

        LevelUpSystemInfo outLevelUpSystemInfo = ResourceAgent.Instance.GetLevelUpSystemData(eAdventureLevelUpItemType, value);
        if (outLevelUpSystemInfo.eAdventureLevelUpItemType == AdventureLevelUpItemType.MAX)
            return;

        Image PanelImage = GetComponent<Image>();
        if (PanelImage)
            PanelImage.color = GradeColor(grade);

        string strDescFormat = outLevelUpSystemInfo.Description;
        string strDescParam = "";
        EtcSpriteType eEtcSpriteType = EtcSpriteType.NONE;
        switch (eAdventureLevelUpItemType)
        {
            case AdventureLevelUpItemType.CHARACTER:
            {
                SpeciesType eSpeciesType = (SpeciesType)value;
                SetSpeciesType(eSpeciesType, 0);
                SetDesc(strDescFormat);
                return;
            }
            case AdventureLevelUpItemType.STAT:
            {
                AdventureLevelUpStatType eAdventureLevelUpStatType = (AdventureLevelUpStatType)value;
                switch (eAdventureLevelUpStatType)
                {
                    case AdventureLevelUpStatType.DAMAGE:
                        eEtcSpriteType = EtcSpriteType.DAMAGE;
                        break;
                    case AdventureLevelUpStatType.ATTACKSPEED:
                        eEtcSpriteType = EtcSpriteType.ATTACKSPEED;
                        break;
                    case AdventureLevelUpStatType.RANGE:
                        eEtcSpriteType = EtcSpriteType.RANGE;
                        break;
                    case AdventureLevelUpStatType.MOVE:
                        eEtcSpriteType = EtcSpriteType.MOVESPEED;
                        break;
                    case AdventureLevelUpStatType.JUMP:
                        eEtcSpriteType = EtcSpriteType.JUMP;
                        break;
                };

                int valueIndex = MyPlayer.GetAdventureLevelUpStat(eAdventureLevelUpStatType);
                if (valueIndex < outLevelUpSystemInfo.valueList.Length)
                {
                    strDescParam = string.Format("{0:F1}", outLevelUpSystemInfo.valueList[valueIndex]);
                    //strDescParam = Convert.ToInt32(outLevelUpSystemInfo.valueList[valueIndex]).ToString();
                }
            }
            break;
            case AdventureLevelUpItemType.UTIL:
            {
                if (value == 1)
                {
                    eEtcSpriteType = EtcSpriteType.HEAL;
                }
                else
                {
                    int valueIndex = MyPlayer.selectedBuildingIndex < 0 ? 0 : MyPlayer.selectedBuildingIndex;
                    if (valueIndex < outLevelUpSystemInfo.valueList.Length)
                    {
                        strDescParam = Convert.ToInt32(outLevelUpSystemInfo.valueList[valueIndex]).ToString();
                    }

                    eEtcSpriteType = EtcSpriteType.LEVELUP;
                }
            }
            break;
        };

        SetSpriteAndName(eEtcSpriteType);

        string strDesc = string.Format(strDescFormat, strDescParam);
        SetDesc(strDesc);
    }

    public void SetGradeImage(Sprite gradeSprite)
    {
        if (GradeImage == null)
            return;

        if (gradeSprite == null)
        {
            GradeImage.gameObject.SetActive(false);
        }
        else
        {
            GradeImage.sprite = gradeSprite;
            GradeImage.SetNativeSize();
            GradeImage.gameObject.SetActive(true);
        }
    }

    private Color GradeColor(int iGrade)
    {
        Color outColor = new Color(1f, 1f, 1f);
        switch (iGrade)
        { 
            case 1:
                outColor = new Color(205f / 255f, 127f / 255f, 50f / 255f);
                break;
            case 2:
                outColor = new Color(192f / 255f, 192f / 255f, 192f / 255f);
                break;
            case 3:
                outColor = new Color(255f / 255f, 215f / 255f, 0f);
                break;
            case 4:
            case 5:
            case 6:
                outColor = new Color(83f / 255f, 1f, 1f);
                break;
        }

        return outColor;
    }

    ////////////////////////////////////////////////////////////////////////////////////////
}
