using CharacterDefines;
using DG.Tweening;
using GameDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyShockWave : WeaponBase
{
    private Map_Adventure m_MapAdventure = null;
    private bool m_GotoRoad = false;
    private bool m_Hijacking = false;
    private float m_fAdjustment = 0f;

    private enum PathStepType
    {
        NONE,
        LADDERDOWN, ///< 餌棻葬頂溥陛晦
        LADDERUP,   ///< 餌棻葬螢塭陛晦
        GOSTRAIGHT, ///< 霜霞
        END,        ///< 部
    };

    private enum DirectionType
    {
        UPLEFT,
        UPRIGHT,
        DOWNLEFT,
        DOWNRIGHT,
    }

    private struct PathInfo
    {
        public int curIndex;
        public ADVLayerType ownerADVLayerType;

        public List<(PathStepType, Vector3)> pathList;

        public PathInfo(ADVLayerType Owner)
        {
            curIndex = 0;
            ownerADVLayerType = Owner;
            pathList = new List<(PathStepType, Vector3)>();
        }
    }

    private PathInfo m_PathInfo;
    private DirectionType m_eDirectionType;
    private bool m_bActionEnd = false;
    private Vector3 initialPosition = Vector3.zero;
    private float startTime = 0f;
    private float journeyFraction = 0f;
    private float journeyLength = 0f;

    protected override void Clear()
    {
        base.Clear();

        m_GotoRoad = false;
    }

    public override void SetInfo(BossAdventure_Last_Skill master, Transform target, Vector3 weaponDirection)
    {
        m_Target = target;
        direction = weaponDirection;
        m_GotoRoad = true;
        m_Damage = master.m_Damage;

        if (m_MapAdventure == null)
        {
            MapBase pMapBase = MapManager.Instance.GetCurrentMapInfo();
            if (pMapBase is Map_Adventure)
            {
                m_MapAdventure = pMapBase as Map_Adventure;
            }
        }

        Vector3 vecCurrentPosition = master.transform.position;
        if (vecCurrentPosition.x < 0)
        {
            m_eDirectionType = vecCurrentPosition.y < 0 ? DirectionType.UPLEFT : DirectionType.DOWNLEFT;
        }
        else
        {
            m_eDirectionType = vecCurrentPosition.y < 0 ? DirectionType.UPRIGHT : DirectionType.DOWNRIGHT;
        }

        ADVLayerType eADVLayerType;
        if (m_eDirectionType == DirectionType.UPLEFT || m_eDirectionType == DirectionType.UPRIGHT)
            eADVLayerType = ADVLayerType.ADVLayerType_1;
        else
            eADVLayerType = ADVLayerType.ADVLayerType_4;

        m_PathInfo = new PathInfo(eADVLayerType);
        FindAway();
    }

    protected override void Awake()
    {
        m_eWeaponType = WeaponType.ENEMYSHOCKWAVE;
    }

    protected override void FixedUpdate()
    {
        if (!bEnableUpdate)
            return;

        if (m_GotoRoad)
        {
            m_GotoRoad = false;
            StartCoroutine(Move());
        }

        if (m_Hijacking)
        {
            m_Hijacking = false;

            //Player MyPlayer = GameManager.Instance.GetPlayer();
            //if (MyPlayer)
            //{
            //    MyPlayer.isStopAction = false;
            //}
            
            //if (m_Target)
            //{
            //    float[] randomPosY = new float[] { -8, -3, 2, 7 };
            //    float randomPosX = Oracle.RandomDice(-17, 18);
            //    m_Target.transform.position = new Vector3(randomPosX, randomPosY[Oracle.RandomDice(0, 4)], 0);
            //}
        }
    }

    protected override void OnTriggerEnter(Collider collision)
    {
        if (collision.transform != m_Target)
            return;

        if (BossAdventure_Last_Manager.Instance.m_bDie)
            return;

        Player MyPlayer = GameManager.Instance.GetPlayer();
        if (MyPlayer == null)
            return;

        m_Hijacking = true;
        MyPlayer.ReduceHP(m_Damage);
        //MyPlayer.isStopAction = true;

        if (m_Target)
        {
            float[] randomPosY = new float[] { -8, -3, 2, 7 };
            float randomPosX = Oracle.RandomDice(-17, 18);
            m_Target.transform.position = new Vector3(randomPosX, randomPosY[Oracle.RandomDice(0, 4)], 0);
        }

        DestroyPool();
    }

    public override void DestroyPool()
    {
        //if (m_Hijacking)
        //{
        //    Player MyPlayer = GameManager.Instance.GetPlayer();
        //    if (MyPlayer)
        //        MyPlayer.isStopAction = false;
        //}

        base.DestroyPool();
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private void FindAway()
    {
        if (m_PathInfo.curIndex < 0)
            return;

        if (m_eDirectionType == DirectionType.UPLEFT || m_eDirectionType == DirectionType.UPRIGHT)
        {
            // 4類
            if (m_PathInfo.ownerADVLayerType == ADVLayerType.ADVLayerType_4)
            {
                Simulation_Last();
                return;
            }
            // 3類
            else if (m_PathInfo.ownerADVLayerType == ADVLayerType.ADVLayerType_3)
            {
                LadderType[] LadderTypeList = new LadderType[] { LadderType.FLOOR_3_LEFT_1, LadderType.FLOOR_3_LEFT_2, LadderType.FLOOR_3_RIGHT_1, LadderType.FLOOR_3_RIGHT_2 };
                Simulation(LadderTypeList, ADVLayerType.ADVLayerType_4, false);
            }
            // 2類
            else if (m_PathInfo.ownerADVLayerType == ADVLayerType.ADVLayerType_2)
            {
                LadderType[] LadderTypeList = new LadderType[] { LadderType.FLOOR_2_LEFT, LadderType.FLOOR_2_RIGHT };
                Simulation(LadderTypeList, ADVLayerType.ADVLayerType_3, false);
            }
            // 1類
            else if (m_PathInfo.ownerADVLayerType == ADVLayerType.ADVLayerType_1)
            {
                LadderType[] LadderTypeList = new LadderType[] { LadderType.FLOOR_1 };
                Simulation(LadderTypeList, ADVLayerType.ADVLayerType_2, false);
            }
        }
        else
        {
            // 1類
            if (m_PathInfo.ownerADVLayerType == ADVLayerType.ADVLayerType_1)
            {
                Simulation_Last();
                return;
            }
            // 4類
            else if (m_PathInfo.ownerADVLayerType == ADVLayerType.ADVLayerType_4)
            {
                LadderType[] LadderTypeList = new LadderType[] { LadderType.FLOOR_3_LEFT_1, LadderType.FLOOR_3_LEFT_2, LadderType.FLOOR_3_RIGHT_1, LadderType.FLOOR_3_RIGHT_2 };
                Simulation(LadderTypeList, ADVLayerType.ADVLayerType_3, true);
            }
            // 3類
            else if (m_PathInfo.ownerADVLayerType == ADVLayerType.ADVLayerType_3)
            {
                LadderType[] LadderTypeList = new LadderType[] { LadderType.FLOOR_2_LEFT, LadderType.FLOOR_2_RIGHT };
                Simulation(LadderTypeList, ADVLayerType.ADVLayerType_2, true);
            }
            // 2類
            else if (m_PathInfo.ownerADVLayerType == ADVLayerType.ADVLayerType_2)
            {
                LadderType[] LadderTypeList = new LadderType[] { LadderType.FLOOR_1 };
                Simulation(LadderTypeList, ADVLayerType.ADVLayerType_1, true);
            }
        }

        FindAway();
    }
    private void Simulation(LadderType[] LadderTypeList, ADVLayerType eADVLayerType, bool bUp)
    {
        int index = Oracle.RandomDice(0, LadderTypeList.Length);

        Vector3 goalPosition = m_MapAdventure.GetLaddersPosition(LadderTypeList[index], bUp);
        goalPosition.y += m_fAdjustment;
        m_PathInfo.pathList.Add((PathStepType.GOSTRAIGHT, goalPosition));
        m_PathInfo.pathList.Add((bUp ? PathStepType.LADDERDOWN : PathStepType.LADDERUP, goalPosition));

        m_PathInfo.ownerADVLayerType = eADVLayerType;
        m_PathInfo.curIndex++;
    }

    private void Simulation_Last()
    {
        m_PathInfo.curIndex = -1;

        Vector3 vecMyPosition = transform.position;
        switch (m_eDirectionType)
        { 
            case DirectionType.UPLEFT:
                vecMyPosition = new Vector3(-18f, 6f, 0f);
                break;
            case DirectionType.UPRIGHT:
                vecMyPosition = new Vector3(18f, 6f, 0f);
                break;
            case DirectionType.DOWNLEFT:
                vecMyPosition = new Vector3(-18f, -9f, 0f);
                break;
            case DirectionType.DOWNRIGHT:
                vecMyPosition = new Vector3(18f, -9f, 0f);
                break;
        }

        vecMyPosition.y += m_fAdjustment;
        m_PathInfo.pathList.Add((PathStepType.END, vecMyPosition));
    }

    private IEnumerator Move()
    {
        foreach ((PathStepType, Vector3) action in m_PathInfo.pathList)
        {
            m_bActionEnd = false;
            switch ((PathStepType)action.Item1)
            {
                case PathStepType.LADDERDOWN:
                    {
                        StartCoroutine(GoLadderDown(action.Item2, false));
                    }
                    break;
                case PathStepType.LADDERUP:
                    {
                        StartCoroutine(GoLadderDown(action.Item2, true));
                    }
                    break;
                case PathStepType.GOSTRAIGHT:
                    {
                        StartCoroutine(GoStraight(action.Item2, false));
                    }
                    break;
                case PathStepType.END:
                    {
                        StartCoroutine(GoStraight(action.Item2, true));
                    }
                    break;
            };

            yield return new WaitUntil(() => m_bActionEnd);
        }
    }

    private IEnumerator GoLadderDown(Vector3 vecDestination, bool bUp)
    {
        vecDestination = new Vector3(vecDestination.x, vecDestination.y, vecDestination.z);
        vecDestination.y += (bUp ? 5f : -5f);

        journeyFraction = 0f;
        initialPosition = transform.position;
        journeyLength = Vector3.Distance(initialPosition, vecDestination);
        startTime = Time.time;

        while (journeyFraction < 1.0f)
        {
            float distanceCovered = Time.deltaTime * moveSpeed;
            journeyFraction += journeyLength != 0 ? distanceCovered / journeyLength : 0;
            transform.position = Vector3.Lerp(initialPosition, vecDestination, journeyFraction);

            //float distanceCovered = (Time.time - startTime) * moveSpeed;
            //journeyFraction = journeyLength != 0 ? distanceCovered / journeyLength : 0;
            //transform.position = Vector3.Lerp(initialPosition, vecDestination, journeyFraction);
            yield return null;
        }

        transform.position = vecDestination;
        m_bActionEnd = true;
    }

    private IEnumerator GoStraight(Vector3 vecDestination, bool bEnd)
    {
        journeyFraction = 0f;
        initialPosition = transform.position;
        journeyLength = Vector3.Distance(initialPosition, vecDestination);
        startTime = Time.time;

        while (journeyFraction < 1.0f)
        {
            float distanceCovered = Time.deltaTime * moveSpeed;
            journeyFraction += journeyLength != 0 ? distanceCovered / journeyLength : 0;
            transform.position = Vector3.Lerp(initialPosition, vecDestination, journeyFraction);

            //float distanceCovered = (Time.time - startTime) * moveSpeed;
            //journeyFraction = journeyLength != 0 ? distanceCovered / journeyLength : 0;
            //transform.position = Vector3.Lerp(initialPosition, vecDestination, journeyFraction);
            yield return null;
        }

        transform.position = vecDestination;
        m_bActionEnd = true;

        if (bEnd)
        {
            DestroyPool();
            yield break;
        }
    }
}
