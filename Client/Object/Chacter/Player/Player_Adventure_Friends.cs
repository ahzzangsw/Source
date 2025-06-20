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

        // ��ٸ��϶��� ���ݺҰ�
        if (AdventureController.GetOnLadder())
            return;

        // ����ȭ�϶� ���ݺҰ�
        if (isIncapacitate)
            return;

        ProjectileClass.AdventureAttack();
    }
}
