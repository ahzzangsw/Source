using CharacterDefines;
using GameDefines;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Magic : WeaponBase
{
    [SerializeField] private WeaponType m_eWeaponSubType = WeaponType.MAGIC;
    [SerializeField] float hitOffset = 0f;
    [SerializeField] private bool UseFirePointRotation;
    [SerializeField] private Vector3 rotationOffset = Vector3.zero;
    [SerializeField] private GameObject hit = null;
    [SerializeField] private GameObject flash = null;
    [SerializeField] private GameObject[] Detached;

    private Rigidbody rigidBody = null;
    private bool bWaveStart = false;

    // Bounce
    private Vector3[] BouncePosList = null;
    private int BounceIndex = 0;
    private float BounceTime = 0f;
    private float MaxBounceTime = 5f;

    protected override void Awake()
    {
        m_eWeaponType = m_eWeaponSubType;
        rigidBody = GetComponent<Rigidbody>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        direction = Vector3.zero;
        bWaveStart = false;
        BouncePosList = null;
        BounceTime = 0f;

        if (bPenetrate && m_Target)
            direction = (m_Target.position - transform.position).normalized;
    }

    public override void SetInfo(Building master, Transform target, bool skipCollision, byte countBounce = 0)
    {
        base.SetInfo(master, target, skipCollision, countBounce);
        SetFlashEffect();
    }
    public override void SetInfo(Building master, Vector3 endPosition)
    {
        base.SetInfo(master, endPosition);
        SetFlashEffect();
    }

    void SetFlashEffect()
    {
        if (OptionManager.Instance.bFlashEffect)
        {
            if (flash != null)
            {
                Vector3 newPosition = transform.position;
                newPosition.z = -1;
                GameObject flashInstance = Instantiate(flash, newPosition, Quaternion.identity);
                if (flashInstance)
                {
                    flashInstance.transform.forward = transform.forward;
                    ParticleSystem flashPs = flashInstance.GetComponent<ParticleSystem>();
                    if (flashPs != null)
                    {
                        if (flashPs.main.duration > 0)
                            Destroy(flashInstance, flashPs.main.duration / 2);
                        else
                            Destroy(flashInstance);
                    }
                    else
                    {
                        ParticleSystem flashPsParts = flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                        if (flashPsParts.main.duration > 0)
                            Destroy(flashInstance, flashPsParts.main.duration / 2);
                        else
                            Destroy(flashInstance);
                    }
                }
            }
        }
    }

    protected override void FixedUpdate()
    {
        if (!bEnableUpdate)
            return;

        switch (m_eMagicType) 
        {
            case MagicType.NONE:
            case MagicType.ONETIME_DOWN:
                Normal();
                break;
            case MagicType.BOUNCE:
                Bounce();
                break;
        }
    }

    private void Normal()
    {
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
            //float z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //transform.rotation = Quaternion.Euler(0, 0, z);
        }

        transform.position += direction * moveSpeed * Time.deltaTime;
        base.FixedUpdate();
    }
    
    private void Bounce()
    {
        if (!bWaveStart)
        {
            float distance = Vector3.Distance(m_vEndPosition, transform.position);
            if (distance > 0.2f)
            {
                direction = (m_vEndPosition - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;
            }
            else
            {
                bWaveStart = true;
                BounceTime = Time.time;
            }
        }
        else
        {
            if (Time.time - BounceTime > MaxBounceTime)
            {
                DestroyPool();
                return;
            }

            float distance = Vector3.Distance(BouncePosList[BounceIndex], transform.position);
            if (distance > 0.2f)
            {
                direction = (BouncePosList[BounceIndex] - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;
            }
            else
            {
                ++BounceIndex;
                if (BounceIndex > 1)
                    BounceIndex = 0;
            }
        }
    }

    protected override void OnTriggerEnter(Collider collision)
    {
        if (m_eMagicType == MagicType.NONE)
        {
            if (!bPenetrate)
            {
                if (collision.transform != m_Target)
                    return;
            }
        }

        if (gameObject == null)
            return;

        if (m_MasterObject)
        {
            if (m_MasterObject.HitMonster(collision.GetComponent<MonsterBase>(), this, HitParticleType.NONE) == false)
                return;
        }

        // 关俊 何磐 面倒贸府饶 柳青
        if (OptionManager.Instance.bHitEffect)
        {
            rigidBody.constraints = RigidbodyConstraints.FreezeAll;
            Vector3 contact = collision.transform.position.normalized;
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact);
            Vector3 pos = collision.transform.position + contact * hitOffset;

            if (hit != null)
            {
                pos.z = -1f;
                GameObject hitObject = Instantiate(hit, pos, rot);
                if (UseFirePointRotation) { hitObject.transform.rotation = transform.rotation * Quaternion.Euler(0, 180f, 0); }
                else if (rotationOffset != Vector3.zero) { hitObject.transform.rotation = Quaternion.Euler(rotationOffset); }
                else { hitObject.transform.LookAt(collision.transform.position + contact); }

                EffectBase effectHit = hitObject.GetComponent<EffectBase>();
                if (effectHit)
                    effectHit.SetInfo(m_MasterObject);

                ParticleSystem hitPs = hitObject.GetComponent<ParticleSystem>();
                if (hitPs != null)
                {
                    if (hitPs.main.duration > 0)
                        Destroy(hitObject, hitPs.main.duration / 2);
                    else
                        Destroy(hitObject);
                }
                else
                {
                    ParticleSystem hitPsParts = hitObject.transform.GetChild(0).GetComponent<ParticleSystem>();
                    if (hitPsParts.main.duration > 0)
                        Destroy(hitObject, hitPsParts.main.duration / 2);
                    else
                        Destroy(hitObject);
                }
            }

            m_MasterObject.WeaponFlashSound(true);

            foreach (var detachedPrefab in Detached)
            {
                if (detachedPrefab != null)
                {
                    detachedPrefab.transform.parent = null;
                    Destroy(detachedPrefab, 1);
                }
            }
        }

        if (m_eMagicType == MagicType.BOUNCE)
        {
            return;
        }

        if (m_MasterObject && bPenetrate)
        {
            float currentDistance = Vector2.Distance(m_MasterObject.transform.position, transform.position);
            if (currentDistance >= m_MasterObject.Range * 2f)
            {
                DestroyPool();
                return;
            }
        }
        else
        {
            DestroyPool();
        }
    }

    public override void SetMagicType(MagicType eMagicType)
    {
        m_eMagicType = eMagicType;
        if (m_eMagicType == MagicType.BOUNCE)
        {
            SetBounce();
            //return;
            
            //DestroyPool();
        }
    }

    private void SetBounce()
    {
        MapBase pMapBase = MapManager.Instance.GetCurrentMapInfo();
        if (pMapBase == null)
            return;

        if (BouncePosList == null)
        {
            BouncePosList = new Vector3[2];
            if (m_Target)
                m_vEndPosition = m_Target.transform.position;
            else
            {
                bWaveStart = true;
                BounceTime = Time.time;
                MaxBounceTime = 10f;
            }
        }

        if (Oracle.m_eGameType == MapType.ADVENTURE)
        {
            if (pMapBase is Map_Adventure == false)
                return;

            Map_Adventure pMapAdventure = pMapBase as Map_Adventure;
            if (Oracle.RandomDice(0, 2) == 0)
            {
                BouncePosList[0] = pMapAdventure.GetFloorBothEnds(m_MasterObject.GetCurrentLayerFloor(), true);
                BouncePosList[1] = pMapAdventure.GetFloorBothEnds(m_MasterObject.GetCurrentLayerFloor(), false);
            }
            else
            {
                BouncePosList[0] = pMapAdventure.GetFloorBothEnds(m_MasterObject.GetCurrentLayerFloor(), false);
                BouncePosList[1] = pMapAdventure.GetFloorBothEnds(m_MasterObject.GetCurrentLayerFloor(), true);
            }
        }
        else
        {
            MonsterBase monster = m_Target.GetComponent<MonsterBase>();
            if (monster == null)
                return;

            int index = monster.moveIndex;
            BouncePosList[0] = pMapBase.GetWayPointByVector2(index - 1);
            if (index > 1)
                BouncePosList[1] = pMapBase.GetWayPointByVector2(index - 2);
            else
                BouncePosList[1] = pMapBase.spawnPoint.position;
        }

        for (int i = 0; i < BouncePosList.Length; ++i)
        {
            BouncePosList[i].y += 0.5f;
        }
    }
}
