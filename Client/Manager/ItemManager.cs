using GameDefines;
using OptionDefines;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ItemManager : Singleton<ItemManager>
{
    [SerializeField] public GameObject[] ItemPrefabsArray;
    private Dictionary<ItemType, GameObject> ItemPrefabDictionary;

    private List<IObjectPool<ItemBase>> poolsList;
    private GameObject ItemPrefab = null;
    private Vector3 ItemPrefabPosition = Vector3.zero;

    protected override void Awake()
    {
        poolsList = new List<IObjectPool<ItemBase>>();
        for (int i = 0; i < (int)ItemType.MAX; ++i)
        {
            ObjectPool<ItemBase> pool = new ObjectPool<ItemBase>(CreateItem, OnGetItem, OnReleaseItem, OnDestroyItem, maxSize: 50);
            poolsList.Add(pool);
        }

        ItemPrefabDictionary = new Dictionary<ItemType, GameObject>();
        foreach (GameObject prefab in ItemPrefabsArray)
        {
            if (prefab == null)
                continue;

            ItemBase item = prefab.GetComponent<ItemBase>();
            if (item == null)
                continue;

            if (item.m_eItemType == ItemType.NONE)
                continue;

            ItemPrefabDictionary.Add(item.m_eItemType, prefab);
        }
    }
    private ItemBase CreateItem()
    {
        if (ItemPrefab == null)
            return null;

        GameObject ItemClone = Instantiate(ItemPrefab, ItemPrefabPosition, Quaternion.identity);
        if (ItemClone == null)
            return null;

        ItemBase ItemBase = ItemClone.GetComponent<ItemBase>();
        if (ItemBase)
            ItemBase.SetManagedPool(poolsList[(int)(ItemBase.m_eItemType)]);

        ItemPrefab = null;
        ItemPrefabPosition = Vector3.zero;
        return ItemBase;
    }
    private void OnGetItem(ItemBase item)
    {
        if (ItemPrefabPosition != Vector3.zero)
            item.gameObject.transform.position = ItemPrefabPosition;
        item.gameObject.SetActive(true);
        ItemPrefabPosition = Vector3.zero;
    }
    private void OnReleaseItem(ItemBase item)
    {
        item.gameObject.SetActive(false);
    }
    private void OnDestroyItem(ItemBase item)
    {
        Destroy(item.gameObject);
    }
    public ItemBase GetItem(ItemType eItemType, Vector3 StartPosition)
    {
        ItemPrefab = GetItemPrefab(eItemType);
        ItemPrefabPosition = StartPosition;
        return poolsList[(int)eItemType].Get();
    }
    public GameObject GetItemPrefab(ItemType eItemType)
    {
        if (ItemPrefabDictionary.ContainsKey(eItemType) == false)
            return null;
        return ItemPrefabDictionary[eItemType];
    }
    public void DropItem(ItemType eItemType, Vector3 vPosition, Vector3 vStartPosition)
    {
        ItemBase item = GetItem(eItemType, vPosition);
        if (item == null)
            return;

        Vector3 vLook = (vPosition - vStartPosition).normalized;
        item.SetFly(vLook, vStartPosition.y);
        item.Appear();
    }
    public ItemBase MakeItem(ItemType eItemType, Vector3 vPosition)
    {
        ItemBase item = GetItem(eItemType, vPosition);
        if (item == null)
            return null;

        return item;
    }
    public void AutoRubyGet(int RubyCountInField)
    {
        GameManager.Instance.AddGameMoney(RubyCountInField);
        SoundManager.Instance.PlayUISound(UISoundType.PICKUPITEM);
    }
}