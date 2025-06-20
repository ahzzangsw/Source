using GameDefines;
using OptionDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Ruby : ItemBase
{
    private bool bProduction = false;
    private Camera mainCamera = null;
    private Rigidbody rigid = null;
    private Vector2 lastVelocity = Vector2.zero;
    private float fSpeed = 0f;

    protected override void Awake()
    {
        mainCamera = Camera.main;
        m_fDisappearTime = 0.1f;
        fSpeed = 300f;
        Vector2 lastVelocity = Vector2.zero;
    }

    private void OnEnable()
    {
        bProduction = false;
        Vector2 lastVelocity = Vector2.zero;
    }

    protected override void Update()
    {
        if (bProduction == false)
            return;

        if (rigid == null)
            return;

        if (transform.position.y < m_EndPos)
        {
            bProduction = false;
            rigid.velocity = Vector2.zero;
            rigid.angularVelocity = Vector2.zero;
            Destroy(rigid);
            return;
        }

        Vector3 objectViewportPos = mainCamera.WorldToViewportPoint(transform.position);
        bool isWithinBounds = objectViewportPos.x > 0f && objectViewportPos.x < 1f && objectViewportPos.y > 0f && objectViewportPos.y < 1f;
        if (isWithinBounds == false)
            rigid.velocity = Vector2.Reflect(rigid.velocity.normalized, objectViewportPos.normalized);
    }

    public override void Appear()
    {
        rigid = gameObject.AddComponent<Rigidbody>();
        if (rigid)
        {
            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
            rigid.AddForce(m_vLookPos * fSpeed);
        }

        bProduction = true;
        SoundManager.Instance.PlayUISound(UISoundType.DROPITEM);

        base.Appear();
    }
    public override void PickUp()
    {
        GameManager.Instance.AddGameMoney(m_iCount);
        SoundManager.Instance.PlayUISound(UISoundType.PICKUPITEM);
        // 연출 필요

        base.PickUp();
    }
}
