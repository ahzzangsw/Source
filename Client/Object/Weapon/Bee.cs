using GameDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : WeaponBase
{
    private Vector3 controlPoint;
    private float t = 0f;

    protected override void Awake()
    {
        m_eWeaponType = WeaponType.BEE;
    }

    protected override void FixedUpdate()
    {
        if (!bEnableUpdate)
            return;

        if (m_Target != null)
        {
            t += Time.deltaTime * moveSpeed;
            if (t > 1f)
                t = 1f;

            //BezierCurve
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 position = uuu * transform.position;
            position += 3 * uu * t * m_Target.position;
            position += 3 * u * tt * controlPoint;
            position += ttt * m_Target.position;

            transform.position = new Vector3(position.x, position.y, 0);
        }

        base.FixedUpdate();
    }

    protected override void Clear()
    {
        base.Clear();
    }

    public override void SetInfo(Building master, Transform target, bool skipCollision, byte countBounce = 0)
    {
        base.SetInfo(master, target, skipCollision, countBounce);

        if (m_Target != null)
        {
            controlPoint = (m_Target.position - transform.position).normalized;
        }
    }
}
