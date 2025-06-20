using DG.Tweening;
using GameDefines;
using OptionDefines;
using System.Collections;
using UnityEngine;

public class Gimmick_Troll : Gimmick
{
    protected override void Clear()
    {
        base.Clear();
    }
    protected override IEnumerator PhaseStep0()
    {
        FireSpeed = 3f;
        FireCount = 1;
        SubAttackState("Shooting");
        yield return null;
    }
    protected override IEnumerator PhaseStep1()
    {
        FireSpeed = 0.8f;
        FireCount = 3;
        SubAttackState("Shooting");
        yield return null;
    }
    protected override IEnumerator PhaseStep2()
    {
        FireSpeed = 0.3f;
        FireCount = 5;
        SubAttackState("Shooting");
        yield return null;
    }

    private IEnumerator Shooting()
    {
        if (m_OwnerBoss == null)
            yield break;

        for (int i = 0; i < FireCount; ++i)
        {
            if (m_Target == null)
                break;

            WeaponBase clone = WeaponPool.Instance.GetWeapon(WeaponType.ENEMYSHOCKWAVE, m_MuzzlePosition);
            if (clone == null)
                continue;

            EnemyShockWave stone = clone as EnemyShockWave;
            if (stone == null)
                continue;

            m_OwnerBoss.Attack();
            SoundManager.Instance.PlayUISound(UISoundType.ENEMYSHOCKWAVE);

            stone.SetInfo(m_OwnerBoss, m_Target.transform, m_OwnerBoss.transform.position);
            stone.bEnableUpdate = true;
            yield return new WaitForSeconds(FireSpeed);
        }

        bSkillEnd = true;
    }
}