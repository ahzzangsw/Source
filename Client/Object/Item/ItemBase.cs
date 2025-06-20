using GameDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ItemBase : MonoBehaviour
{
    [SerializeField] public ItemType m_eItemType = ItemType.NONE;

    protected Vector3 m_vLookPos = Vector3.zero;
    protected float m_EndPos = 0f;

    protected int m_iCount = 1;
    protected float m_fDisappearTime = 0.5f;

    protected IObjectPool<ItemBase> ManagedPool;

    protected virtual void Awake()
    {

    }

    protected virtual void Update()
    {

    }

    public virtual void SetManagedPool(IObjectPool<ItemBase> pool)
    {
        ManagedPool = pool;
    }

    public virtual void DestroyPool()
    {
        ManagedPool.Release(this);
    }

    public void SetItemCount(int count)
    {
        m_iCount = count;
    }

    public void SetFly(Vector3 vLook, float EndPos)
    {
        m_vLookPos = vLook;
        m_EndPos = EndPos-1f;

        m_vLookPos.z = 0f;
    }

    public virtual void SetInt(int i)
    {

    }

    public virtual void Appear()
    {
        if (Oracle.m_eGameType == MapType.ADVENTURE)
        {
            Player MyPlayer = GameManager.Instance.GetPlayer();
            if (MyPlayer)
            {
                MyPlayer.PlayerIgnoreCollision(GetComponent<Collider>());
            }
        }
    }

    public virtual void PickUp()
    {
        Invoke("DestroyItem", m_fDisappearTime);
    }

    protected virtual void DestroyItem()
    {
        DestroyPool();
    }
}
