using CharacterDefines;
using GameDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workman : Character
{
    public float moveSpeed { get; set; } = 0;
    public int moveIndex { get; protected set; } = 1;

    protected Vector2[] movePositions;

    protected bool bWork = true;
    protected int workDelay = 3;
    protected int currentMoveIndex = -1;
    protected Vector2 initialPosition;
    protected float journeyLength;
    protected float startTime;

    private Item_Money digMoney = null;

    protected virtual void Awake()
    {
        movePositions = new Vector2[3] { new Vector2(11.5f, -16f), new Vector2(-11f, -16f), new Vector2(11.5f, -16f) };
        m_AttackAnimState = "Attack";

        moveIndex = 1;
        currentMoveIndex = -1;
        moveSpeed = 1f;

        transform.position = movePositions[0];
        bWork = false;
    }

    protected override void FixedUpdate()
    {
        if (bWork)
            return;

        Vector2 vecArrived = movePositions[moveIndex];
        if (currentMoveIndex != moveIndex)
        {
            currentMoveIndex = moveIndex;
            initialPosition = transform.position;
            journeyLength = Vector2.Distance(initialPosition, vecArrived);
            startTime = Time.time;
        }

        float distanceCovered = (Time.time - startTime) * moveSpeed;
        float journeyFraction = distanceCovered / journeyLength;
        Vector2 LookVector = (vecArrived - (Vector2)transform.position).normalized;
        if (LookVector.x != 0)
        {
            Turn(LookVector.x);
        }

        transform.position = Vector2.Lerp(initialPosition, vecArrived, journeyFraction);
        if (digMoney != null)
        {
            digMoney.transform.position = transform.position;
        }

        if (journeyFraction >= 1.0f)
        {
            ++moveIndex;
            if (moveIndex >= movePositions.Length)
            {
                moveIndex = 1;

                if (digMoney != null)
                {
                    digMoney.PickUp();
                    digMoney = null;
                }
            }
            else
            {
                if (moveIndex == 2)
                {
                    bWork = true;
                    StartCoroutine(CallWorkStart());
                }
            }
        }

        SetAnimationState(LAnimationState.Running);
    }

    private IEnumerator CallWorkStart()
    {
        for (int i = 0; i < workDelay; ++i)
        {
            SetAnimationActionState(LAnimationState.Attack);
            yield return new WaitForSeconds(1f);
        }

        bWork = false;

        Vector3 MoneyPosition = transform.position;
        digMoney = ItemManager.Instance.MakeItem(ItemType.MONEY, MoneyPosition) as Item_Money;
        if (digMoney) 
        {
            digMoney.SetInt(Oracle.RandomDice(3, 6));
        }

        SetAnimationState(LAnimationState.Running);
    }

    public void ChangeMoveSpeedPercent(float percent)
    {
        moveSpeed *= percent;
    }
}
