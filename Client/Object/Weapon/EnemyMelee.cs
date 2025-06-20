using GameDefines;
using System.Collections;
using UnityEngine;

public class EnemyMelee : WeaponBase
{
    private BossAdventure_Last_Skill m_Owner = null;
    private bool m_ActionAttack = false;

    protected override void Awake()
    {
        m_eWeaponType = WeaponType.ENEMYMELEE;
    }

    protected override void Clear()
    {
        base.Clear();
    }

    public override void SetInfo(BossAdventure_Last_Skill master, Transform target, Vector3 weaponDirection)
    {
        m_Owner = master;
        m_Target = target;
        direction = weaponDirection;

        m_Damage = m_Owner.m_Damage;
        m_ActionAttack = true;
    }

    protected override void FixedUpdate()
    {
        if (bEnableUpdate == false)
            return;

        if (m_ActionAttack)
        {
            m_ActionAttack = false;
            StartCoroutine(DestroyWeapon());
        }
    }

    protected override void OnTriggerEnter(Collider collision)
    {
        if (gameObject == null)
            return;

        if (collision.transform != m_Target)
            return;

        if (BossAdventure_Last_Manager.Instance.m_bDie)
            return;

        GameManager.Instance.GetPlayer().ReduceHP(m_Damage);
        if (m_Owner)
        {
            m_Owner.WeaponFlashSound(m_eWeaponType, true);
        }

        DestroyPool();
    }

    private IEnumerator DestroyWeapon()
    {
        bEnableUpdate = false;
        yield return new WaitForSeconds(0.2f);
        DestroyPool();
    }
}
