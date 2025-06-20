using GameDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splash : WeaponBase
{
    [SerializeField] private WeaponType eWeaponType = WeaponType.NONE;
    [SerializeField] float hitOffset = 0f;
    [SerializeField] private GameObject hit = null;
    [SerializeField] private GameObject flash = null;
    [SerializeField] private GameObject[] Detached;

    private Rigidbody rigidBody = null;

    protected override void Awake()
    {
        m_eWeaponType = eWeaponType;

        rigidBody = GetComponent<Rigidbody>();
        if (flash != null)
        {
            var flashInstance = Instantiate(flash, transform.position, Quaternion.identity);
            flashInstance.transform.forward = gameObject.transform.forward;

            var flashPs = flashInstance.GetComponent<ParticleSystem>();
            if (flashPs != null)
            {
                Destroy(flashInstance, flashPs.main.duration);
            }
            else
            {
                var flashPsParts = flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(flashInstance, flashPsParts.main.duration);
            }
        }
    }

    protected override void FixedUpdate()
    {
        if (!bEnableUpdate)
            return;
        
        if (m_Target != null)
        {
            Vector3 direction = (m_Target.position - transform.position).normalized;
            float z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, 0, -z);
            transform.position += direction * moveSpeed * Time.deltaTime;
        }

        base.FixedUpdate();
    }

    protected override void OnTriggerEnter(Collider collision)
    {
        if (!bPenetrate)
        {
            if (collision.transform != m_Target)
                return;
        }

        if (gameObject == null)
            return;

        Explode(collision.transform.position);
        m_MasterObject.WeaponFlashSound(true);
        DestroyPool();
    }

    private void Explode(Vector3 position)
    {
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        if (hit != null)
        {
            Vector3 contact = position.normalized;
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact);
            Vector3 pos = position + contact * hitOffset;

            EffectBase pEffectBase = EffectPool.Instance.GetEffect(pos, hit);
            if (pEffectBase)
            {
                pEffectBase.SetInfo(m_MasterObject);
                pEffectBase.SetParticleHitEffect(rot, position + contact);
            }
        }

        foreach (var detachedPrefab in Detached)
        {
            if (detachedPrefab != null)
            {
                detachedPrefab.transform.parent = null;
                Destroy(detachedPrefab, 1);
            }
        }
    }

    protected override void ForceExplodeWeapon()
    {
        if (gameObject == null)
            return;

        Explode(transform.position);
    }
}
