using GameDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EffectBase : MonoBehaviour
{
    [SerializeField] public EffectType eEffectType = EffectType.NONE;
    [SerializeField] protected string aniName = "";

    protected Animator animator = null;

    private IObjectPool<EffectBase> ManagedPool;

    protected Building m_MasterObject = null;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public virtual void SetInfo(Building master)
    {
        m_MasterObject = master;
        if (animator)
            animator.Play(aniName);
    }

    public virtual void OnAnimationEnd()
    {
        DestroyPool();
    }

    protected virtual void OnTriggerEnter(Collider collision)
    {
    }

    private void Clear()
    {
        transform.rotation = Quaternion.identity;
    }

    public void SetManagedPool(IObjectPool<EffectBase> pool)
    {
        ManagedPool = pool;
    }

    public void DestroyPool()
    {
        Clear();

        if (Oracle.m_eGameType == MapType.ADVENTURE)
        {
            if (m_MasterObject == null)
            {
                Destroy(gameObject);
                return;
            }
        }

        ManagedPool.Release(this);
    }

    public void SetRotation(Vector2 vRotate)
    {
        if (vRotate == Vector2.right)
            return;
        
        if (vRotate == Vector2.up)
            transform.rotation = Quaternion.Euler(0, 0, 90);
        else if (vRotate == Vector2.down)
            transform.rotation = Quaternion.Euler(0, 0, -90);
        else
            transform.rotation = Quaternion.Euler(0, 0, 180);
    }

    public virtual void SetParticleHitEffect(Quaternion rot, Vector3 vLookAt)
    {
        transform.rotation = rot;
        transform.LookAt(vLookAt);

        ParticleSystem hitPs = GetComponent<ParticleSystem>();
        if (hitPs != null)
        {
            StartCoroutine(DestroyCoroutine(hitPs.main.duration));
        }
        else
        {
            if (transform.childCount == 0)
                return;

            ParticleSystem hitPsParts = transform.GetChild(0).GetComponent<ParticleSystem>();
            StartCoroutine(DestroyCoroutine(hitPsParts.main.duration));
        }
    }

    private IEnumerator DestroyCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        OnAnimationEnd();
        StopCoroutine(DestroyCoroutine(0f));
    }
}
