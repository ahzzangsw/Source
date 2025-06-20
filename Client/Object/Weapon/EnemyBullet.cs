using CharacterDefines;
using GameDefines;
using UnityEngine;

public class EnemyBullet : WeaponBase
{
    private BossAdventure_Last_Skill m_Owner = null;

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
    }

    protected override void Awake()
    {
        m_eWeaponType = WeaponType.ENEMYBALL;
    }

    protected override void FixedUpdate()
    {
        if (!bEnableUpdate)
            return;

        transform.position += direction * moveSpeed * Time.deltaTime;

        bool bDestroy = false;
        if (transform.position.x < -19f)
            bDestroy = true;
        else if (transform.position.x > 19f)
            bDestroy = true;
        else if (transform.position.y < -10f)
            bDestroy = true;
        else if (transform.position.y > 12f)
            bDestroy = true;

        if (bDestroy)
        {
            bEnableUpdate = false;
            DestroyPool();
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
}
