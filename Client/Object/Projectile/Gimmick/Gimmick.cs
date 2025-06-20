using GameDefines;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Gimmick : Projectile
{
    [SerializeField] protected float MeleeAttackDelay = 0f;
    [SerializeField] protected float MeleeAttackDistance = 1f;

    protected PhaseStep m_ePhaseStep = PhaseStep.None;
    protected BossAdventure_Last_Skill m_OwnerBoss = null;
    protected Player_Adventure m_Target = null;
    protected AdventurePrefabsType m_eAdventurePrefabsType = AdventurePrefabsType.MAX;

    protected float FireSpeed = 1f;
    protected int FireCount = 1;
    protected bool bSkillEnd = false;
    protected List<Coroutine> CoroutineList = null;

    protected virtual void Clear()
    {
        m_ePhaseStep = PhaseStep.None;
    }

    protected virtual void Awake()
    {
        m_OwnerBoss = GetComponent<BossAdventure_Last_Skill>();
        m_Target = null;

        CoroutineList = new List<Coroutine>();
    }

    protected override void Update()
    {
        if (m_Target == null)
            return;

        ChangeMuzzlePosition();

        PhaseStep newPhaseStep = PhaseStep.None;
        if (m_ePhaseStep != newPhaseStep)
        {
            ChangeState(newPhaseStep);
        }

        if (bSkillEnd)
        {
            bSkillEnd = false;
            BossAdventure_Last_Manager.Instance.DestoryObjectpoolList(m_OwnerBoss);
            Destroy(this.gameObject);
        }
    }

    public void SetTarget(Player_Adventure target, int iLevel, AdventurePrefabsType eAdventurePrefabsType)
    {
        if (target == null)
        {
            Debug.Log("Gimmick error search not target");
            return;
        }

        Clear();
        m_Target = target;
        m_eAdventurePrefabsType = eAdventurePrefabsType;
        ChangeMuzzlePosition();

        PhaseStep ePhaseStep = PhaseStep.None;
        switch (iLevel)
        {
            case 0:
            case 1:
                ePhaseStep = PhaseStep.PhaseStep0;
                break;
            case 2:
                ePhaseStep = PhaseStep.PhaseStep1;
                break;
            case 3:
            case 4:
                ePhaseStep = PhaseStep.PhaseStep2;
                break;
        }

        ChangeState(ePhaseStep);
    }

    public void OnDie()
    {
        ChangeState(PhaseStep.None);
    }

    public void ForceChangeState(PhaseStep newPhaseStep)
    {
        ChangeState(newPhaseStep);
    }

    protected virtual void ChangeState(PhaseStep newPhaseStep)
    {
        StopCoroutine(m_ePhaseStep.ToString());
        foreach (Coroutine curCoroutine in CoroutineList)
        {
            StopCoroutine(curCoroutine);
        }
        CoroutineList.Clear();

        m_ePhaseStep = newPhaseStep;

        Clear();
        StartCoroutine(newPhaseStep.ToString());
    }

    protected override void StopState()
    {
        StopCoroutine(m_ePhaseStep.ToString());
        m_ePhaseStep = PhaseStep.None;
    }

    protected void ChangeMuzzlePosition()
    {
        switch (m_eAdventurePrefabsType)
        {
            case AdventurePrefabsType.GREENTROLL:
            case AdventurePrefabsType.BLUETROLL:
            case AdventurePrefabsType.BLACKTROLL:
                m_MuzzlePosition = transform.position;
                break;
            case AdventurePrefabsType.GREEENOGRE:
            case AdventurePrefabsType.REDOGRE:
            case AdventurePrefabsType.PURPLEOGRE:
                m_MuzzlePosition = transform.position + (Vector3.up * 2f);
                break;
            case AdventurePrefabsType.BROWNWEREWOLF:
            case AdventurePrefabsType.REDWEREVOLF:
            case AdventurePrefabsType.BLACKWEREWOLF:
            case AdventurePrefabsType.GREENREX:
            case AdventurePrefabsType.BLUEREX:
            case AdventurePrefabsType.REDREX:
            case AdventurePrefabsType.GREENMEGAPUMPKIN:
            case AdventurePrefabsType.PURPLEMEGAPUMPKIN:
            case AdventurePrefabsType.YELLOWMEGAPUMPKIN:
                m_MuzzlePosition = transform.position + Vector3.up;
                break;
        };
    }

    protected IEnumerator None() { yield break; }

    protected virtual IEnumerator PhaseStep0()
    {
        yield return null;
    }
    protected virtual IEnumerator PhaseStep1()
    {
        yield return null;
    }
    protected virtual IEnumerator PhaseStep2()
    {
        yield return null;
    }

    protected void SubAttackState(string strState)
    {
        CoroutineList.Add(StartCoroutine(strState));
    }
}
