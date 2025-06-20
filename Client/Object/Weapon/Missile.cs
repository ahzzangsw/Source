using GameDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : WeaponBase
{
    private bool isAscending = true;

    [SerializeField] private float initialSpeed = 1f;         // 미사일의 초기 속도
    [SerializeField] private float maxSpeed = 15f;            // 미사일의 최대 속도
    [SerializeField] private float accelerationRate = 0.5f;   // 가속도
    [SerializeField] private float gravity = 9.81f;           // 중력 가속도
    [SerializeField] private GameObject ExplosionPrefeb = null;

    private Vector3 velocity;       // 미사일의 현재 속도

    protected override void Awake()
    {
        m_eWeaponType = WeaponType.MISSILE;

        Clear();
    }

    protected override void FixedUpdate()
    {
        if (!bEnableUpdate)
            return;

        if (m_Target != null)
        {
            if (isAscending)
            {
                velocity.y += accelerationRate * Time.deltaTime;
                velocity.y = Mathf.Clamp(velocity.y, initialSpeed, maxSpeed);
                transform.position += velocity * moveSpeed * Time.deltaTime;
                if (transform.position.y >= highest)
                {
                    isAscending = false;
                }
                return;
            }
            else
            {
                Vector3 direction = (m_Target.position - transform.position).normalized;
                float z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, z);
                transform.position += direction * gravity * moveSpeed * Time.deltaTime;
            }
        }

        base.FixedUpdate();
    }

    protected override void OnTriggerEnter(Collider collision)
    {
        if (isAscending)
            return;

        Explode();
        m_MasterObject.WeaponFlashSound(true);
        DestroyPool();
    }

    private void Explode()
    {
        if (ExplosionPrefeb == null || m_Target == null)
            return;

        Vector3 newPosition = m_Target.position;
        //newPosition.z = -1;

        EffectBase pEffectBase = EffectPool.Instance.GetEffect(newPosition, ExplosionPrefeb);
        if (pEffectBase != null)
        {
            pEffectBase.SetInfo(m_MasterObject);
        }
    }

    protected override void Clear()
    {
        base.Clear();

        isAscending = true;
        velocity = Vector3.up * initialSpeed;
        transform.rotation = Quaternion.Euler(0, 0, 90f);
    }
}
