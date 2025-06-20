using System.Collections;
using System.Collections.Generic;
using UIDefines;
using UnityEngine;

public class Boss6 : BossBase
{
    private int maxDefense = 95;
    protected override void Awake()
    {
        base.Awake();
        iBossSkillPercent = 10;
        RubyCount = 6;
    }

    protected override void DoInterrupt()
    {
        Defense += 1;
        if (Defense > maxDefense)
            Defense = maxDefense;
    }
}
