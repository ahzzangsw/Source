using DG.Tweening;
using GameDefines;
using OptionDefines;
using System.Collections;
using UnityEngine;

public class Gimmick_Ogre : Gimmick
{
    private bool bSkillEnd0 = false;
    protected override void Clear()
    {
        base.Clear();
    }
    protected override IEnumerator PhaseStep0()
    {
        FireSpeed = 0.5f;
        FireCount = 6;
        SubAttackState("Throwing");
        yield return null;
    }
    protected override IEnumerator PhaseStep1()
    {
        FireSpeed = 0.3f;
        FireCount = 10;
        SubAttackState("Throwing");
        yield return null;
    }
    protected override IEnumerator PhaseStep2()
    {
        FireSpeed = 0.2f;
        FireCount = 30;
        SubAttackState("Throwing");
        yield return null;
    }

    private IEnumerator Throwing()
    {
        if (m_OwnerBoss == null)
            yield break;

        Sprite[] outSprite = BossAdventure_Last_Manager.Instance.SlimeSpriteList;
        if (outSprite == null || outSprite.Length == 0)
        {
            Debug.Log("BossAdventure_Last_Manager Slime Sprite is null");
            yield break;
        }

        float angle = 40f;
        for (int i = 0; i < FireCount; ++i)
        {
            if (m_Target == null)
                break;

            WeaponBase clone = WeaponPool.Instance.GetWeapon(WeaponType.ENEMYSTONE, m_MuzzlePosition);
            if (clone == null)
                continue;

            EnemyStone stone = clone as EnemyStone;
            if (stone == null)
                continue;

            float angleRad = Oracle.RandomDice(angle, 90f) * Mathf.Deg2Rad;
            float randomAngle = Oracle.RandomDice(0, 2) == 0 ? Mathf.Cos(angleRad) : -Mathf.Cos(angleRad);
            //Vector3 vecDirection = new Vector2(randomAngle, Mathf.Sin(angleRad)) * FireSpeed;
            Vector3 vecDirection = new Vector2(randomAngle, 1f);

            SoundManager.Instance.PlayUISound(UISoundType.ENEMYSTONE);

            stone.SetInfo(m_OwnerBoss, m_Target.transform, vecDirection);
            stone.SetSprite(outSprite[Oracle.RandomDice(0, outSprite.Length)]);
            stone.bEnableUpdate = true;
            yield return new WaitForSeconds(FireSpeed);
        }

        bSkillEnd = true;
    }
}