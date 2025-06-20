using GameDefines;
using UnityEngine;

public class Arrow : WeaponBase
{
    protected override void Awake()
    {
        m_eWeaponType = WeaponType.ARROW;
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
        }

        float z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, z);
        transform.position += direction * moveSpeed * Time.deltaTime;

        base.FixedUpdate();
    }
}
