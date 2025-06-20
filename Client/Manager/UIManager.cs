using UIDefines;
using System.Collections.Generic;
using UnityEngine;
using GameDefines;
using System;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Canvas[] UI;
    private Dictionary<UIIndexType, UIBase> UIList = new Dictionary<UIIndexType, UIBase>();

    [SerializeField] private Sprite[] SpriteArray;
    private Dictionary<string, Sprite> SpriteDictionary = new Dictionary<string, Sprite>();

    [SerializeField] private Sprite[] EtcSpriteArray;
    private Dictionary<int, Sprite> EtcSpriteDictionary = new Dictionary<int, Sprite>();
    private List<int> EtcSpriteRandomPickList;

    [SerializeField] private GameObject[] UIPrefeb;
    private Dictionary<UIPrefebType, GameObject> UIPrefebDictionary = new Dictionary<UIPrefebType, GameObject>();

    [SerializeField] private Sprite[] UIBuffArray;
    private Dictionary<BuffType, Sprite> UIBuffDictionary = new Dictionary<BuffType, Sprite>();

    [SerializeField] private GameObject PopUpObject;   // 여러개 생길시 배열로

    public event Action<Character, bool> OnCharacterUIEvent;
    public event Action<SpeciesType> OnWaveReadyUIEvent;
    public event Action<UIEventArgs> OnUpdateTooltipEvent;
    public event Action<SpeciesType, AttributeType> OnChangeSpeciesTypeUIEvent;
    public event Action<string, bool> OnNoticeEvent;
    public event Action<int> OnRoundPeriodic;

    public event Action OnChangeStartGame;
    public event Action OnChangeTooltipSortType;
    public event Action OnRankEvent;

    // Just Shop
    public event Action<int> OnRefreshShopItem;
    public event Action<int> OnSellShopItem;

    public bool bShowDialog = false;

    protected override void Awake()
    {
        LoadUI();
    }

    void Start()
    {
        
    }

    void LoadUI()
    {
        if (SpriteArray != null)
        {
            for (int i = 0; i < SpriteArray.Length; ++i)
            {
                Sprite sprite = SpriteArray[i];
                if (sprite == null)
                    continue;

                SpriteDictionary.Add(sprite.name, sprite);
            }
        }

        if (EtcSpriteArray != null)
        {
            EtcSpriteRandomPickList = new List<int>();
            for (int i = 0; i < EtcSpriteArray.Length; ++i)
            {
                Sprite sprite = EtcSpriteArray[i];
                if (sprite == null)
                    continue;

                EtcSpriteDictionary.Add(i, sprite);
                EtcSpriteRandomPickList.Add(i);
            }
        }

        if (UIPrefeb != null)
        {
            for (int i = 0; i < UIPrefeb.Length; ++i)
            {
                UIPrefebDictionary.Add((UIPrefebType)i, UIPrefeb[i]);
            }
        }

        if (UIBuffArray != null)
        {
            BuffType[] BuffTypeList = { BuffType.ARMORREDUCING0, BuffType.SLOW0, BuffType.POISON0, BuffType.BURN0, BuffType.STUN0, BuffType.CRITICAL0, BuffType.KNOCKBACK0 };
            for (int i = 0; i < UIBuffArray.Length; ++i)
            {
                Sprite sprite = UIBuffArray[i];
                if (sprite == null)
                    continue;

                UIBuffDictionary.Add(BuffTypeList[i], sprite);
            }
        }

        if (UI != null)
        {
            for (int i = 0; i < UI.Length; ++i)
            {
                UIBase ui = UI[i].GetComponent<UIBase>();
                if (ui == null)
                    continue;

                ui.SetControlInfo();
                UIList.Add(ui.GetUIIndexType(), ui);
            }
        }
    }

    public Sprite GetSprite(string name)
    {
        if (SpriteDictionary.ContainsKey(name))
        {
            return SpriteDictionary[name];
        }

        return null;
    }

    public Sprite GetEtcSprite(int index)
    {
        if (EtcSpriteDictionary.ContainsKey(index))
        {
            return EtcSpriteDictionary[index];
        }

        return null;
    }

    public Sprite GetEtcSprite_Random(bool bDuplication, out int SpriteIndex)
    {
        if (bDuplication)
        {
            SpriteIndex = Oracle.RandomDice(0, EtcSpriteDictionary.Count);
        }
        else
        {
            SpriteIndex = EtcSpriteRandomPickList[Oracle.RandomDice(0, EtcSpriteRandomPickList.Count)];
        }

        if (EtcSpriteDictionary.ContainsKey(SpriteIndex))
        {
            EtcSpriteRandomPickList.Remove(SpriteIndex);
            return EtcSpriteDictionary[SpriteIndex];
        }

        return null;
    }

    public Sprite GetBuffSprite(BuffType eBuffType)
    {
        BuffType eFilterBuffType = Oracle.GetBuffCategory(eBuffType, true);
        if (UIBuffDictionary.ContainsKey(eFilterBuffType))
        {
            return UIBuffDictionary[eFilterBuffType];
        }

        return null;
    }

    public GameObject GetUIPrefeb(UIPrefebType eUIPrefebType)
    {
        if (UIPrefebDictionary.ContainsKey(eUIPrefebType))
        {
            return UIPrefebDictionary[eUIPrefebType];
        }

        return null;
    }
    ////////////////////////////////////////////////////////////////////////////////////
    public void ShowUI(UIIndexType eUIIndexType)
    {
        if (CheckUI(eUIIndexType))
        {
            if (UIList[eUIIndexType].IsShow() == false)
                UIList[eUIIndexType].Show();
        }
    }

    public void HideUI(UIIndexType eUIIndexType)
    {
        if (CheckUI(eUIIndexType))
        {
            if (UIList[eUIIndexType].IsShow() == true)
                UIList[eUIIndexType].Hide();
        }
    }

    public UIBase GetUI(UIIndexType eUIIndexType)
    {
        if (CheckUI(eUIIndexType))
        {
            return UIList[eUIIndexType];
        }

        return null;
    }

    public bool IsShow(UIIndexType eUIIndexType)
    {
        if (CheckUI(eUIIndexType))
        {
            return UIList[eUIIndexType].IsShow();
        }

        return false;
    }

    private bool CheckUI(UIIndexType eUIIndexType)
    {
        if (UIList.ContainsKey(eUIIndexType))
            return true;

        Debug.Log("Not Find UI Index = " + eUIIndexType);
        return false;
    }

    ////////////////////////////////////////////////////////////////////////////////////
    public void ShowCharacterUI(UIIndexType eUIIndexType, Character pTarget, bool update)
    {
        if (pTarget == null)
            return;

        if (pTarget.m_eClickTargetType == ClickTargetType.BOSS || pTarget.m_eClickTargetType == ClickTargetType.FINAL)
            return;

        if (CheckUI(eUIIndexType)) 
        {
            OnCharacterUIEvent?.Invoke(pTarget, update);

            if (update == false)
            {
                ShowUI(eUIIndexType);
            }
        }
    }

    public void ShowSpawnSphereUI(UIIndexType eUIIndexType, SpawnSphere pTarget)
    {
        if (pTarget == null)
            return;

        UIBase ui = UIManager.Instance.GetUI(eUIIndexType);
        if (ui == null)
            return;
        
        UI_TargetDataViewer targetDataViewerUI = ui as UI_TargetDataViewer;
        if (targetDataViewerUI)
        {
            targetDataViewerUI.SetSpawnInfo(pTarget);
        }

        ShowUI(eUIIndexType);
    }

    public void ShowWaveUI(SpeciesType eSpeciesType)
    {
        Action<UIIndexType> showandcheckUI = (eUIIndexType) =>
        {
            if (CheckUI(eUIIndexType))
            {
                OnWaveReadyUIEvent?.Invoke(eSpeciesType);
                ShowUI(eUIIndexType);
            }
        };

        if (Oracle.m_eGameType == MapType.BUILD)
            showandcheckUI(UIIndexType.BUILDINGLIST);
        else
        {
            OnWaveReadyUIEvent?.Invoke(eSpeciesType);
        }
    }
    ////////////////////////////////////////////////////////////////////////////////////
    public void ShowTooltip(UITooltipType eUITooltipType, string text, Vector2 pivot)
    {
        if (CheckUI(UIIndexType.TOOLTIP) == false)
            return;

        UIEventArgs UIArgs = null;
        if (eUITooltipType == UITooltipType.STRING)
        {
            UIArgs = new UIEventArgs(text);
        }
        else if (eUITooltipType == UITooltipType.BUILDINGINFO || eUITooltipType == UITooltipType.SPAWNBOSSINFO)
        {
            int index = int.Parse(text);
            UIArgs = new UIEventArgs(index);
            UIArgs.SetID((int)eUITooltipType);
        }
        else
            return;

        UIArgs.SetVector2D(pivot);
        OnUpdateTooltipEvent?.Invoke(UIArgs); 
    }

    public void UpdateSpeciesType(SpeciesType eSpeciesType, AttributeType eAttributeType)
    {
        OnChangeSpeciesTypeUIEvent?.Invoke(eSpeciesType, eAttributeType);
    }

    public void ShowNoticeText(string strText, bool bSound)
    {
        OnNoticeEvent?.Invoke(strText, bSound);
    }
    public void ShowRoundPeriodic(int stage)
    {
        OnRoundPeriodic?.Invoke(stage);
    }

    public void ShowRank()
    {
        OnRankEvent?.Invoke();
    }

    public void StartGame()
    {
        OnChangeStartGame?.Invoke();
    }

    public void ChangeTooltipSortType()
    {
        OnChangeTooltipSortType?.Invoke();
    }

    public void RefreshShopItem(int index)
    {
        OnRefreshShopItem?.Invoke(index);
    }
    public void SellShopItem(int index)
    {
        OnSellShopItem?.Invoke(index);
    }

    protected override void OnDestroy()
    {
        UIList.Clear();
        SpriteDictionary.Clear();
        EtcSpriteDictionary.Clear();
        UIPrefebDictionary.Clear();
        UIBuffDictionary.Clear();

        for (int i = 0; i < SpriteArray.Length; ++i)
        {
            SpriteArray[i] = null;
        }

        for (int i = 0; i < UIPrefeb.Length; ++i)
        {
            UIPrefeb[i] = null;
        }

        for (int i = 0; i < UIBuffArray.Length; ++i)
        {
            UIBuffArray[i] = null;
        }
    }

    public GameObject GetPopup(Transform parent)
    {
        if (PopUpObject == null)
            return null;

        var instance = Instantiate(PopUpObject, parent);
        if (instance != null)
        {
            bShowDialog = true;
        }

        return instance;
    }
}
