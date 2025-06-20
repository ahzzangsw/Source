using GameDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class WeaponPool : Singleton<WeaponPool>
{
    //private List<IObjectPool<WeaponBase>> poolsList;
    private Dictionary<WeaponType, IObjectPool<WeaponBase>> poolsList;

    private GameObject WeaponPrefab = null;
    private Vector3 WeaponPrefabPosition = Vector3.zero;

    protected override void Awake()
    {
        //poolsList = new List<IObjectPool<WeaponBase>>();
        //for (int i = 0; i < (int)WeaponType.MAX; ++i)
        //{
        //    ObjectPool<WeaponBase> pool = new ObjectPool<WeaponBase>(CreateWeapon, OnGetWeapon, OnReleaseWeapon, OnDestroyWeapon, maxSize: 200);
        //    poolsList.Add(pool);
        //}

        int WeaponMaxSize = 100;
        if (Oracle.m_eGameType == MapType.ADVENTURE)
        {
            WeaponMaxSize = 200;
        }

        poolsList = new Dictionary<WeaponType, IObjectPool<WeaponBase>>();
        for (int i = 0; i < (int)WeaponType.MAX; ++i)
        {
            ObjectPool<WeaponBase> pool = new ObjectPool<WeaponBase>(CreateWeapon, OnGetWeapon, OnReleaseWeapon, OnDestroyWeapon, maxSize: WeaponMaxSize);
            poolsList.Add((WeaponType)i, pool);
        }
    }

    private WeaponBase CreateWeapon()
    {
        if (WeaponPrefab == null)
            return null;

        GameObject WeaponClone = Instantiate(WeaponPrefab, WeaponPrefabPosition, Quaternion.identity);
        if (WeaponClone == null)
            return null;

        WeaponBase weaponBase = WeaponClone.GetComponent<WeaponBase>();
        if (weaponBase)
            //weaponBase.SetManagedPool(poolsList[(int)(weaponBase.m_eWeaponType)]);
            weaponBase.SetManagedPool(poolsList[weaponBase.m_eWeaponType]);

        WeaponPrefab = null;
        WeaponPrefabPosition = Vector3.zero;
        return weaponBase;
    }
    private void OnGetWeapon(WeaponBase weapon)
    {
        if (WeaponPrefabPosition != Vector3.zero)
            weapon.gameObject.transform.position = WeaponPrefabPosition;
        weapon.gameObject.SetActive(true);
        WeaponPrefabPosition = Vector3.zero;
    }
    private void OnReleaseWeapon(WeaponBase weapon)
    {
        weapon.gameObject.SetActive(false);
    }
    private void OnDestroyWeapon(WeaponBase weapon)
    {
        Destroy(weapon.gameObject);
    }

    public WeaponBase GetWeapon(WeaponType eWeaponType, Vector3 StartPosition)
    {
        WeaponPrefab = ResourceAgent.Instance.GetWeaponPrefab(eWeaponType);
        WeaponPrefabPosition = StartPosition;
        //return poolsList[(int)eWeaponType].Get();
        return poolsList[eWeaponType].Get();
    }
}
