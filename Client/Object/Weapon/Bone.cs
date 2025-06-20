using CharacterDefines;
using GameDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bone : WeaponBase
{
    private float lifeTime = 0f;

    private int count = 0;
    protected override void Awake()
    {
        m_eWeaponType = WeaponType.BONE;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        count = 0;
        lifeTime = Time.time;
    }

    protected override void FixedUpdate()
    {
        if (!bEnableUpdate)
            return;

        Vector3 newPosition = direction * moveSpeed * Time.deltaTime;
        transform.position += newPosition;

        float rotationAngle = 45f * Time.deltaTime;
        transform.Rotate(Vector3.forward * rotationAngle);

        if (Time.time - lifeTime >= 3f)
        {
            DestroyPool();
            return;
        }
        if (count >= m_MasterObject.TargetCount)
        {
            DestroyPool();
            return;
        }
    }

    protected override void OnTriggerEnter(Collider collision)
    {
        if (gameObject == null)
            return;

        if (m_MasterObject)
        {
            if (m_MasterObject.HitMonster(collision.GetComponent<MonsterBase>(), this, HitParticleType.NONE) == false)
                return;
        }

        m_MasterObject.WeaponFlashSound(true);

        Vector3 normal = -(transform.position - collision.transform.position);
        direction = Vector3.Reflect(direction, normal).normalized;
        direction.z = 0f;
        ++count;
    }

    public override void SetInfo(Building master, Transform target, bool skipCollision, byte countBounce = 0)
    {
        base.SetInfo(master, target, false, 0);

        if (m_Target)
        {
            direction = (m_Target.position - transform.position).normalized;
        }
    }
}
