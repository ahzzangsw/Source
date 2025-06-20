using CharacterDefines;
using GameDefines;
using UnityEngine;

public class FollowPlayerController : AdventureMoveController
{
    private Player_Adventure m_Captain = null;

    private float moveSignDistance = 1f;
    private float maxMoveSignDistance = 4f;

    private float AccelSpeed = 5f;
    private bool IsLadder = false;
    private Vector3 LadderPos = Vector3.zero;

    // Test
#if UNITY_EDITOR
    private bool m_Test = false;
#endif

    protected override void Awake()
    {
        m_Rigidbody = null;

        if (m_Player == null)
        {
            m_Player = GetComponent<Player_Adventure>();
            m_PlayerCollider = GetComponent<Collider>();
            if (m_PlayerCollider)
            {
                m_PlayerCollider.isTrigger = true;
            }
        }

        m_LookPosition = Vector2.zero;

        // Not play game
        if (m_Player == null)
            gameObject.SetActive(false);

        Physics.gravity = new Vector2(0f, m_GravityPower);

        MapBase pMapBase = MapManager.Instance.GetCurrentMapInfo();
        if (pMapBase is Map_Adventure)
        {
            m_MapAdventure = pMapBase as Map_Adventure;
        }
    }

    protected override void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.V))
            m_Test = !m_Test;

        if (m_Test)
            return;
#endif

        if (IsMoveSign() == false)
            return;

        SearchAndFollow();
    }

    public void SetInfo(Player_Adventure captain, PlayerAdventureBasicInfo playerAdventureBasicInfo, ADVLayerType eADVLayerType)
    {
        m_Captain = captain;
        if (m_Captain == null)
        {
            Debug.Log("FollowPlayerController is die because AdventureMoveController is null");
            enabled = false;
            return;
        }

        m_Speed = playerAdventureBasicInfo.moveSpeed;
        m_JumpSpeed = playerAdventureBasicInfo.jumpSpeed;

        m_Player.SetFirstLayerFloor(eADVLayerType);
        m_CurrentLayerFloor = eADVLayerType;

        bFriend = true;
    }

    private bool IsMoveSign()
    {
        if (IsLadder)
            return true;

        float distance = Vector3.Distance(transform.position, m_Captain.transform.position);
        if (distance > moveSignDistance)
            return true;

        m_LookPosition = Vector2.zero;
        m_AnimationState = LAnimationState.Idle;
        m_Player.StopAnimation(false);
        return false;
    }

    private void SearchAndFollow()
    {
        m_LookPosition = Vector2.zero;
        // 플레이어가 다른층에 있는지
        if (m_CurrentLayerFloor != m_Captain.GetCurrentLayerFloor() || IsLadder)
        {
            ChangeLayerFloor();
        }
        else
        {
            m_CurrentLadderType = LadderType.NONE;
            m_LookPosition = (m_Captain.transform.position - transform.position).normalized;
            m_LookPosition = new Vector3(m_LookPosition.x, 0f, 0f);
            if (m_AnimationState != LAnimationState.Running)
            {
                m_Player.StopAnimation(true);
                m_AnimationState = LAnimationState.Running;
                m_Player.StopAnimation(false);
            }

            float fDistance = Vector3.Distance(m_Captain.transform.position, transform.position);
            float speed = fDistance > maxMoveSignDistance ? m_Speed * AccelSpeed : m_Speed;
            GoStraight(m_LookPosition, speed);
        }
    }

    private void ChangeLayerFloor()
    {
        if (m_MapAdventure == null)
            return;

        // 사다리 위치찾기
        bool bUp = (int)(m_CurrentLayerFloor) - (int)(m_Captain.GetCurrentLayerFloor()) < 0;
        if (IsLadder == false)
        {
            LadderPos = m_MapAdventure.GetLaddersNearPosition(m_CurrentLayerFloor, transform.position, bUp, ref m_CurrentLadderType);
        }

        float fDistance = Vector3.Distance(LadderPos, transform.position);
        if (fDistance > 0.4f && IsLadder == false)
        {
            m_LookPosition = (LadderPos - transform.position).normalized;
            m_LookPosition = new Vector3(m_LookPosition.x, 0f, 0f);
            if (m_AnimationState != LAnimationState.Running)
            {
                m_Player.StopAnimation(true);
                m_AnimationState = LAnimationState.Running;
                m_Player.StopAnimation(false);
            }

            fDistance = Vector3.Distance(m_Captain.transform.position, transform.position);
            float speed = fDistance > maxMoveSignDistance ? m_Speed * AccelSpeed : m_Speed;
            GoStraight(m_LookPosition, speed);
        }
        else
        {
            m_LookPosition.y = bUp ? 1f : -1f;
            if (m_LookPosition.y != 0f)
            {
                float fMaxHeight = 99, fMinHeight = -99;
                ADVLayerType eADVLayerType = ADVLayerType.ADVLayerType_None;
                switch (m_CurrentLadderType)
                {
                    case LadderType.FLOOR_1:
                        fMaxHeight = -4f;
                        fMinHeight = -9f;
                        eADVLayerType = bUp ? ADVLayerType.ADVLayerType_2 : ADVLayerType.ADVLayerType_1;
                        break;
                    case LadderType.FLOOR_2_LEFT:
                    case LadderType.FLOOR_2_RIGHT:
                        fMaxHeight = 1f;
                        fMinHeight = -4f;
                        eADVLayerType = bUp ? ADVLayerType.ADVLayerType_3 : ADVLayerType.ADVLayerType_2;
                        break;
                    case LadderType.FLOOR_3_LEFT_1:
                    case LadderType.FLOOR_3_LEFT_2:
                    case LadderType.FLOOR_3_RIGHT_1:
                    case LadderType.FLOOR_3_RIGHT_2:
                        fMaxHeight = 6f;
                        fMinHeight = 1f;
                        eADVLayerType = bUp ? ADVLayerType.ADVLayerType_4 : ADVLayerType.ADVLayerType_3;
                        break;
                }

                bool bTop = false, bBottom = false;
                if (transform.position.y >= fMaxHeight)
                    bTop = true;
                else if (transform.position.y <= fMinHeight)
                    bBottom = true;

                //// 사다리 끝
                if ((m_LookPosition.y > 0 && bTop) || (m_LookPosition.y < 0 && bBottom))
                {
                    m_LookPosition.y = 0f;
                    //ADVLayerType eADVLayerType = GetUpFloor(!bUp);
                    m_Player.SetFirstLayerFloor(eADVLayerType);
                    m_CurrentLayerFloor = eADVLayerType;

                    LadderPos.y = bUp ? fMaxHeight : fMinHeight;
                    transform.position = LadderPos;
                    IsLadder = false;
                    m_LadderState = 0;
                    return;
                }

                IsLadder = true;
                m_LadderState = 2;
                if ((bTop | bBottom) == false)
                {
                    transform.position = new Vector3(LadderPos.x, transform.position.y, transform.position.z);
                }
            }

            if (m_AnimationState != LAnimationState.Climbing)
            {
                m_AnimationState = LAnimationState.Climbing;
                m_Player.StopAnimation(true);
            }
            m_Player.StopAnimation(false);

            float speed = m_Speed * AccelSpeed;
            GoStraight(m_LookPosition, speed);
        }
    }

    private void GoStraight(Vector3 vecGo, float speed)
    {
        m_Player.Turn(m_LookPosition.x);
        transform.position += new Vector3(vecGo.x, vecGo.y, vecGo.z) * speed * Time.deltaTime;

        if (m_AnimationState != LAnimationState.Idle)
            m_Player.SetAnimationState(m_AnimationState);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
    }
    protected override void OnTriggerEnter(Collider collision)
    {
    }
    protected override void OnTriggerExit(Collider collision)
    {
    }
}
