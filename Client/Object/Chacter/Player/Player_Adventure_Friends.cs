using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Adventure_Friends : Player_Adventure
{
    private Collider m_PlayerCollider = null;

    void Awake()
    {
        m_PlayerCollider = GetComponent<Collider>();
    }
    protected override void Attack()
    {
        if (GameManager.Instance.GetPlayer().isStopAction)
            return;

        if (ProjectileClass == null)
            return;

        if (AdventureController == null)
        {
            AdventureController = GetComponent<FollowPlayerController>();
        }

        // 사다리일때는 공격불가
        if (AdventureController.GetOnLadder())
            return;

        // 무력화일때 공격불가
        if (isIncapacitate)
            return;

        ProjectileClass.AdventureAttack();
    }
}
