using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ScrollList_Component : MonoBehaviour, IScrollHandler
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float scrollSpeedMultiplier = 1.0f;

    [SerializeField] GameObject itemCover;

    private List<Transform> m_ChildList;
    private List<GameObject> m_ItemCoverList;

    public void Generate(GameObject cell, int totalCount)
    {
        
    }

    private void Awake()
    {
        m_ItemCoverList = new List<GameObject>();
        m_ChildList = new List<Transform>();

        RectTransform content = GetComponent<ScrollRect>().content;
        for (int i = 0; i < content.childCount; i++)
        {
            Transform child = content.GetChild(i);
            Button item = child.GetComponent<Button>();
            if (item == null)
                continue;

            int index = i;
            if (itemCover != null)
            {
                bool bHas = UnlockManager.Instance.CheckShopProductItem(index);
                if (bHas)
                {
                    m_ItemCoverList.Add(Instantiate(itemCover, child));
                }
            }

            item.onClick.AddListener(() => OnClickProductItem(index));

            m_ChildList.Add(child);
        }

        UIManager.Instance.OnSellShopItem += HandleSellShopItem;
    }

    public void OnScroll(PointerEventData eventData)
    {
        float scrollDelta = eventData.scrollDelta.y * scrollSpeedMultiplier;
        Vector2 scrollPosition = scrollRect.normalizedPosition;

        scrollPosition.y += scrollDelta;
        scrollPosition.y = Mathf.Clamp01(scrollPosition.y);
        scrollRect.normalizedPosition = scrollPosition;
    }

    private void OnClickProductItem(int index)
    {
        UIManager.Instance.RefreshShopItem(index);
    }

    private void HandleSellShopItem(int index)
    {
        if (index < 0 || index >= m_ChildList.Count)
            return;

        if (UnlockManager.Instance.CheckShopProductItem(index))
        {
            m_ItemCoverList.Add(Instantiate(itemCover, m_ChildList[index]));
        }
    }

    private void OnDestroy()
    {
        for (int i = m_ItemCoverList.Count - 1; i >= 0; --i)
        {
            Destroy(m_ItemCoverList[i]);
            m_ItemCoverList[i] = null;
            m_ItemCoverList.RemoveAt(i);
        }
    }
}
