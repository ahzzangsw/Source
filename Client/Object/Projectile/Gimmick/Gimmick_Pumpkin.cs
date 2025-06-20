using DG.Tweening;
using GameDefines;
using OptionDefines;
using System.Collections;
using UnityEngine;

public class Gimmick_Pumpkin : Gimmick
{
    private bool bSkillEnd0 = false;
    private bool bSkillEnd1 = false;

    protected override void Update()
    {
        if (bSkillEnd0 && bSkillEnd1)
            bSkillEnd = true;

        base.Update();
    }

    protected override void Clear()
    {
        base.Clear();
        bSkillEnd0 = false;
        bSkillEnd1 = false;
    }
    protected override IEnumerator PhaseStep0()
    {
        FireSpeed = 2f;
        FireCount = 3;
        bSkillEnd0 = true;
        SubAttackState("CircleFire"); 
        yield return null;
    }
    protected override IEnumerator PhaseStep1()
    {
        FireSpeed = 1f;
        FireCount = 10;
        bSkillEnd1 = true;
        moveSpeed = 5;
        SubAttackState("TargetFire");
        SubAttackState("BackAndForth");
        yield return null;
    }
    protected override IEnumerator PhaseStep2()
    {
        FireSpeed = 0.8f;
        FireCount = 10;
        moveSpeed = 5;
        SubAttackState("TargetFire");
        SubAttackState("CircleFire");
        SubAttackState("BackAndForth");
        yield return null;
    }

    private IEnumerator TargetFire()
    {
        if (m_OwnerBoss == null)
            yield break;

        float EndY = transform.position.y < 0 ? 12f : -12f;
        for (int i = 0; i < FireCount; ++i)
        {
            if (m_Target == null)
                break;

            WeaponBase clone = WeaponPool.Instance.GetWeapon(WeaponType.ENEMYBALL, m_MuzzlePosition);
            if (clone == null)
                continue;

            EnemyBullet bullet = clone as EnemyBullet;
            if (bullet == null)
                continue;

            SoundManager.Instance.PlayUISound(UISoundType.ENEMYBULLET);

            Vector3 EndPosition = new Vector3(m_MuzzlePosition.x, EndY, m_MuzzlePosition.z);
            Vector3 vecDirection = (EndPosition - bullet.transform.position).normalized;
            bullet.SetInfo(m_OwnerBoss, m_Target.transform, vecDirection);
            bullet.bEnableUpdate = true;
            yield return new WaitForSeconds(FireSpeed);
        }

        bSkillEnd0 = true;
    }

    private IEnumerator CircleFire()
    {
        if (m_OwnerBoss == null)
            yield break;

        int count = 20;
        float intervalAngle = 360 / count;
        float weightAngle = 0f;

        for (int i = 0; i < FireCount; ++i)
        {
            if (m_Target == null)
                break;

            for (int j = 0; j < count; ++j)
            {
                if (m_Target == null)
                    break;

                WeaponBase clone = WeaponPool.Instance.GetWeapon(WeaponType.ENEMYBALL, m_MuzzlePosition);
                if (clone == null)
                    continue;

                EnemyBullet bullet = clone as EnemyBullet;
                if (bullet == null)
                    continue;

                SoundManager.Instance.PlayUISound(UISoundType.ENEMYBALL);

                float angle = weightAngle + intervalAngle * j;
                float x = Mathf.Cos(angle * Mathf.PI / 180f);
                float y = Mathf.Sin(angle * Mathf.PI / 180f);
                bullet.SetInfo(m_OwnerBoss, m_Target.transform, new Vector3(x, y, 0f));
                bullet.bEnableUpdate = true;
            }

            // 발사체 생성되는 시작 각도 설정 변수
            weightAngle += 10f;
            yield return new WaitForSeconds(FireSpeed);
        }

        bSkillEnd1 = true;
    }

    private IEnumerator BackAndForth()
    {
        if (m_OwnerBoss == null)
            yield break;

        float fDistance = Oracle.RandomDice(0, 2) == 0 ? 1f : -1f;
        while (true)
        {
            if (m_Target == null)
                break;

            transform.position += Vector3.right * fDistance * moveSpeed * Time.deltaTime;
            m_OwnerBoss.Running();

            if (transform.position.x < -11f || transform.position.x > 11f)
            {
                fDistance *= -1f;
            }

            yield return null;
        }
    }
}