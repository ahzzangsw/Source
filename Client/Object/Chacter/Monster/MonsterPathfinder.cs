using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterDefines;
using GameDefines;

public class MonsterPathfinder : MonoBehaviour
{
    /*Stat*/
    public bool bEnabled { get; set; } = true;
    
    private bool bEndPath = false;

    /*Move*/
    private Vector2 LookVector;
    private Vector2 initialPosition;
    private float startTime = 0f;
    private float journeyFraction = 0f;
    private float journeyLength = 0f;
    private Vector3 beforeKnockbackPosition = Vector3.zero;
    private float totalKnockbackMoved = 0f;

    private Map_Adventure m_MapAdventure = null;
    // 몬스터 목적지는 무조건 1층이다 위를 제외하고 생각하자
    private enum PathStepType
    {
        NONE,
        LADDERDOWN, ///< 사다리내려가기
        FLOORDOWN,  ///< 지형무시
        STAIRDOWN,  ///< 아래지형충돌시까지
        GOSTRAIGHT, ///< 직진
        TELEPORT,   ///< 순간이동
        END,        ///< 끝
    };

    private MonsterAdventure m_Owner = null;

    private struct PathInfo
    {
        public ADVLayerType ownerADVLayerType;
        public Prey target;

        public int curIndex;
        public List<(PathStepType, Vector3)> pathList;

        public bool isBoss;

        public PathInfo(Character Owner, Prey Target)
        {
            ownerADVLayerType = Owner.GetCurrentLayerFloor();
            target = Target;

            isBoss = Owner.m_eClickTargetType == GameDefines.ClickTargetType.FINAL ? true : false;

            curIndex = 0;
            pathList = new List<(PathStepType, Vector3)>();
        }
    };

    private PathInfo m_PathInfo;
    private bool m_bActionEnd = false;

    void Update()
    {
        if (bEnabled == false)
            return;

        if (m_Owner == null)
        {
            bEnabled = false;
            return;
        }

        if (m_Owner.gameObject.activeSelf == false)
            bEnabled = false;

        if (m_Owner.IsDie())
            bEnabled = false;
    }

    void OnEnable()
    {
        // Move
        StartCoroutine(Move());
    }

    public void SetTarget(MonsterAdventure owner, Prey target)
    {
        if (owner == null || target == null)
            return;

        m_Owner = owner;
        if (m_MapAdventure == null)
        {
            MapBase pMapBase = MapManager.Instance.GetCurrentMapInfo();
            if (pMapBase is Map_Adventure)
            {
                m_MapAdventure = pMapBase as Map_Adventure;
            }
        }

        m_PathInfo = new PathInfo(m_Owner, target);
        // Root
        FindAway();
    }

    private IEnumerator Move()
    {
        bEndPath = false;
        foreach ((PathStepType, Vector3) action in m_PathInfo.pathList)
        {
            if (bEnabled == false)
                break;

            m_bActionEnd = false;
            switch ((PathStepType)action.Item1)
            {
                case PathStepType.LADDERDOWN:
                {
                    StartCoroutine(GoLadderDown(action.Item2));
                }
                break;
                case PathStepType.FLOORDOWN:
                {
                    GoFloorDown();
                }
                break;
                case PathStepType.STAIRDOWN:
                {
                    GoStairDown();
                }
                break;
                case PathStepType.GOSTRAIGHT:
                {
                    StartCoroutine(GoStraight(action.Item2, false));
                }
                break;
                case PathStepType.TELEPORT:
                {
                    StartCoroutine(GoTeleport(action.Item2));
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

        if (bEndPath)
        {
            //StartCoroutine(CallAttackStart());
            m_Owner.HitPrey();
        }
    }

    private void FindAway()
    {
        if (m_PathInfo.curIndex < 0)
            return;

        // 1층
        if (m_PathInfo.ownerADVLayerType == m_PathInfo.target.GetCurrentLayerFloor())
        {
            Simulation_Last();
            return;
        }
        // 4층
        else if (m_PathInfo.ownerADVLayerType == ADVLayerType.ADVLayerType_4)
        {
            Simulation_4();
        }
        // 3층
        else if (m_PathInfo.ownerADVLayerType == ADVLayerType.ADVLayerType_3)
        {
            Simulation_3();
        }
        // 2층
        else if (m_PathInfo.ownerADVLayerType == ADVLayerType.ADVLayerType_2)
        {
            Simulation_2();
        }

        FindAway();
    }

    private void Simulation_4()
    {
        Vector3 myPosition = transform.position;
        Vector3[] FloorPosition = new Vector3[] { new Vector3(-10f, 12.5f, 0f), new Vector3(0f, 12.5f, 0f), new Vector3(10f, 12.5f, 0f) };
        foreach (Vector3 pos in FloorPosition)
        {
            if (pos == myPosition)
            {
                myPosition = new Vector3(pos.x, 6f, pos.z);
                m_PathInfo.pathList.Add((PathStepType.GOSTRAIGHT, myPosition));
                break;
            }
        }

        if (m_PathInfo.isBoss)
        {
            Vector3 goalPosition;

            float pivotX = m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, 3).x;
            if (transform.position.x < pivotX)
            {
                goalPosition = m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, 1);
            }
            else if (transform.position.x > pivotX)
            {
                goalPosition = m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, 0);
            }
            else
            {
                goalPosition = m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, Oracle.RandomDice(0, 2));
            }

            m_PathInfo.pathList.Add((PathStepType.GOSTRAIGHT, goalPosition));

            Vector3 goalPosition1 = m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, Oracle.RandomDice(5, 7));
            m_PathInfo.pathList.Add((PathStepType.TELEPORT, goalPosition1));
        }
        else
        {
            Vector3[] goalPosition = new Vector3[] { new Vector3(-14.5f, 6f, 0f), new Vector3(-4.5f, 6f, 0f), new Vector3(4.5f, 6f, 0f), new Vector3(14.5f, 6f, 0f) };
            LadderType[] posList = new LadderType[] { LadderType.FLOOR_3_LEFT_1, LadderType.FLOOR_3_LEFT_2, LadderType.FLOOR_3_RIGHT_1, LadderType.FLOOR_3_RIGHT_2 };

            int index = 0;
            float compareValue = (myPosition - m_MapAdventure.GetLaddersPosition(LadderType.FLOOR_3_LEFT_1)).magnitude;
            for (int i = 1; i < posList.Length; ++i)
            {
                float fValue = (myPosition - m_MapAdventure.GetLaddersPosition(posList[i])).magnitude;
                if (fValue < compareValue)
                {
                    index = i;
                    compareValue = fValue;
                }
                else if (fValue == compareValue)
                {
                    if (Oracle.RandomDice(0, 2) == 0)
                        index = i;
                }
            }

            m_PathInfo.pathList.Add((PathStepType.GOSTRAIGHT, goalPosition[index]));
            m_PathInfo.pathList.Add((PathStepType.LADDERDOWN, goalPosition[index]));
        }

        m_PathInfo.ownerADVLayerType = ADVLayerType.ADVLayerType_3;
        m_PathInfo.curIndex++;
    }
    private void Simulation_3()
    {
        Vector3 myPosition;
        if (m_PathInfo.pathList.Count > 0)
        {
            myPosition = m_PathInfo.pathList[m_PathInfo.pathList.Count - 1].Item2;
        }
        else
        {
            myPosition = transform.position;
        }

        if (m_PathInfo.isBoss)
        {
            Vector3 goalPosition;

            if (myPosition.x < 0)
            {
                goalPosition = m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, 6);
            }
            else
            {
                goalPosition = m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, 5);
            }

            m_PathInfo.pathList.Add((PathStepType.GOSTRAIGHT, goalPosition));

            Vector3 goalPosition1 = m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, Oracle.RandomDice(7, 9));
            m_PathInfo.pathList.Add((PathStepType.TELEPORT, goalPosition1));
        }
        else
        {
            Vector3[] goalPosition = new Vector3[] { new Vector3(-9f, 1f, 0f), new Vector3(9f, 1f, 0f) };
            float left = (myPosition - m_MapAdventure.GetLaddersPosition(LadderType.FLOOR_2_LEFT)).magnitude;
            float right = (myPosition - m_MapAdventure.GetLaddersPosition(LadderType.FLOOR_2_RIGHT)).magnitude;

            int index;
            if (left > right)
                index = 1;
            else
                index = 0;

            m_PathInfo.pathList.Add((PathStepType.GOSTRAIGHT, goalPosition[index]));
            m_PathInfo.pathList.Add((PathStepType.LADDERDOWN, goalPosition[index]));
        }

        m_PathInfo.ownerADVLayerType = ADVLayerType.ADVLayerType_2;
        m_PathInfo.curIndex++;
    }
    private void Simulation_2()
    {
        Vector3 myPosition;
        if (m_PathInfo.pathList.Count > 0)
        {
            myPosition = m_PathInfo.pathList[m_PathInfo.pathList.Count - 1].Item2;
        }
        else
        {
            myPosition = transform.position;
        }

        if (m_PathInfo.isBoss)
        {
            Vector3 goalPosition;
            if (myPosition.x < 0)
            {
                goalPosition = m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, 8);
            }
            else
            {
                goalPosition = m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, 7);
            }

            m_PathInfo.pathList.Add((PathStepType.GOSTRAIGHT, goalPosition));

            Vector3 goalPosition1 = m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, Oracle.RandomDice(9, 11));
            m_PathInfo.pathList.Add((PathStepType.TELEPORT, goalPosition1));
        }
        else
        {
            Vector3 goalPosition = new Vector3(0, -4f, 0f);
            m_PathInfo.pathList.Add((PathStepType.GOSTRAIGHT, goalPosition));
            m_PathInfo.pathList.Add((PathStepType.LADDERDOWN, goalPosition));
        }

        m_PathInfo.curIndex++;
        m_PathInfo.ownerADVLayerType = m_PathInfo.target.GetCurrentLayerFloor();
    }
    private void Simulation_Last()
    {
        m_PathInfo.curIndex = -1;

        Vector3 vArrivalPosition = m_PathInfo.target.GetLastPosition();

        if (m_PathInfo.pathList.Count == 0)
        {
            vArrivalPosition = new Vector3(vArrivalPosition.x - 0.5f, vArrivalPosition.y, vArrivalPosition.z);
        }
        else
        {
            if (vArrivalPosition.x == 0)
            {

            }
            else if (vArrivalPosition.x < m_PathInfo.pathList[m_PathInfo.pathList.Count - 1].Item2.x)
                vArrivalPosition = new Vector3(vArrivalPosition.x + 0.5f, vArrivalPosition.y, vArrivalPosition.z);
            else
                vArrivalPosition = new Vector3(vArrivalPosition.x - 0.5f, vArrivalPosition.y, vArrivalPosition.z);
        }

        m_PathInfo.pathList.Add((PathStepType.END, vArrivalPosition)); 
    }

    private IEnumerator GoLadderDown(Vector3 vecDestination)
    {
        vecDestination = new Vector3(vecDestination.x, vecDestination.y - 5f, vecDestination.z);

        journeyFraction = 0f;
        initialPosition = transform.position;
        journeyLength = Vector2.Distance(initialPosition, vecDestination);
        startTime = Time.time;

        while (journeyFraction < 1.0f)
        {
            if (bEnabled == false)
                break;

            if (m_Owner.bKnockback)
            {
                bool bReset = true;
                if (beforeKnockbackPosition == Vector3.zero)
                {
                    beforeKnockbackPosition = transform.position;
                }

                Vector3 knockbackDirection = (transform.position - vecDestination).normalized;
                transform.position += knockbackDirection * Time.deltaTime * 2f;

                float fKnockbackMoved = Vector3.Distance(beforeKnockbackPosition, transform.position);
                totalKnockbackMoved += fKnockbackMoved;
                if (totalKnockbackMoved < 1f)
                {
                    bReset = false;
                }

                if (bReset)
                {
                    m_Owner.bKnockback = false;
                    beforeKnockbackPosition = Vector2.zero;
                    totalKnockbackMoved = 0f;

                    initialPosition = transform.position;
                    journeyLength = Vector2.Distance(initialPosition, vecDestination);
                    startTime = Time.time;
                }
            }
            else
            {
                if (m_Owner.bSlow)
                {
                    initialPosition = transform.position;
                    journeyLength = Vector2.Distance(initialPosition, vecDestination);
                    startTime = Time.time;
                    m_Owner.bSlow = false;
                }

                if (m_Owner.bStun)
                {
                    initialPosition = transform.position;
                    journeyLength = Vector2.Distance(initialPosition, vecDestination);
                    startTime = Time.time;
                }
                else
                {
                    float distanceCovered = (Time.time - startTime) * (m_Owner.m_eClickTargetType == GameDefines.ClickTargetType.FINAL ? 20 : m_Owner.moveSpeed);
                    journeyFraction = journeyLength != 0 ? distanceCovered / journeyLength : 1;

                    LookVector = (vecDestination - transform.position).normalized;
                    if (LookVector.y != 0)
                    {
                        m_Owner.SetAnimationState(LAnimationState.Climbing);
                    }

                    transform.position = Vector2.Lerp(initialPosition, vecDestination, journeyFraction);
                }
            }
            
            yield return null;
        }

        m_bActionEnd = true;
    }

    private IEnumerator GoFloorDown()
    {
        yield return null;
    }

    private IEnumerator GoStairDown()
    {
        yield return null;
    }

    private IEnumerator GoStraight(Vector3 vecDestination, bool bEndDestination)
    {
        journeyFraction = 0f;
        initialPosition = transform.position;
        journeyLength = Vector2.Distance(initialPosition, vecDestination);
        startTime = Time.time;

        while (journeyFraction < 1.0f)
        {
            if (bEnabled == false)
            {
                bEndDestination = false;
                break;
            }

            if (m_Owner.bKnockback)
            {
                bool bReset = true;
                if (beforeKnockbackPosition == Vector3.zero)
                {
                    beforeKnockbackPosition = transform.position;
                }

                Vector3 knockbackDirection = (transform.position - vecDestination).normalized;
                transform.position += knockbackDirection * Time.deltaTime * 2f;

                float fKnockbackMoved = Vector3.Distance(beforeKnockbackPosition, transform.position);
                totalKnockbackMoved += fKnockbackMoved;
                if (totalKnockbackMoved < 1f)
                {
                    bReset = false;
                }

                if (bReset)
                {
                    m_Owner.bKnockback = false;
                    beforeKnockbackPosition = Vector2.zero;
                    totalKnockbackMoved = 0f;

                    initialPosition = transform.position;
                    journeyLength = Vector2.Distance(initialPosition, vecDestination);
                    startTime = Time.time;
                }
            }
            else
            {
                if (m_Owner.bSlow)
                {
                    initialPosition = transform.position;
                    journeyLength = Vector2.Distance(initialPosition, vecDestination);
                    startTime = Time.time;
                    m_Owner.bSlow = false;
                }

                if (m_Owner.bStun)
                {
                    initialPosition = transform.position;
                    journeyLength = Vector2.Distance(initialPosition, vecDestination);
                    startTime = Time.time;
                }
                else
                {
                    float distanceCovered = (Time.time - startTime) * m_Owner.moveSpeed;
                    journeyFraction = journeyLength != 0 ? distanceCovered / journeyLength : 1;

                    LookVector = (vecDestination - transform.position).normalized;
                    if (LookVector.x != 0)
                    {
                        m_Owner.Turn(LookVector.x);
                        m_Owner.SetAnimationState(LAnimationState.Running);
                    }
                    else if (LookVector.y != 0)
                    {
                        m_Owner.UpAndDown(0f);
                    }

                    transform.position = Vector2.Lerp(initialPosition, vecDestination, journeyFraction);
                }
            }

            yield return null;
        }

        if (bEndDestination)
        {
            bEndPath = true;
        }

        m_bActionEnd = true;
    }

    private IEnumerator GoTeleport(Vector3 vecDestination)
    {
        if (bEnabled)
        {
            transform.position = vecDestination;
            yield return null;
        }
        m_bActionEnd = true;
    }

    private IEnumerator CallAttackStart()
    {
        while (true)
        {
            m_Owner.HitPrey();
            yield return new WaitForSeconds(1f);
        }
    }

    public void TeleportationEndPosition()
    {
        bEnabled = false;
        m_bActionEnd = true;

        StopCoroutine("GoStraight");
        StopCoroutine("GoLadderDown");

        transform.position = m_PathInfo.pathList[m_PathInfo.pathList.Count - 1].Item2;
        //StartCoroutine(CallAttackStart());
    }
}