using CharacterDefines;
using GameDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class WeaponBase : MonoBehaviour
{
    [SerializeField] protected float moveSpeed = 1f;
    [SerializeField] protected float highest = 12f;
    public WeaponType m_eWeaponType { get; protected set; } = WeaponType.NONE;
    public bool bPenetrate { get; protected set; } = false;
    public byte bounceCount { get; set; } = 0;

    public bool bEnableUpdate { get; set; } = false;
    protected int bounceID = 0;

    protected Building m_MasterObject = null;
    protected MagicType m_eMagicType = MagicType.NONE;
    protected Vector3 m_vEndPosition = Vector3.zero;
    protected Transform m_Target = null;
    protected bool bUpdatePenetrate = false;
    protected Vector3 direction = Vector3.zero;

    protected IObjectPool<WeaponBase> ManagedPool;

    protected AudioClip[] audioClips = { null, null, null };    // Spawn, Fire, Hit

    protected int m_Damage = 0;

    protected virtual void Awake()
    {

    }

    protected virtual void OnEnable()
    {
        bEnableUpdate = false;
    }

    protected virtual void FixedUpdate()
    {
        if (m_MasterObject == null)
        {
            DestroyPool();
            return;
        }

        if (bPenetrate)
        {
            float currentDistance = Vector2.Distance(m_MasterObject.transform.position, transform.position);
            if (currentDistance >= m_MasterObject.Range * 2f)
            {
                DestroyPool();
                return;
            }
        }
        else
        {
            if (m_Target == null)
            {
                ForceExplodeWeapon();
                DestroyPool();
                return;
            }

            if (m_Target.gameObject.activeSelf == false)
            {
                ForceExplodeWeapon();
                DestroyPool();
                return;
            }

            MonsterBase monster = m_Target.GetComponent<MonsterBase>();
            if (monster != null)
            {
                if (monster.IsDie())
                {
                    ForceExplodeWeapon();
                    DestroyPool();
                    return;
                }
            }
        }
    }

    protected virtual void OnTriggerEnter(Collider collision)
    {
        if (!bPenetrate)
        {
            if (collision.transform != m_Target)
                return;
        }

        if (gameObject == null)
            return;

        if (m_MasterObject)
        {
            if (m_MasterObject.HitMonster(collision.GetComponent<MonsterBase>(), this, HitParticleType.NONE) == false)
                return;

            m_MasterObject.WeaponFlashSound(true);
        }

        if (!bPenetrate)
        {
            DestroyPool();
        }
    }

    protected virtual void Clear()
    {
        bPenetrate = false;
    }

    public virtual void SetInfo(Building master, Transform target, bool skipCollision, byte countBounce = 0)
    {
        m_Target = target;
        m_MasterObject = master;
        bPenetrate = skipCollision;
        bUpdatePenetrate = true;
        bounceCount = countBounce;
    }

    public virtual void SetInfo(Building master, Vector3 endPosition)
    {
        m_MasterObject = master;
        m_vEndPosition = endPosition;
    }

    public virtual void SetInfo(BossAdventure_Last_Skill master, Transform target, Vector3 weaponDirection)
    {
        direction = weaponDirection;
    }

    public virtual void SetMagicType(MagicType eMagicType)
    {
    }

    public virtual void SetManagedPool(IObjectPool<WeaponBase> pool)
    {
        ManagedPool = pool;
    }

    public virtual void DestroyPool()
    {
        Clear();
        ManagedPool.Release(this);
    }

    protected virtual void ForceExplodeWeapon() { }
}
