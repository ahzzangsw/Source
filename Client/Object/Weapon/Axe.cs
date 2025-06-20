using GameDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : WeaponBase
{
    protected override void Awake()
    {
        m_eWeaponType = WeaponType.AXE;

        Clear();
    }

    protected override void FixedUpdate()
    {
        if (!bEnableUpdate)
            return;

        if (m_Target != null)
        {
            direction = (m_Target.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.Rotate(Vector3.forward * 10f);
        }

        base.FixedUpdate();
    }
}
