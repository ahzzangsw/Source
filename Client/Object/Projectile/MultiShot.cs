using GameDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiShot : Projectile
{
    void Awake()
    {
        eProjectileType = ProjectileType.MULTISHOT;
    }

    protected override IEnumerator Search()
    {
        while (true)
        {
            bool bChange = true;
            float fAttackCountPerSecond = 0;
            List<GameObject> MonsterList = MonsterPool.Instance.GetMonsters();
            if (MonsterList != null)
            {
                int targetIndex = 0;
                Transform[] targetTransform = { null, null, null, null, null };
                for (int i = 0; i < MonsterList.Count; ++i)
                {
                    GameObject monsterObject = MonsterList[i];
                    if (CheckTarget(monsterObject) == false)
                        continue;

                    float distance = Vector3.Distance(monsterObject.transform.position, m_MuzzlePosition);
                    if (distance <= m_Master.Range)
                    {
                        targetTransform[targetIndex++] = monsterObject.transform;
                    }

                    if (targetIndex >= m_Master.TargetCount)
                        break;
                }

                if (targetIndex > 0)
                {
                    fAttackCountPerSecond = 1f / m_Master.AttackSpeed;

                    for (int i = targetIndex; i > 0; --i)
                    {
                        if (targetTransform[i - 1] == null)
                            continue;

                        Fire(targetTransform[i - 1], bChange);
                        bChange = false;
                    }
                }
            }

            if (bChange)
                yield return null;
            else
                yield return new WaitForSeconds(fAttackCountPerSecond);
        }
    }

    protected override IEnumerator SearchAndAttack()
    {
        List<GameObject> MonsterList = MonsterPool.Instance.GetMonsters();
        if (MonsterList == null)
            yield break;
        
        int targetIndex = 0;
        Transform[] targetTransform = { null, null, null, null, null };
        for (int i = 0; i < MonsterList.Count; ++i)
        {
            GameObject monsterObject = MonsterList[i];
            if (CheckTarget(monsterObject) == false)
                continue;

            float distance = Vector3.Distance(monsterObject.transform.position, m_MuzzlePosition);
            if (distance <= m_Master.Range)
            {
                targetTransform[targetIndex++] = monsterObject.transform;
            }

            if (targetIndex >= m_Master.TargetCount)
                break;
        }

        if (targetIndex > 0)
        {
            bool bChange = true;
            isCoroutineRunning = true;
            for (int i = targetIndex; i > 0; --i)
            {
                if (targetTransform[i - 1] == null)
                    continue;

                Fire(targetTransform[i - 1], bChange);
                bChange = false;
            }

            float fAttackCountPerSecond = 1f / m_Master.AttackSpeed;
            yield return new WaitForSeconds(fAttackCountPerSecond);
        }

        isCoroutineRunning = false;
    }
}