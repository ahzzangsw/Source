using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss7 : BossBase
{
    protected override void Awake()
    {
        base.Awake();
        iBossSkillPercent = 20;
        RubyCount = 7;
    }

    protected override void DoInterrupt()
    {
        moveSpeed += 0.1f;
        m_BuffContainer.UpdateMonsterInfo(moveSpeed);
    }

    protected override void ClearInterrupt()
    {
        bInterrupt = false;
    }
}
