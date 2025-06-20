using CharacterDefines;
using GameDefines;
using System.Collections;
using UnityEngine;

public class AdventureMoveController : MonoBehaviour
{
    public float m_GravityPower = -20f;

    protected Rigidbody m_Rigidbody = null;
    protected Transform m_LadderTransform = null;
    protected Collider m_PlayerCollider = null;

    protected ADVLayerType m_CurrentLayerFloor = ADVLayerType.ADVLayerType_None;
    protected LadderType m_CurrentLadderType = LadderType.NONE;
    private int m_IgnoreFloor = -1;
    // Trigger
    protected ADVLayerType m_eLayerType_TriggerEnter = ADVLayerType.ADVLayerType_None;
    protected ADVLayerType m_eLayerType_CollisionEnter = ADVLayerType.ADVLayerType_None; 

    protected Player_Adventure m_Player;
    protected LAnimationState m_AnimationState = LAnimationState.Ready;

    protected Map_Adventure m_MapAdventure = null;
    private bool m_IsUpStair = false;
    protected bool m_IsJumpOff = false;
    protected bool m_IsJump = false;
    protected int m_LadderState = 0;                                              // 현재 사다리 상태
    private bool m_IsFalling = false;
    private StairType m_CurrentStairType = StairType.NONE;

    protected Vector2 m_LookPosition;
    [SerializeField] protected float m_Speed = 3f;
    [SerializeField] protected float m_JumpSpeed = 8f;
    protected float m_BuffSpeed = 1f;
    private float m_LeftBoundary = 0f;
    private float m_RightBoundary = 0f;
    private float m_UpBoundary = 0f; 
    private float m_DownBoundary = 0f;
    private float m_WrapOffset = 0f;
    protected bool m_bIncapacitate = false;

    protected bool bFriend = false;

#if UNITY_EDITOR
    private bool canDash = true;
    private bool isDashing = false;
    private float dashingPower = 12f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 0.5f;
    private TrailRenderer m_TrailRenderer = null;
#endif

    protected virtual void Awake()
    {
        if (m_Rigidbody == null)
        {
            m_Rigidbody = gameObject.AddComponent<Rigidbody>();
            if (m_Rigidbody)
            {
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
            }
        }

        if (m_Player == null)
        {
            m_Player = GetComponent<Player_Adventure>();
            m_PlayerCollider = GetComponent<Collider>();
        }

        m_LookPosition = Vector2.zero;

        // Not play game
        if (m_Player == null)
            gameObject.SetActive(false);

        m_LeftBoundary = -18f;
        m_RightBoundary = 18f;
        m_UpBoundary = 11f;
        m_DownBoundary = -10.5f;
        m_WrapOffset = 0f;

        Physics.gravity = new Vector2(0f, m_GravityPower);

        MapBase pMapBase = MapManager.Instance.GetCurrentMapInfo();
        if (pMapBase is Map_Adventure)
        {
            m_MapAdventure = pMapBase as Map_Adventure;
        }
    }

    protected virtual void Update()
    {
        if (m_eLayerType_TriggerEnter != ADVLayerType.ADVLayerType_None)
        {
            FloorCollisionHandling(false);
            ForcedWeightingApplyPosition(m_eLayerType_TriggerEnter);
            m_eLayerType_TriggerEnter = ADVLayerType.ADVLayerType_None;
        }

        if (m_eLayerType_CollisionEnter != ADVLayerType.ADVLayerType_None)
        {
            m_CurrentLayerFloor = m_eLayerType_CollisionEnter;
            m_Player.SetFirstLayerFloor(m_CurrentLayerFloor);
            m_eLayerType_CollisionEnter = ADVLayerType.ADVLayerType_None;
        }

        if (m_bIncapacitate || GameManager.Instance.GetPlayer().isStopAction)
            return;

        m_LookPosition = Vector2.zero;
        m_AnimationState = LAnimationState.Idle;

        m_LookPosition.x = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump") && m_LadderState < 2)
        {
            if (!m_IsJump)
            {
                m_IsJump = true;
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    m_Rigidbody.velocity = new Vector3(0f, 1f);
                    CallJumpOffFloor();
                }
                else
                {
                    m_Rigidbody.velocity = new Vector3(0f, (m_JumpSpeed * m_BuffSpeed));
                }
            }
        }

#if UNITY_EDITOR
        if (canDash && Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(Dash());
        }
#endif

        // 사다리
        if (m_LadderState > 0 && !m_IsJumpOff)
        {
            m_LookPosition.y = Input.GetAxisRaw("Vertical");
            if (m_LookPosition.y != 0f)
            {
                // 정규화를 하려고했으나, 잔버그가 많아서 하드코딩으로 대체
                float fMaxHeight = 99, fMinHeight = -99;
                switch (m_CurrentLadderType)
                {
                    case LadderType.FLOOR_1:
                        fMaxHeight = -4f;
                        fMinHeight = -9f;
                        break;
                    case LadderType.FLOOR_2_LEFT:
                    case LadderType.FLOOR_2_RIGHT:
                        fMaxHeight = 1f;
                        fMinHeight = -4f;
                        break;
                    case LadderType.FLOOR_3_LEFT_1:
                    case LadderType.FLOOR_3_LEFT_2:
                    case LadderType.FLOOR_3_RIGHT_1:
                    case LadderType.FLOOR_3_RIGHT_2:
                        fMaxHeight = 6f;
                        fMinHeight = 1f;
                        break;
                }

                bool bTop = false, bBottom = false;
                if (transform.position.y >= fMaxHeight)
                    bTop = true;
                else if (transform.position.y <= fMinHeight)
                    bBottom = true;

                ////////////////////////////////////////////////////////////////////////////////////
                // 사다리 끝
                if ((m_LookPosition.y > 0 && bTop) || (m_LookPosition.y < 0 && bBottom))
                {
                    FloorCollisionHandling(false); 
                    ChangeGravity(true);
                    m_LadderState = 1;
                    m_LookPosition.y = 0f;
                }
                // 사다리 입장
                else if ((m_LookPosition.y < 0) && bTop || (m_LookPosition.y > 0 && bBottom))
                {
                    ChangeGravity(false);
                    FloorCollisionHandling(true);
                }
                ////////////////////////////////////////////////////////////////////////////////////
                // 사다리타는중 - 떨어지는거 수정해봄 if -> else if
                else if (m_LadderState != 2 && (bTop | bBottom) == false)
                {
                    ChangeGravity(false);
                    FloorCollisionHandling(true);
                    m_LadderState = 2;
                    if (m_LadderTransform)
                    {
                        transform.position = new Vector3(m_LadderTransform.position.x, transform.position.y, transform.position.z); // 사다리 위치로 강제로 맞추고
                        m_Rigidbody.velocity = Vector3.zero; // 현재 속도도 없애주자
                    }
                }
            }

            if (m_LadderState == 2)
            {
                m_LookPosition.x = 0f;
                m_AnimationState = LAnimationState.Climbing;
                m_IsJump = false;

                if (m_LookPosition.y != 0f)
                    m_Player.StopAnimation(false);
                else
                    m_Player.StopAnimation(true);
            }
        }
        else
            m_Player.StopAnimation(false);

        if (m_LookPosition.x != 0f)
            m_AnimationState = LAnimationState.Running;

        if (m_IsJump)
            m_AnimationState = LAnimationState.Jumping;

        m_Player.Turn(m_LookPosition.x);
        transform.position += new Vector3(m_LookPosition.x, m_LookPosition.y, 0f) * (m_Speed * m_BuffSpeed) * Time.deltaTime;

        if (m_AnimationState != LAnimationState.Idle)
            m_Player.SetAnimationState(m_AnimationState);

        m_IsFalling = m_Rigidbody.velocity.normalized.y < 0 ? true : false;

        if (transform.position.x > m_RightBoundary)
        {
            transform.position = new Vector3(m_LeftBoundary + m_WrapOffset, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < m_LeftBoundary)
        {
            transform.position = new Vector3(m_RightBoundary - m_WrapOffset, transform.position.y, transform.position.z);
        }

        if (transform.position.y < m_DownBoundary)
        {
            transform.position = new Vector3(transform.position.x, m_UpBoundary + m_WrapOffset, transform.position.z);
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            ADVLayerType elayerType = (ADVLayerType)(collision.gameObject.layer);
            if (elayerType > ADVLayerType.ADVLayerType_None && elayerType < ADVLayerType.ADVLayerType_Max)
            {
                if (m_CurrentLayerFloor != elayerType)
                {
                    m_eLayerType_CollisionEnter = elayerType;
                }

                // 계단에서 내려온거라면
                if (m_IsUpStair)
                {
                    m_MapAdventure.IgnoreCollisionPlayer_Stair(m_CurrentStairType, m_PlayerCollider, true);
                }

                m_IsJump = false;
                m_IsJumpOff = false;

                m_IsUpStair = false;
                m_CurrentStairType = StairType.NONE;
            }
        }
        else if (collision.gameObject.CompareTag("Boss"))
        {

        }
    }

    protected virtual void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            ADVLayerType elayerType = (ADVLayerType)(collision.gameObject.layer);
            if (elayerType > ADVLayerType.ADVLayerType_None && elayerType < ADVLayerType.ADVLayerType_Max)
            {
                if (m_CurrentLayerFloor != elayerType)
                {
                    if (m_IsJumpOff)
                    {
                        m_eLayerType_TriggerEnter = elayerType;
                    }
                }
            }
        }
        else if (collision.gameObject.CompareTag("Ladders"))
        {
            m_LadderState = 1;
            m_LadderTransform = collision.transform;

            // 0은 항상 11부터 시작
            m_CurrentLadderType = (LadderType)(collision.gameObject.layer - 11);
        }
    }

    protected virtual void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Ladders"))
        {
            m_LadderState = 0;
            ChangeGravity(true);

            m_LadderTransform = null;
        }
    }

    protected void ChangeGravity(bool bGravity)
    {
        if (m_Rigidbody == null)
            return;

        if (m_Rigidbody.useGravity != bGravity)
        {
            m_Rigidbody.useGravity = bGravity;
        }
    }

    protected void FloorCollisionHandling(ADVLayerType elayerType)
    {
        m_MapAdventure.IgnoreCollisionPlayer_Floor(elayerType, m_PlayerCollider, false);
        m_CurrentLayerFloor = elayerType;
        m_Player.SetFirstLayerFloor(m_CurrentLayerFloor);
        m_MapAdventure.IgnoreCollisionPlayer_Floor(GetUpFloor(), m_PlayerCollider, true);
    }

    protected void FloorCollisionHandling(bool bTrigger)
    {
        if (m_PlayerCollider == null)
            return;

        if (m_PlayerCollider.isTrigger != bTrigger)
        {
            m_PlayerCollider.isTrigger = bTrigger;
        }
    }

    protected bool GetPlayerColliderTrigger()
    {
        if (m_PlayerCollider == null)
            return false;

        return m_PlayerCollider.isTrigger;
    }

    protected void CallJumpOffFloor()
    {
        m_IsJumpOff = true;
        if (m_IsUpStair)
        {
            m_MapAdventure.IgnoreCollisionPlayer_Stair(m_CurrentStairType, m_PlayerCollider, true);
        }
        else
        {
            FloorCollisionHandling(true);
        }
    }

    protected void ForcedWeightingApplyPosition(ADVLayerType elayerType = ADVLayerType.ADVLayerType_None)
    {
        if (elayerType == ADVLayerType.ADVLayerType_None)
            elayerType = m_CurrentLayerFloor;

        float y = m_MapAdventure.GetFloorColliderPositionY(elayerType);
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    protected ADVLayerType GetUpFloor(bool bNot = false)
    {
        switch (m_CurrentLayerFloor)
        {
            case ADVLayerType.ADVLayerType_1:
                return bNot ? ADVLayerType.ADVLayerType_None : ADVLayerType.ADVLayerType_2;
            case ADVLayerType.ADVLayerType_2:
                return bNot ? ADVLayerType.ADVLayerType_1 : ADVLayerType.ADVLayerType_3;
            case ADVLayerType.ADVLayerType_3:
                return bNot ? ADVLayerType.ADVLayerType_2 : ADVLayerType.ADVLayerType_4;
            case ADVLayerType.ADVLayerType_4:
                return bNot ? ADVLayerType.ADVLayerType_3 : ADVLayerType.ADVLayerType_None;
        }

        return ADVLayerType.ADVLayerType_None;
    }

    public void PlayerIgnoreCollision(Collider targetCollider)
    {
        if (m_PlayerCollider == null || targetCollider == null)
            return;

        Physics.IgnoreCollision(targetCollider, m_PlayerCollider, true);
    }

    public void SetInfo(PlayerAdventureBasicInfo playerAdventureBasicInfo)
    {
        m_Speed = playerAdventureBasicInfo.moveSpeed;
        m_JumpSpeed = playerAdventureBasicInfo.jumpSpeed;
    }

    public void SetBuffInfo(BuffType eBuffType, bool bAdd)
    {
        switch (eBuffType)
        {
            case BuffType.INJURY:
                {
                    // 99% 감소
                    m_BuffSpeed = bAdd ? 0.1f : 1f;
                    m_IsJump = false;
                }
                break;
        }
    }

    public bool GetOnLadder()
    {
        return m_LadderState == 2;
    }

    public ADVLayerType GetCurrentLayerFloor()
    {
        return m_CurrentLayerFloor;
    }

    public void SetIncapacitate(bool bIncapacitate)
    {
        m_bIncapacitate = bIncapacitate;
    }

#if UNITY_EDITOR

    public void SetTrailRenderer(TrailRenderer tr)
    {
        m_TrailRenderer = gameObject.AddComponent<TrailRenderer>();
        m_TrailRenderer = tr;
        m_TrailRenderer.emitting = false;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        m_Rigidbody.velocity = new Vector3(transform.localScale.x * dashingPower, 0f, 0f);
        m_TrailRenderer.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        m_TrailRenderer.emitting = false;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
#endif
}
