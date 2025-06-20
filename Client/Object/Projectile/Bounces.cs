using GameDefines;
using CharacterDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Bounces : Projectile
{
    private Transform m_TargetTransform = null;
    private List<int> targetIDList;

    void Awake()
    {
        eProjectileType = ProjectileType.BOUNCES;
        targetIDList = new List<int>();
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
                        if (distance < closestDistSqr)
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

    public void NextFire(MonsterBase hitMonster, Building master, WeaponBase weapon)
    {
        if (master == null || hitMonster == null || weapon == null)
            return;

        if (weapon.bounceCount == 0)
            return;

        Transform targetTransform = null;
        List<GameObject> MonsterList = MonsterPool.Instance.GetMonsters();
        if (MonsterList != null)
        {
            float closestDistSqr = Mathf.Infinity;
            for (int i = 0; i < MonsterList.Count; ++i)
            {
                GameObject monsterObject = MonsterList[i];
                if (CheckTarget(monsterObject) == false)
                    continue;

                MonsterBase monsterBase = monsterObject.GetComponent<MonsterBase>();
                if (hitMonster.ID == monsterBase.ID)
                    continue;

                float distance = Vector3.Distance(monsterObject.transform.position, hitMonster.transform.position);
                if (distance <= m_Master.Range)
                {
                    if (distance < closestDistSqr)
                    {
                        closestDistSqr = distance;
                        targetTransform = monsterObject.transform;
                    }
                }
            }

            if (targetTransform == null)
                return;

            StartCoroutine(NextFireDelay(hitMonster, master, weapon, targetTransform));
        }
    }

    private IEnumerator NextFireDelay(MonsterBase hitMonster, Building master, WeaponBase weapon, Transform targetTransform)
    {
        yield return new WaitForSeconds(0.2f);
        if (hitMonster == null || master == null || weapon == null || targetTransform == null)
            yield break;

        if (hitMonster.IsDie())
            yield break;

        WeaponBase nextWeapon = WeaponPool.Instance.GetWeapon(weapon.m_eWeaponType, hitMonster.transform.position);
        if (nextWeapon)
        {
            nextWeapon.SetInfo(master, targetTransform, false, --weapon.bounceCount);
            nextWeapon.bEnableUpdate = true;
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
                if (distance < closestDistSqr)
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
