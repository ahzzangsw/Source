using GameDefines;
using UIDefines;
using OptionDefines;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_CharacterDecks : UIBase
{
    [SerializeField] private int m_PickCount;
    [SerializeField] private GameObject m_MainPanel;
    [SerializeField] private Button[] m_Decks;
    [SerializeField] private Button m_GOButton;

    [Header("Initialize")]
    [SerializeField] private Vector2 startPos;
    [SerializeField] private Vector3[] arrivalPoint;

    private List<SpeciesType> RandomSpeciesTypeList;
    private int m_iSelectCount;

    public override void SetControlInfo()
    {
        base.SetControlInfo();
        if (RandomSpeciesTypeList == null)
            RandomSpeciesTypeList = new List<SpeciesType>();
            
        RandomSpeciesTypeList.Clear();
        List<SpeciesType> useableSpeciesTypeList = UnlockManager.Instance.GetUseableSpeciesTypeList();
        if (useableSpeciesTypeList == null)
            return;

        if (useableSpeciesTypeList.Count == 0)
            return;

        for (int i = 0; i < m_Decks.Length; ++i)
        {
            int index = Oracle.RandomDice(0, useableSpeciesTypeList.Count);
            SpeciesType eType = useableSpeciesTypeList[index];

            RandomSpeciesTypeList.Add(eType);
            useableSpeciesTypeList.RemoveAt(index);
        }

        for (int i = 0; i < m_Decks.Length; ++i)
        {
            CharacterDeck deck = m_Decks[i].GetComponent<CharacterDeck>();
            if (deck == null)
                continue;

            RectTransform rectTransform = deck.GetComponent<RectTransform>();
            if (rectTransform)
            {
                rectTransform.anchoredPosition = startPos;
                deck.SetSpeciesType(RandomSpeciesTypeList[i], m_PickCount);
            }
        }

        int btnIndex = 0;
        if (m_Decks[btnIndex] != null)
        {
            m_Decks[btnIndex].onClick.AddListener(OnClick_Deck0);
            ++btnIndex;
        }
        if (m_Decks[btnIndex] != null)
        {
            m_Decks[btnIndex].onClick.AddListener(OnClick_Deck1);
            ++btnIndex;
        }
        if (m_Decks[btnIndex] != null)
        {
            m_Decks[btnIndex].onClick.AddListener(OnClick_Deck2);
            ++btnIndex;
        }
        if (m_Decks[btnIndex] != null)
        {
            m_Decks[btnIndex].onClick.AddListener(OnClick_Deck3);
            ++btnIndex;
        }
        if (m_Decks[btnIndex] != null)
        {
            m_Decks[btnIndex].onClick.AddListener(OnClick_Deck4);
            ++btnIndex;
        }
        if (m_Decks[btnIndex] != null)
        {
            m_Decks[btnIndex].onClick.AddListener(OnClick_Deck5);
            ++btnIndex;
        }
        if (m_Decks[btnIndex] != null)
        {
            m_Decks[btnIndex].onClick.AddListener(OnClick_Deck6);
            ++btnIndex;
        }
        if (m_Decks[btnIndex] != null)
        {
            m_Decks[btnIndex].onClick.AddListener(OnClick_Deck7);
            ++btnIndex;
        }
        if (m_Decks[btnIndex] != null)
        {
            m_Decks[btnIndex].onClick.AddListener(OnClick_Deck8);
            ++btnIndex;
        }
        if (m_Decks[btnIndex] != null)
        {
            m_Decks[btnIndex].onClick.AddListener(OnClick_Deck9);
            ++btnIndex;
        }

        if (m_GOButton != null)
        {
            m_GOButton.interactable = false;
            m_GOButton.onClick.AddListener(OnClick_GO);
        }
    }

    private void OnEnable()
    {
        StartGame();
    }

    private void StartGame()
    {
        if (arrivalPoint.Length != m_Decks.Length)
        {
            Debug.Log("UI_CharacterDecks - arrivalPoint.Length != m_Decks.Length error");
            return;
        }

        if (Oracle.PercentSuccess(50f))
        {
            SoundManager.Instance.PlayUISound(UISoundType.SHUFFLE);
        }
        else
        {
            SoundManager.Instance.PlayUISound(UISoundType.SHUFFLE1);
        }

        for (int i = 0; i < m_Decks.Length; ++i)
        {
            Button deck = m_Decks[i];
            if (deck == null)
                continue;

            RectTransform rectTransform = deck.GetComponent<RectTransform>();
            rectTransform.DOAnchorPos(arrivalPoint[i], 1f);
            rectTransform.DORotate(new Vector3(0f, 0f, 360f), 1f, RotateMode.FastBeyond360);
        }
    }
    private void OnClick_GO()
    {
        List<SpeciesType> selectedSpeciesTypeList = new List<SpeciesType>();
        for (int i = 0; i < m_Decks.Length; ++i)
        {
            CharacterDeck deck = m_Decks[i].GetComponent<CharacterDeck>();
            if (deck == null)
                continue;

            if (deck.selected)
                selectedSpeciesTypeList.Add(RandomSpeciesTypeList[i]);
        }

        BuildingPool.Instance.DeckComposition(selectedSpeciesTypeList);
        GameManager.Instance.ForceGameStart();

        SoundManager.Instance.PlayUISound(UISoundType.INGAMESTART);
        UIManager.Instance.HideUI(UIIndexType.CHARACTERDECK);
    }

    private void OnClick_Deck0()
    {
        SelectedDeck(0);
    }
    private void OnClick_Deck1()
    {
        SelectedDeck(1);
    }
    private void OnClick_Deck2()
    {
        SelectedDeck(2);
    }
    private void OnClick_Deck3()
    {
        SelectedDeck(3);
    }
    private void OnClick_Deck4()
    {
        SelectedDeck(4);
    }
    private void OnClick_Deck5()
    {
        SelectedDeck(5);
    }
    private void OnClick_Deck6()
    {
        SelectedDeck(6);
    }
    private void OnClick_Deck7()
    {
        SelectedDeck(7);
    }
    private void OnClick_Deck8()
    {
        SelectedDeck(8);
    }
    private void OnClick_Deck9()
    {
        SelectedDeck(9);
    }

    private void SelectedDeck(int index)
    {
        CharacterDeck deck = m_Decks[index].GetComponent<CharacterDeck>();
        if (deck == null)
            return;

        m_iSelectCount += deck.ToggleArrow(m_iSelectCount);

        if (m_GOButton)
        {
            bool bInteractable = false;
            if (m_iSelectCount == m_PickCount)
                bInteractable = true;

            m_GOButton.interactable = bInteractable;
        }
    }
}
