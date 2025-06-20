using System.Collections.Generic;
using UnityEngine;
using CharacterDefines;
using GameDefines;
using UnityEngine.UIElements;

public class Map_Adventure : MapBase
{
    [SerializeField] private GameObject[] FloorObjects = null;
    [SerializeField] private GameObject[] StairsObjects = null;
    [SerializeField] private GameObject[] LaddersObjects = null;

    [SerializeField] private Vector3[] FloorLeftEndPosition = null;
    [SerializeField] private Vector3[] FloorRightEndPosition = null;

    private Dictionary<ADVLayerType, Collider> FloorColliderList = null;
    private Dictionary<StairType, List<Collider>> StairColliderList = null;
    private Dictionary<LadderType, Collider> LadderColliderList = null;
    private Dictionary<LadderType, (Vector3, Vector3)> LadderPositionList = null;

    protected override void Awake()
    {
        FloorColliderList = new Dictionary<ADVLayerType, Collider>();
        StairColliderList = new Dictionary<StairType, List<Collider>>();
        LadderColliderList = new Dictionary<LadderType, Collider>();

        for (int i = 0; i < FloorObjects.Length; ++i)
        {
            GameObject Floors = FloorObjects[i];
            if (Floors == null)
                continue;

            Collider tempCollider = Floors.GetComponent<Collider>();
            if (tempCollider == null)
                continue;

            switch (i)
            {
                case 0:
                    FloorColliderList.Add(ADVLayerType.ADVLayerType_1, tempCollider);
                    break;
                case 1:
                    FloorColliderList.Add(ADVLayerType.ADVLayerType_2, tempCollider);
                    break;
                case 2:
                    FloorColliderList.Add(ADVLayerType.ADVLayerType_3, tempCollider);
                    break;
                case 3:
                    FloorColliderList.Add(ADVLayerType.ADVLayerType_4, tempCollider);
                    break;
            };
        }

        for (int i = 0; i < StairsObjects.Length; ++i)
        {
            GameObject Stair = StairsObjects[i];
            if (Stair == null)
                continue;

            List<Collider> tempColliderList = new List<Collider>();

            Collider[] colliders = Stair.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                if (collider.isTrigger)
                    tempColliderList.Add(collider);
            }

            StairType eStairType = (StairType)i;
            StairColliderList.Add(eStairType, tempColliderList);
        }

        for (int i = 0; i < LaddersObjects.Length; ++i)
        {
            GameObject Ladder = LaddersObjects[i];
            if (Ladder == null)
                continue;

            Collider tempCollider = Ladder.GetComponent<Collider>();
            if (tempCollider == null)
                continue;

            LadderType eLadderType = (LadderType)i;
            LadderColliderList.Add(eLadderType, tempCollider);
        }

        if (FloorObjects.Length != (int)(ADVLayerType.ADVLayerType_Max - ADVLayerType.ADVLayerType_None) - 1)
        {
            enabled = false;
        }

        // 사다리 위치 (Define, (Up, Down))
        LadderPositionList = new Dictionary<LadderType, (Vector3, Vector3)>
        {
            { LadderType.FLOOR_1, (new Vector3(0f, -4f, 0f), new Vector3(0f, -9f, 0f)) },
            { LadderType.FLOOR_2_LEFT, (new Vector3(-9f, 1f, 0f), new Vector3(-9f, -4f, 0f)) },
            { LadderType.FLOOR_2_RIGHT, (new Vector3(9f, 1f, 0f), new Vector3(9f, -4f, 0f)) },
            { LadderType.FLOOR_3_LEFT_1, (new Vector3(-14.5f, 6f, 0f), new Vector3(-14.5f, 1f, 0f)) },
            { LadderType.FLOOR_3_LEFT_2, (new Vector3(-4.5f, 6f, 0f), new Vector3(-4.5f, 1f, 0f)) },
            { LadderType.FLOOR_3_RIGHT_1, (new Vector3(4.5f, 6f, 0f), new Vector3(4.5f, 1f, 0f)) },
            { LadderType.FLOOR_3_RIGHT_2, (new Vector3(14.5f, 6f, 0f), new Vector3(14.5f, 1f, 0f)) },
        };
    }

    public float GetFloorColliderPositionY(ADVLayerType eADVLayerType)
    {
        float outResultPositionY = 0f;
        if (FloorColliderList.ContainsKey(eADVLayerType))
        {
            Bounds tempBounds = FloorColliderList[eADVLayerType].bounds;
            outResultPositionY = tempBounds.center.y + 0.5f;
        }

        return outResultPositionY;
    }

    public void IgnoreCollisionPlayer_Floor(ADVLayerType eADVLayerType, Collider ObjectCollider, bool bIgnore)
    {
        if (FloorColliderList.ContainsKey(eADVLayerType) == false)
            return;

        Physics.IgnoreCollision(ObjectCollider, FloorColliderList[eADVLayerType], bIgnore);
    }
    public void TriggerCollisionPlayer_Floor(ADVLayerType eADVLayerType, Collider ObjectCollider, bool bTrigger)
    {
        if (FloorColliderList.ContainsKey(eADVLayerType) == false)
            return;

        FloorColliderList[eADVLayerType].isTrigger = bTrigger;
    }

    public void IgnoreCollisionPlayer_Stair(StairType eStairType, Collider ObjectCollider, bool bIgnore)
    {
        if (StairColliderList == null || StairColliderList.Count == 0)
            return;

        if (StairColliderList.ContainsKey(eStairType) == false)
            return;

        for (int i = 0; i < StairColliderList[eStairType].Count; ++i)
        {
            Collider StairCollider = StairColliderList[eStairType][i];
            Physics.IgnoreCollision(ObjectCollider, StairCollider, bIgnore);
        }
    }

    public void TriggerCollisionPlayer_Ladder(LadderType eLadderType, bool bTrigger)
    {
        if (LadderColliderList.ContainsKey(eLadderType) == false)
            return;

        Collider LadderCollider = LadderColliderList[eLadderType];
        if (LadderCollider == null)
            return;

        LadderCollider.isTrigger = bTrigger;
    }

    public Vector3 GetStairsPosition(StairType eStairType, int index = 0)
    {
        if (StairColliderList == null || StairColliderList.Count == 0)
            return TypeDefs.VECTOR_NONE;

        if (StairColliderList.ContainsKey(eStairType) == false)
            return TypeDefs.VECTOR_NONE;

        return StairColliderList[eStairType][index].transform.position;
    }

    public Vector3 GetLaddersPosition(LadderType eLadderType)
    {
        if (LaddersObjects == null || LaddersObjects.Length == 0)
            return TypeDefs.VECTOR_NONE;

        int index = (int)eLadderType;
        if (index < 0 || index >= LaddersObjects.Length)
            return TypeDefs.VECTOR_NONE;

        return LaddersObjects[index].transform.position;
    }

    public Vector3 GetLaddersPosition(LadderType eLadderType, bool bUp)
    {
        if (LadderPositionList == null || LadderPositionList.Count == 0)
            return TypeDefs.VECTOR_NONE;

        if (LadderPositionList.ContainsKey(eLadderType) == false)
            return TypeDefs.VECTOR_NONE;

        return bUp ? LadderPositionList[eLadderType].Item1 : LadderPositionList[eLadderType].Item2;
    }

    public Vector3 GetLaddersNearPosition(ADVLayerType eADVLayerType, Vector3 position, bool bUp, ref LadderType eOutLadderType)
    {
        eOutLadderType = LadderType.NONE;
        if (LadderPositionList == null || LadderPositionList.Count == 0)
            return TypeDefs.VECTOR_NONE;

        List<LadderType> posList = new List<LadderType>();
        Vector3 targetPosition = TypeDefs.VECTOR_NONE;
        switch (eADVLayerType)
        {
            case ADVLayerType.ADVLayerType_1:
                {
                    if (bUp)
                    {
                        posList.Add(LadderType.FLOOR_1);
                    }
                }
                break;
            case ADVLayerType.ADVLayerType_2:
                {
                    if (bUp)
                    {
                        posList.Add(LadderType.FLOOR_2_LEFT);
                        posList.Add(LadderType.FLOOR_2_RIGHT);
                    }
                    else
                    {
                        posList.Add(LadderType.FLOOR_1);
                    }
                }
                break;
            case ADVLayerType.ADVLayerType_3:
                {
                    if (bUp)
                    {
                        posList.Add(LadderType.FLOOR_3_LEFT_1);
                        posList.Add(LadderType.FLOOR_3_LEFT_2);
                        posList.Add(LadderType.FLOOR_3_RIGHT_1);
                        posList.Add(LadderType.FLOOR_3_RIGHT_2);
                    }
                    else
                    {
                        posList.Add(LadderType.FLOOR_2_LEFT);
                        posList.Add(LadderType.FLOOR_2_RIGHT);
                    }
                }
                break;
            case ADVLayerType.ADVLayerType_4:
                {
                    if (!bUp)
                    {
                        posList.Add(LadderType.FLOOR_3_LEFT_1);
                        posList.Add(LadderType.FLOOR_3_LEFT_2);
                        posList.Add(LadderType.FLOOR_3_RIGHT_1);
                        posList.Add(LadderType.FLOOR_3_RIGHT_2);
                    }
                }
                break;
        }

        float fMinDistance = Mathf.Infinity;
        for (int i = 0; i < posList.Count; ++i)
        {
            LadderType eLadderType = posList[i];
            (Vector3, Vector3) LadderUpDownPosition = LadderPositionList[eLadderType];

            Vector3 curPos = bUp ? LadderUpDownPosition.Item2 : LadderUpDownPosition.Item1;

            float fDistance = Vector3.Distance(curPos, position);
            if (fMinDistance > fDistance)
            {
                fMinDistance = fDistance;
                targetPosition = curPos;
                eOutLadderType = eLadderType;
            }
        }

        return targetPosition;
    }

    public Vector3 GetSpawnPointPosition(AdventureLevelType eAdventureLevelType, int index)
    {
        if (spawnPointList == null)
            return Vector3.zero;

        if (spawnPointList.Length == 0)
            return Vector3.zero;

        switch (eAdventureLevelType)
        {
            case AdventureLevelType.LEVEL1:
                break;
            case AdventureLevelType.LEVEL2:
                break;
            case AdventureLevelType.LEVEL3:
                break;
            case AdventureLevelType.LEVEL4:
                break;
        }

        return spawnPointList[index].position;
    }

    public ADVLayerType GetSpawnPointADVLayerType(int index)
    {
        if (index < 5)
            return ADVLayerType.ADVLayerType_4;
        else if (index < 7)
            return ADVLayerType.ADVLayerType_3;
        else if (index < 9)
            return ADVLayerType.ADVLayerType_2;
        else
            return ADVLayerType.ADVLayerType_1;
    }

    public Vector3 GetFloorBothEnds(ADVLayerType eADVLayerType, bool bLeft)
    {
        if (FloorLeftEndPosition.Length < 4 || FloorRightEndPosition.Length < 4)
            return TypeDefs.VECTOR_NONE;

        switch (eADVLayerType)
        {
            case ADVLayerType.ADVLayerType_1:
                return bLeft ? FloorLeftEndPosition[0] : FloorRightEndPosition[0];
            case ADVLayerType.ADVLayerType_2:
                return bLeft ? FloorLeftEndPosition[1] : FloorRightEndPosition[1];
            case ADVLayerType.ADVLayerType_3:
                return bLeft ? FloorLeftEndPosition[2] : FloorRightEndPosition[2];
            case ADVLayerType.ADVLayerType_4:
                return bLeft ? FloorLeftEndPosition[3] : FloorRightEndPosition[3];
        }

        return TypeDefs.VECTOR_NONE;
    }
}
