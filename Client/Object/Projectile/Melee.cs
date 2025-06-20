using CharacterDefines;
using GameDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Projectile
{
    //[SerializeField] private MeleeType m_eMeleeType = MeleeType.NONE;
    void Awake()
    {
        eProjectileType = ProjectileType.NONE;
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
                List<Transform> targetTransform = new List<Transform>();
                for (int i = 0; i < MonsterList.Count; ++i)
                {
                    GameObject monsterObject = MonsterList[i];
                    if (CheckTarget(monsterObject) == false)
                        continue;

                    float distance = Vector3.Distance(monsterObject.transform.position, m_MuzzlePosition);
                    if (distance <= m_Master.Range)
                    {
                        targetTransform.Add(monsterObject.transform);
                        ++targetIndex;
                        if (!skipCollision && targetIndex >= m_Master.TargetCount)
                            break;
                    }
                }

                if (targetIndex > 0)
                {
                    fAttackCountPerSecond = 1f / m_Master.AttackSpeed;

                    for (int i = targetIndex; i > 0; --i)
                    {
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
    protected override void Fire(Transform target, bool bChange, MagicType eMagicType = MagicType.NONE)
    {
        if (bChange)
        {
            vLookVector = (target.position - m_MuzzlePosition).normalized;
            if (Oracle.m_eGameType != MapType.ADVENTURE)
                m_Master.Turn(vLookVector.x);
            m_Master.SetAnimationActionState(LAnimationState.Attack);
            m_Master.WeaponFlashSound(false, true);
            //SetMeleeEffect(m_eMeleeType);
        }

        MonsterBase hitMonster = target.GetComponent<MonsterBase>();
        m_Master.HitMonster(hitMonster, HitParticleType.NONE);
    }
    public void SetMeleeEffect(MeleeType eMeleeType)
    {
        MotionTrail pMotionTrail = EffectPool.Instance.GetMotionTrail(m_Master);
        if (pMotionTrail != null)
        {
            Vector3 MotionTrailPos = m_Master.GetComponent<Transform>().position;
            pMotionTrail.SetMotionTrail(eMeleeType, vLookVector, MotionTrailPos);
        }
    }

    protected override IEnumerator SearchAndAttack()
    {
        List<GameObject> MonsterList = MonsterPool.Instance.GetMonsters();
        if (MonsterList == null)
            yield break;

        int targetIndex = 0;
        List<Transform> targetTransform = new List<Transform>();
        for (int i = 0; i < MonsterList.Count; ++i)
        {
            GameObject monsterObject = MonsterList[i];
            if (CheckTarget(monsterObject) == false)
                continue;

            float distance = Vector3.Distance(monsterObject.transform.position, m_MuzzlePosition);
            if (distance <= m_Master.Range)
            {
                targetTransform.Add(monsterObject.transform);
                ++targetIndex;
                if (!skipCollision && targetIndex >= m_Master.TargetCount)
                    break;
            }
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
