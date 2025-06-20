using CharacterDefines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;

public class Boss8 : BossBase
{
    private enum Boss8State { NORMAL, ANGRY, MAD, FURY, FIRE };
    private Boss8State eBoss8State = Boss8State.NORMAL;
    private List<KeyValuePair<int, bool>> StatusAboutHP = null;
    protected override void Awake()
    {
        base.Awake();
        iBossSkillPercent = 100;
        eBoss8State = Boss8State.NORMAL;

        StatusAboutHP = new List<KeyValuePair<int, bool>>();
        RubyCount = 8;
    }

    protected override void SetBasicInfo()
    {
        float Maxhp = (float)Hp;
        StatusAboutHP.Add(new KeyValuePair<int, bool>((int)(Maxhp * 0.7f), true));
        StatusAboutHP.Add(new KeyValuePair<int, bool>((int)(Maxhp * 0.5f), true));
        StatusAboutHP.Add(new KeyValuePair<int, bool>((int)(Maxhp * 0.1f), true));
    }

    protected override void FixedUpdate()
    {
        if (eBoss8State == Boss8State.ANGRY || eBoss8State == Boss8State.MAD || eBoss8State == Boss8State.FIRE)
        {
            Vector2 vecArrived = movePositions[moveIndex];
            LookVector = (vecArrived - (Vector2)transform.position).normalized;
            if (LookVector.x != 0)
            {
                Turn(LookVector.x);
            }

            Vector3 newPosition = LookVector * moveSpeed * Time.deltaTime;
            transform.position += newPosition;

            float distance = Vector3.Distance(transform.position, vecArrived);
            if (distance < 1f)
            {
                eBoss8State = Boss8State.NORMAL;

                KeyValuePair<int, bool> updatePair = new KeyValuePair<int, bool>(StatusAboutHP[(int)eBoss8State].Key, false);
                StatusAboutHP[(int)eBoss8State] = updatePair;
                return;
            }
        }
        else
        {
            base.FixedUpdate();
        }
    }

    protected override void DoInterrupt()
    {
        if (eBoss8State == Boss8State.NORMAL)
        {
            if (currentHP <= StatusAboutHP[2].Key)
            {
                eBoss8State = Boss8State.FURY;
                iBossSkillPercent = 10;
                moveSpeed += 5;
            }
            else if (currentHP <= StatusAboutHP[1].Key && StatusAboutHP[1].Value)
            {
                eBoss8State = Boss8State.MAD;
                SetMoveIndex(2);
            }
            else if (currentHP <= StatusAboutHP[0].Key && StatusAboutHP[0].Value)
            {
                eBoss8State = Boss8State.ANGRY;
                SetMoveIndex(1);
            }
        }
        else if (eBoss8State == Boss8State.FURY)
        {
            eBoss8State = Boss8State.FIRE;
            SetMoveIndex(movePositions.Length);
            m_Animator.SetBool("isMad", false);
            m_Animator.SetBool("isAngry", false);
        }
    }

    private void SetMoveIndex(int iAdd)
    {
        moveIndex = moveIndex + iAdd;
        if (moveIndex >= movePositions.Length)
        {
            moveIndex = movePositions.Length - 1;
        }
    }

    protected override void ClearInterrupt()
    {
        bInterrupt = false;
        eBoss8State = Boss8State.NORMAL;
    }
}
