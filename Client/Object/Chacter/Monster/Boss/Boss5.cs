using GameDefines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UIDefines;
using UnityEditor;
using UnityEngine;

public class Boss5 : BossBase
{
    private int currentSplitCount = 10;
    private float splitScaleOffset = 0.85f;

    protected override void Awake()
    {
        base.Awake();
        iBossSkillPercent = 0;
        RubyCount = 5;
    }

    protected override void OnDie(bool beKilled)
    {
        if (currentSplitCount == 0 || beKilled == false)
        {
            MonsterPool.Instance.ReserveRemoveBoss(gameObject);
            base.OnDie(beKilled);
            return;
        }

        --currentSplitCount;
        //Boss5 splitBoss = MonsterPool.Instance.AddBossInfo(prefabIndex, 5) as Boss5;
        //if (splitBoss == null)
        //{
        //    Debug.Log("Split Boss - Count = " + currentSplitCount);
        //    return;
        //}

        //splitBoss.transform.position = transform.position;
        SetSplitInfo(this);

        //CameraManager.Instance.bProductioning = false;
        //gameObject.SetActive(false);

        //MonsterPool.Instance.ReserveRemoveBoss(gameObject);
    }

    void SetSplitInfo(Boss5 originBoss)
    {
        m_eSpeciesType = originBoss.m_eSpeciesType;
        m_eAttributeType = originBoss.m_eAttributeType;

        currentSplitCount = originBoss.currentSplitCount;
        int tempHP = originBoss.Hp / 2;
        if(currentSplitCount > 5)
            tempHP += 2000;

        SetComponent(originBoss);
        SetInfo(originBoss.ID + 1, tempHP, originBoss.Defense, originBoss.moveSpeed, originBoss.giveMoney);
        currentHP = tempHP;
        moveIndex = originBoss.moveIndex;
        prefabIndex = originBoss.prefabIndex;
        SetDropItem(originBoss.m_dropItem);

        transform.localScale = originBoss.transform.localScale;
        transform.localScale *= splitScaleOffset;

        bProductioning = false;
        bEnabled = true;
    }
}
