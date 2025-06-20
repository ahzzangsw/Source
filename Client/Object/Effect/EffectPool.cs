using GameDefines;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EffectPool : Singleton<EffectPool>
{
    private List<IObjectPool<EffectBase>> mainPoolsList;
    private ObjectPool<MotionTrail> motionTrailPools;

    private GameObject EffectPrefab = null;
    private Vector3 EffectPos = Vector3.zero;
    private Vector3 MotionTrailPos = Vector3.zero;

    [SerializeField] private GameObject[] EffectList;

    protected override void Awake()
    {
        mainPoolsList = new List<IObjectPool<EffectBase>>();
        for (int i = 1; i < (int)WeaponType.MAX; ++i)
        {
            ObjectPool<EffectBase> pool = new ObjectPool<EffectBase>(CreateEffect, OnGetEffect, OnReleaseEffect, OnDestroyEffect, maxSize: 100);
            mainPoolsList.Add(pool);
        }

        motionTrailPools = new ObjectPool<MotionTrail>(CreateMotionTrail, OnGetMotionTrail, OnReleaseMotionTrail, OnDestroyMotionTrail, maxSize: 80);
    }

    private EffectBase CreateEffect()
    {
        if (EffectPrefab == null)
            return null;

        GameObject EffectClone = Instantiate(EffectPrefab, EffectPos, Quaternion.identity);
        if (EffectClone == null)
            return null;

        EffectBase effectBase = EffectClone.GetComponent<EffectBase>();
        if (effectBase)
            effectBase.SetManagedPool(mainPoolsList[(int)effectBase.eEffectType]);

        EffectPrefab = null;
        return effectBase;
    }
    private void OnGetEffect(EffectBase Effect)
    {
        Effect.transform.position = EffectPos;
        Effect.gameObject.SetActive(true);
    }
    private void OnReleaseEffect(EffectBase Effect)
    {
        Effect.gameObject.SetActive(false);
    }
    private void OnDestroyEffect(EffectBase Effect)
    {
        Destroy(Effect.gameObject);
    }
    public EffectBase GetEffect(Vector3 transPos, GameObject prefab)
    {
        EffectBase effectBase = prefab.GetComponent<EffectBase>();
        if (effectBase == null)
            return null;

        EffectPos = transPos;
        EffectPrefab = prefab;

        return mainPoolsList[(int)effectBase.eEffectType].Get();
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private MotionTrail CreateMotionTrail()
    {
        GameObject MotionTrailPrefab = Instantiate(ResourceAgent.Instance.GetWeaponPrefab(WeaponType.NONE), MotionTrailPos, Quaternion.identity);
        if (MotionTrailPrefab == null)
            return null;

        MotionTrail motionTrail = MotionTrailPrefab.GetComponent<MotionTrail>();
        if (motionTrail)
            motionTrail.SetManagedPool(motionTrailPools);

        return motionTrail;
    }
    private void OnGetMotionTrail(MotionTrail motionTrail)
    {
        motionTrail.gameObject.SetActive(true);
    }
    private void OnReleaseMotionTrail(MotionTrail motionTrail)
    {
        motionTrail.gameObject.SetActive(false);
    }
    private void OnDestroyMotionTrail(MotionTrail motionTrail)
    {
        Destroy(motionTrail.gameObject);
    }
    public MotionTrail GetMotionTrail(Building master)
    {
        if (master == null)
            return null;

        MotionTrailPos = master.GetComponent<Transform>().position;
        return motionTrailPools.Get();
    }

    public GameObject GetEffectPrefab(EffectType eEffectType)
    {
        if (EffectList == null)
            return null;

        int index = (int)eEffectType;
        if (index >= (int)EffectType.MAX || index >= EffectList.Length)
            return null;

        return EffectList[index];
    }
}
