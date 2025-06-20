using CharacterDefines;
using GameDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OptionDefines;

public class Dot : Projectile
{
    [SerializeField] private bool isCollision = true;

    private Transform m_TargetTransform = null;

    private float fAttackCountPerSecond = 0f;
    void Awake()
    {
        eProjectileType = ProjectileType.DOT;
    }

    protected override IEnumerator Search()
    {
        while (true)
        {
            List<GameObject> MonsterList = MonsterPool.Instance.GetMonsters();
            if (MonsterList != null)
            {
                float closestDistSqr = Mathf.Infinity;
                for (int i = 0; i < MonsterList.Count; ++i)
                {
                    GameObject monsterObject = MonsterList[i];
                    if (CheckTarget(monsterObject) == false)
                        continue;

                    float distance = Vector3.Distance(monsterObject.transform.position, m_MuzzlePosition);
                    if (distance <= m_Master.Range)
                    {
                        m_TargetTransform = monsterObject.transform;
                        closestDistSqr = distance;
                    }
                }

                if (closestDistSqr != Mathf.Infinity)
                {
                    if (isCollision)
                    {
                        //ChangeState(BuildingActionState.Ready);
                    }
                    else
                    {
                        ChangeState(BuildingActionState.Attack);
                    }
                }
            }

            yield return null;
        }
    }

    protected override IEnumerator Attack()
    {
        while (true)
        {
            if (m_TargetTransform == null)
            {
                ChangeState(BuildingActionState.Search);
                yield return null;
            }

            if (CheckTarget(m_TargetTransform.gameObject) == false)
            {
                ChangeState(BuildingActionState.Search);
                yield return null;
            }

            float distance = Vector3.Distance(m_TargetTransform.position, m_MuzzlePosition);
            if (distance > m_Master.Range)
            {
                ChangeState(BuildingActionState.Search);
                yield return null;
            }

            fAttackCountPerSecond = 1f / m_Master.AttackSpeed;
            Fire(m_TargetTransform, true);

            yield return new WaitForSeconds(fAttackCountPerSecond);
        }
    }

    protected override IEnumerator Ready()
    {
        yield return null;
    }

    protected override void Fire(Transform target, bool bChange, MagicType eMagicType = MagicType.NONE)
    {
        if (bChange)
        {
            vLookVector = (target.position - m_MuzzlePosition).normalized;
            if (Oracle.m_eGameType != MapType.ADVENTURE)
                m_Master.Turn(vLookVector.x);
            m_Master.WeaponFlashSound(false);
        }

        MonsterBase hitMonster = target.GetComponent<MonsterBase>();
        m_Master.HitMonster(hitMonster, HitParticleType.BE_BITTEN);
    }

    protected override IEnumerator SearchAndAttack()
    {
        List<GameObject> MonsterList = MonsterPool.Instance.GetMonsters();
        if (MonsterList == null)
            yield break;

        float closestDistSqr = Mathf.Infinity;
        for (int i = 0; i < MonsterList.Count; ++i)
        {
            GameObject monsterObject = MonsterList[i];
            if (CheckTarget(monsterObject) == false)
                continue;

            float distance = Vector3.Distance(monsterObject.transform.position, m_MuzzlePosition);
            if (distance <= m_Master.Range)
            {
                m_TargetTransform = monsterObject.transform;
                closestDistSqr = distance;
            }
        }

        if (closestDistSqr != Mathf.Infinity)
        {
            if (m_TargetTransform == null)
                yield break;

            isCoroutineRunning = true;
            if (isCollision)
            {
                //ChangeState(BuildingActionState.Ready);
            }
            else
            {
                Fire(m_TargetTransform, true);
            }

            float fAttackCountPerSecond = 1f / m_Master.AttackSpeed;
            yield return new WaitForSeconds(fAttackCountPerSecond);
        }

        isCoroutineRunning = false;
    }
}

