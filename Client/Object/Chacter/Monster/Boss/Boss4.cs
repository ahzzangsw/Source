using GameDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss4 : BossBase
{
    private float Range = 5f;
    protected override void Awake()
    {
        base.Awake();
        iBossSkillPercent = 10;
        RubyCount = 4;
    }

    protected override void DoInterrupt()
    {
        List<GameObject> buildingList = BuildingPool.Instance.GetBuildingList();
        if (buildingList != null)
        {
            for (int i = 0; i < buildingList.Count; ++i)
            {
                GameObject buildingObject = buildingList[i];
                float distance = Vector3.Distance(buildingObject.transform.position, transform.position);
                if (distance <= Range)
                {
                    Building building = buildingObject.GetComponent<Building>();
                    building.AddBuffActor(BuffType.REDUCING);
                }
            }
        }

        Range += 0.2f;
    }
}
