using GameDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Satellite : WeaponBase
{
    [SerializeField] private float distance = 3f;

    private float lifeTime = 0f;
    private float angle = 0f;

    protected override void Awake()
    {
        m_eWeaponType = WeaponType.BIGAXE;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        angle = 0f;
    }
    protected override void FixedUpdate()
    {
        if (!bEnableUpdate)
            return;

        if (Time.time - lifeTime > 5f)
        {
            DestroyPool();
            return;
        }

        if (m_MasterObject)
        {
            angle += moveSpeed * Time.deltaTime;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
            transform.position = m_MasterObject.transform.position + offset;

            Vector3 direction = transform.position - m_MasterObject.transform.position;
            float angleZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.Rotate(0f, 0f, angleZ);
        }

        base.FixedUpdate();
    }

    public override void SetInfo(Building master, Transform target, bool skipCollision, byte countBounce = 0)
    {
        base.SetInfo(master, target, skipCollision, countBounce);
        lifeTime = Time.time;
    }
}
