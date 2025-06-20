using CharacterDefines;
using GameDefines;
using System.Collections;
using UnityEngine;

public class EnemyStone : WeaponBase
{
    [SerializeField] private float m_fForce = 1f;

    private BossAdventure_Last_Skill m_Owner = null;
    
    private bool m_bForceTransmission = false;
    private Rigidbody m_RigidBody = null;
    private SpriteRenderer m_Body = null;

    protected override void Clear()
    {
        base.Clear();

        m_bForceTransmission = false;
    }

    public override void SetInfo(BossAdventure_Last_Skill master, Transform target, Vector3 weaponDirection)
    {
        m_Owner = master;
        m_Target = target;
        direction = weaponDirection;

        m_Damage = m_Owner.m_Damage;

        if (m_RigidBody == null)
        {
            m_RigidBody = GetComponent<Rigidbody>();
        }
        if (m_Body == null)
        {
            m_Body = GetComponent<SpriteRenderer>();
        }

        m_RigidBody.velocity = Vector3.zero;
    }

    public void SetSprite(Sprite InSprite)
    {
        if (m_Body == null)
            return;

        m_Body.sprite = InSprite;
        m_bForceTransmission = true;
    }

    protected override void Awake()
    {
        m_eWeaponType = WeaponType.ENEMYSTONE;
    }

    protected override void FixedUpdate()
    {
        if (!bEnableUpdate)
            return;

        if (m_bForceTransmission)
        {
            m_bForceTransmission = false;

            if (m_RigidBody != null)
                m_RigidBody.AddForce(direction * m_fForce, ForceMode.Impulse);
        }

        bool bDestroy = false;
        if (transform.position.x < -19f)
            bDestroy = true;
        else if (transform.position.x > 19f)
            bDestroy = true;
        else if (transform.position.y < -10f)
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
