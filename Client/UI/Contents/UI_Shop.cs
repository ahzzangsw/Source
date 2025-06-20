using GameDefines;
using OptionDefines;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Shop : UIBase
{
    [SerializeField] private GameObject m_PreviewPanel;
    [SerializeField] private Button m_SellBtn;
    [SerializeField] private Button m_CompleteBtn;
    [SerializeField] private Button m_ExitBtn;
    [SerializeField] private Text m_RubyText;
    [SerializeField] private Image m_SelecteOnlySpriteImage;
    [SerializeField] private Sprite[] m_SelecteSprite;

    private int m_iSelectedIndex = -1;
    private GameObject[] m_PreviewObject = { null, null, null, null, null };

    protected override void Awake()
    {
        UIManager.Instance.OnRefreshShopItem += HandleRefreshShopItem;
        UIManager.Instance.OnSellShopItem += HandleSellShopItem;

        m_SellBtn.onClick.AddListener(OnClickSell);
        m_ExitBtn.onClick.AddListener(OnClickExit);

        m_SelecteOnlySpriteImage.enabled = false;
    }

    void Start()
    {
        SoundManager.Instance.PlayBGM(BGMType.SHOP);

        UpdateRuby();
    }

    private void OnDestroy()
    {
        m_SellBtn.onClick.RemoveAllListeners();
    }

    private void ChangeButton(bool bComplete)
    {
        if (m_SellBtn == null || m_CompleteBtn == null)
            return;

        m_SellBtn.gameObject.SetActive(!bComplete);
        m_CompleteBtn.gameObject.SetActive(bComplete);
    }

    private void OnClickSell()
    {
        if (m_iSelectedIndex < 0)
            return;

        UnlockManager.Instance.SellShopProductItem(m_iSelectedIndex);
    }

    private void HandleRefreshShopItem(int index)
    {
        if (m_iSelectedIndex == index)
            return;

        m_iSelectedIndex = index;
        SetPreview();
    }

    private void HandleSellShopItem(int index)
    {
        UpdateRuby();
    }

    private void SetPreview()
    {
        m_SelecteOnlySpriteImage.enabled = false;
        for (int i = 0; i < m_PreviewObject.Length; ++i)
        {
            if (m_PreviewObject[i] != null)
            {
                Destroy(m_PreviewObject[i]);
                m_PreviewObject[i] = null;
            }
        }

        ShopInfo shopInfo = UnlockManager.Instance.GetShopProductItemInfo(m_iSelectedIndex);
        if (shopInfo.eSpeciesType == SpeciesType.NONE)
        {
            Debug.Log("SetPreview SpeciesType error = " + m_iSelectedIndex);
        }

        bool b = false;
        float distance = 1f;
        if (shopInfo.eSpeciesType != SpeciesType.MAX)
        {
            for (int i = 4; i >= 0; --i)
            {
                GameObject prefab = ResourceAgent.Instance.GetPrefab(shopInfo.eSpeciesType, i);
                if (prefab == null)
                    continue;

                m_PreviewObject[i] = Instantiate(prefab, m_PreviewPanel.transform);
                if (m_PreviewObject[i] == null)
                    continue;

                Character tempCharacterInfo = m_PreviewObject[i].GetComponent<Character>();
                if (tempCharacterInfo != null)
                {
                    tempCharacterInfo.ChangeRanderOrder(11);
                    m_PreviewObject[i].transform.localScale *= 100;
                    if (i > 0)
                    {
                        Vector3 newPosition = m_PreviewObject[i].transform.position;
                        m_PreviewObject[i].transform.position = new Vector3(newPosition.x + (b ? distance : -distance), newPosition.y, newPosition.z);

                        b = !b;
                        if (!b)
                            distance /= 2f;
                    }
                }
            }
        }
        else
        {
            if (shopInfo.SlotId >= 0 && shopInfo.SlotId < m_SelecteSprite.Length)
            {
                m_SelecteOnlySpriteImage.sprite = m_SelecteSprite[shopInfo.SlotId];
                m_SelecteOnlySpriteImage.enabled = true;
            }
        }

        bool bHas = UnlockManager.Instance.CheckShopProductItem(m_iSelectedIndex);
        ChangeButton(bHas);
    }

    private void UpdateRuby()
    {
        m_RubyText.text = GameManager.Instance.gameMoney.ToString();
    }

    private void OnClickExit()
    {
        SoundManager.Instance.PlayUISound(UISoundType.BACK);
        SceneManager.LoadScene("LobbyScene");
    }
}
