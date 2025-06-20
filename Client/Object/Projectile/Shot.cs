using GameDefines;
using CharacterDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : Projectile
{
    private Transform m_TargetTransform = null;
    void Awake()
    {
        eProjectileType = ProjectileType.SHOT;
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
                        if (closestDistSqr > distance)
                        {
                            m_TargetTransform = monsterObject.transform;
                            closestDistSqr = distance;
                        }
                    }
                }

                if (closestDistSqr != Mathf.Infinity)
                {
                    ChangeState(BuildingActionState.Attack);
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

            float fAttackCountPerSecond = 1f / m_Master.AttackSpeed;
            if (fAttackCountPerSecond == 0)
            {
                ChangeState(BuildingActionState.Search);
                yield return null;
            }

            Fire(m_TargetTransform, true);
            yield return new WaitForSeconds(fAttackCountPerSecond);
        }
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
                if (closestDistSqr > distance)
                {
                    m_TargetTransform = monsterObject.transform;
                    closestDistSqr = distance;
                }
            }
        }

        if (closestDistSqr != Mathf.Infinity)
        {
            if (m_TargetTransform == null)
                yield break;

            isCoroutineRunning = true;
            Fire(m_TargetTransform, true);

            float fAttackCountPerSecond = 1f / m_Master.AttackSpeed;
            yield return new WaitForSeconds(fAttackCountPerSecond);
        }

        isCoroutineRunning = false;
    }
}
