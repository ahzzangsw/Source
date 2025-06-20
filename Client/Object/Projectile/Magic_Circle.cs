using GameDefines;
using CharacterDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic_Circle : Projectile
{
    [SerializeField] public MagicType eMagicType = MagicType.NONE;

    private Transform m_TargetTransform = null;
    private List<Transform> m_TargetTransformList = null;

    private Vector2 m_Endpoint = Vector2.zero;

    void Awake()
    {
        eProjectileType = ProjectileType.MAGIC;
        m_TargetTransformList = new List<Transform>();
    }

    protected override IEnumerator Search()
    {
        while (true)
        {
            List<GameObject> MonsterList = MonsterPool.Instance.GetMonsters();
            if (MonsterList != null)
            {
                float closestDistSqr = Mathf.Infinity;
                if (eMagicType == MagicType.ONETIME_DOWN)
                {
                    for (int i = 0; i < MonsterList.Count; ++i)
                    {
                        GameObject monsterObject = MonsterList[i];
                        if (CheckTarget(monsterObject) == false)
                            continue;

                        float distance = Vector3.Distance(monsterObject.transform.position, m_MuzzlePosition);
                        if (distance <= m_Master.Range)
                        {
                            m_TargetTransformList.Add(monsterObject.transform);
                        }
                    }

                    if (m_TargetTransformList.Count > 0)
                    {
                        ChangeState(BuildingActionState.Attack);
                    }
                }
                else
                {
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
            }

            yield return null;
        }
    }

    protected override IEnumerator Attack()
    {
        while (true)
        {
            if (eMagicType == MagicType.ONETIME_DOWN)
            {
                bool bChange = true;
                for (int i = 0; i < m_TargetTransformList.Count; ++i)
                {
                    if (CheckTarget(m_TargetTransformList[i]) == false)
                        continue;

                    Fire(m_TargetTransformList[i], bChange);
                    bChange = false;
                }

                m_TargetTransformList.Clear();
                if (bChange)
                {
                    ChangeState(BuildingActionState.Search);
                    yield return null;
                }
            }
            else
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

                if (eMagicType == MagicType.BOUNCE)
                {
                    Fire(m_TargetTransform, true, eMagicType);
                }
                else
                {
                    Fire(m_TargetTransform.position, true, eMagicType);
                }
            }

            float fAttackCountPerSecond = 1f / m_Master.AttackSpeed;
            if (fAttackCountPerSecond == 0)
            {
                ChangeState(BuildingActionState.Search);
                m_TargetTransformList.Clear();
                yield return null;
            }

            yield return new WaitForSeconds(fAttackCountPerSecond);
        }
    }

    private bool CheckTarget(Transform transform)
    {
        if (transform == null)
            return false;

        if (CheckTarget(transform.gameObject) == false)
            return false;

        float distance = Vector3.Distance(transform.position, m_MuzzlePosition);
        if (distance > m_Master.Range)
            return false;

        return true;
    }

    protected override IEnumerator SearchAndAttack()
    {
        List<GameObject> MonsterList = MonsterPool.Instance.GetMonsters();
        if (MonsterList == null)
            yield break;

        if (m_Master is Player_Adventure == false)
            yield break;

        //Search
        if (eMagicType == MagicType.ONETIME_DOWN)
        {
            for (int i = 0; i < MonsterList.Count; ++i)
            {
                GameObject monsterObject = MonsterList[i];
                if (CheckTarget(monsterObject) == false)
                    continue;

                float distance = Vector3.Distance(monsterObject.transform.position, m_MuzzlePosition);
                if (distance <= m_Master.Range)
                {
                    m_TargetTransformList.Add(monsterObject.transform);
                }
            }

            if (m_TargetTransformList.Count == 0)
                yield break;
        }
        else
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

            if (closestDistSqr == Mathf.Infinity)
                yield break;
        }

        //Attack
        bool bDelay = false;
        if (eMagicType == MagicType.ONETIME_DOWN)
        {
            bool bChange = true;
            isCoroutineRunning = true;
            for (int i = 0; i < m_TargetTransformList.Count; ++i)
            {
                Fire(m_TargetTransformList[i], bChange);
                bChange = false;
            }

            m_TargetTransformList.Clear();
            bDelay = !bChange;
        }
        else
        {
            if (m_TargetTransform != null)
            {
                bDelay = true;
                isCoroutineRunning = true;
                if (eMagicType == MagicType.BOUNCE)
                {
                    Fire(m_TargetTransform, true, eMagicType);
                }
                else
                {
                    Fire(m_TargetTransform.position, true, eMagicType);
                }
            }
        }

        if (bDelay)
        {
            float fAttackCountPerSecond = 1f / m_Master.AttackSpeed;
            yield return new WaitForSeconds(fAttackCountPerSecond);
        }

        isCoroutineRunning = false;
    }
}
