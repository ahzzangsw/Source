using GameDefines;
using System.Collections.Generic;
using UnityEngine;

public class Boss3 : BossBase
{
    private float Range = 3f;
    protected override void Awake()
    {
        base.Awake();
        iBossSkillPercent = 5;
        RubyCount = 3;
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
                    building.AddBuffActor(BuffType.INCAPACITATE);
                }
            }
        }

        Range += 0.2f;
    }
}
