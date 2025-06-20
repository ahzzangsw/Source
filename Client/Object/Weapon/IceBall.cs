using GameDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBall : WeaponBase
{
    protected override void Awake()
    {
        m_eWeaponType = WeaponType.ICEBALL;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        if (bPenetrate && m_Target)
            direction = (m_Target.position - transform.position).normalized;
    }

    protected override void FixedUpdate()
    {
        if (!bEnableUpdate)
            return;

        if (m_Target != null)
        {
            if (!bPenetrate)
            {
                direction = (m_Target.position - transform.position).normalized;
            }
            else if (bUpdatePenetrate)
            {
                bUpdatePenetrate = false;
                direction = (m_Target.position - transform.position).normalized;
            }

            transform.position += direction * moveSpeed * Time.deltaTime;
        }

        base.FixedUpdate();
    }
}
