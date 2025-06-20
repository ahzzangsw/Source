using GameDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : WeaponBase
{
    protected override void Awake()
    {
        m_eWeaponType = WeaponType.LASER;
    }

    protected override void FixedUpdate()
    {
        if (!bEnableUpdate)
            return;

        if (m_Target != null)
        {

        }

        base.FixedUpdate();
    }
}
