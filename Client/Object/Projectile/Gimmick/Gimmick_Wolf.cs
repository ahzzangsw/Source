using DG.Tweening;
using GameDefines;
using OptionDefines;
using System.Collections;
using UnityEngine;

public class Gimmick_Wolf : Gimmick
{
    //FireSpeed = 2f;
    //FireCount = 10;
    private bool bCloaking = false;
    [SerializeField] private float fMoveTIme = 1f;

    protected override void Clear()
    {
        base.Clear();
    }
    protected override IEnumerator PhaseStep0()
    {
        FireSpeed = 0.5f;
        FireCount = 1;
        moveSpeed = 2;
        StartCoroutine("MeleeAttack");
        yield return null;
    }
    protected override IEnumerator PhaseStep1()
    {
        FireSpeed = 0.5f;
        FireCount = 1;
        moveSpeed = 3;
        StartCoroutine("MeleeAttack");
        yield return null;
    }
    protected override IEnumerator PhaseStep2()
    {
        FireSpeed = 0.2f;
        FireCount = 1;
        moveSpeed = 5;
        Cloaking();
        StartCoroutine("MeleeAttack");
        yield return null;
    }

    private void Cloaking()
    {
        if (m_OwnerBoss == null)
            return;

        m_OwnerBoss.FadeOut(0f);
    }

    private IEnumerator MeleeAttack()
    {
        if (m_OwnerBoss == null || m_Target == null)
            yield break;

        float fTime = 0f;
        float directionForce = 0f;
        while (true)
        {
            if (m_Target == null)
                yield break;

            fTime += Time.deltaTime;

            Vector3 direction = (m_Target.transform.position - transform.position).normalized;
            float fDistance = Vector3.Distance(transform.position, m_Target.transform.position);

            if (fDistance <= MeleeAttackDistance)
            {
                m_OwnerBoss.FadeOutRollback();
                yield return new WaitForSeconds(FireSpeed);
                directionForce = direction.magnitude;
                break;
            }
            else
            {
                transform.position += direction * moveSpeed * Time.deltaTime;
                m_OwnerBoss.Running();
                m_OwnerBoss.Turn(direction.x);
            }

            if (fTime > fMoveTIme)
                break;

            yield return null;
        }

        if (directionForce != 0f)
        {
            StartCoroutine(Hitting(directionForce));
        }
        else
        {
            Destroy(m_OwnerBoss.gameObject);
        }

        yield break;
    }

    private IEnumerator Hitting(float directionX)
    {
        if (m_OwnerBoss == null || m_Target == null)
            yield break;

        WeaponBase clone = WeaponPool.Instance.GetWeapon(WeaponType.ENEMYMELEE, m_MuzzlePosition);
        if (clone == null)
            yield break;

        EnemyMelee stone = clone as EnemyMelee;
        if (stone == null)
            yield break;

        m_OwnerBoss.Attack();

        SoundManager.Instance.PlayUISound(UISoundType.ENEMYMELEE);

        Vector3 WeaponPosition = m_OwnerBoss.transform.position;
        WeaponPosition.x = directionX < 0f ? WeaponPosition.x - 1f : WeaponPosition.x + 1f;

        stone.SetInfo(m_OwnerBoss, m_Target.transform, WeaponPosition);
        stone.bEnableUpdate = true;

        yield return new WaitForSeconds(1f);
        Destroy(m_OwnerBoss.gameObject);
    }
}