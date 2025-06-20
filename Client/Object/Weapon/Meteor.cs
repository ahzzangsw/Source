using GameDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : Splash
{
    [SerializeField] private float initialX = 3f;         // 메테오의 발사각
    private bool bInit = false;

    protected override void Awake()
    {
        base.Awake();
        m_eWeaponType = WeaponType.METEOR;
        initialize();
    }

    protected override void FixedUpdate()
    {
        if (!bEnableUpdate)
            return;

        if (m_Target != null)
        {
            initialize();

            Vector3 direction = (m_Target.position - transform.position).normalized;
            float z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, 0, -z);
            transform.position += direction * moveSpeed * Time.deltaTime;
        }

        base.FixedUpdate();
    }

    protected override void Clear()
    {
        base.Clear();
        bInit = false;
    }

    private void initialize()
    {
        if (m_Target == null || bInit)
            return;

        Vector3 initialPos = m_Target.position;
        initialPos.y = highest;
        if (m_Target.position.x < 0)
            initialPos.x -= initialX;
        else
            initialPos.x += initialX;

        transform.position = initialPos;
        bInit = true;
    }
}
