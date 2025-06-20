using CharacterDefines;
using GameDefines;
using OptionDefines;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UIDefines;

public class BossBase : MonsterBase
{
    protected bool[] bStartProduction = { false, false, false };
    protected int iBossSkillPercent = 0;

    protected bool bInterrupt = false;
    protected bool bProductioning = true;
    protected float fDelay = 1f;
    private Vector3 dieDirection = Vector3.zero;
    private Camera mainCamera = null;
    public bool m_dropItem { get; private set; } = true;

    protected enum DieActionState
    {
        NONE,
        SCALEDOWN,          // 수축
        FALL,               // 떨어지다
        BALLOONDEFLATES,    // 풍선이동
    }
    protected DieActionState eRandomDieAction = DieActionState.NONE;

    public int prefabIndex = 0;
    private Vector3 targetPosition = Vector3.zero;

    protected override void Awake()
    {
        base.Awake();
        
        m_eClickTargetType = ClickTargetType.BOSS;
        eRandomDieAction = DieActionState.NONE;

        // Pos Set
        Vector3 newPosition = movePositions[0];
        newPosition.y += 10;
        transform.position = newPosition;
        targetPosition = movePositions[0];
        // Target UI Set

        // Event Set
        MonsterPool.Instance.OnUpdateBossPatternEvent += HandleUpdateBossPatternEvent;
        dieDirection = Vector3.zero;
    }

    protected override void OnEnable()
    {
        currentMoveIndex = -1;

        if (bStartProduction[0])
        {
            SoundManager.Instance.PlayBossSfx(BossState.DOWN);
            UI_TargetBar BossTargetHPBar = UIManager.Instance.GetUI(UIIndexType.TARGETBAR) as UI_TargetBar;
            if (BossTargetHPBar)
            {
                BossTargetHPBar.SetUp(this);
                UIManager.Instance.ShowUI(UIIndexType.TARGETBAR);
            }
        }

        if (Oracle.m_eGameType != MapType.SPAWN)
            CameraManager.Instance.bProductioning = true;
        bStartProduction[0] = true;
    }

    protected override void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.H))
        {
            bInterrupt = true;
        }
#endif

        if (bInterrupt)
        {
            DoInterrupt();
            ClearInterrupt();
        }

        base.Update();
    }

    protected override void FixedUpdate()
    {
        if (bProductioning)
        {
            if (bStartProduction[0])
            {
                transform.Rotate(Vector3.forward * 35f);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, 3f * Time.deltaTime);

                // 타겟 위치에 도달하면 스케일을 증가시킵니다.
                if (transform.position == targetPosition)
                {
                    bStartProduction[0] = false;
                    transform.rotation = Quaternion.identity;

                    int iProductionIndex = Oracle.RandomDice(0, 2);
                    if (iProductionIndex > 0)
                    {
                        StartCoroutine(IncreaseScale());
                        CameraManager.Instance.CameraShake(1f, false);
                        SoundManager.Instance.PlayBossSfx(BossState.PONG);
                    }
                    else
                    {
                        bStartProduction[1] = true;
                        targetPosition = transform.position;
                        targetPosition.y += 10;
                        SoundManager.Instance.PlayBossSfx(BossState.UP);
                    }
                }
            }
            else if (bStartProduction[1])
            {
                transform.Rotate(Vector3.forward * 35f);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, 7f * Time.deltaTime);
                if (transform.position == targetPosition)
                {
                    bStartProduction[1] = false;
                    bStartProduction[2] = true;
                    transform.rotation = Quaternion.identity;
                    transform.localScale *= 5f;
                    targetPosition = movePositions[0];
                }
            }
            else if (bStartProduction[2])
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, 10f * Time.deltaTime);

                if (transform.position == targetPosition)
                {
                    bStartProduction[2] = false;
                    CameraManager.Instance.CameraShake(0.5f, false);
                    StartProductionEnd();
                    SoundManager.Instance.PlayBossSfx(BossState.PONG);
                }
            }
        }

        if (eRandomDieAction != DieActionState.NONE)
        {
            switch (eRandomDieAction)
            {
                case DieActionState.SCALEDOWN:
                    transform.Rotate(Vector3.forward * 20f);
                    transform.localScale *= 0.9f;
                    break;
                case DieActionState.BALLOONDEFLATES:
                    Vector3 newPosition = dieDirection * 50f * Time.deltaTime;
                    transform.position += newPosition;

                    Vector3 objectViewportPos = mainCamera.WorldToViewportPoint(transform.position);
                    bool isWithinBounds = objectViewportPos.x > 0f && objectViewportPos.x < 1f && objectViewportPos.y > 0f && objectViewportPos.y < 1f;
                    if (isWithinBounds == false)
                    {
                        dieDirection = -transform.position.normalized;
                    }
                    break;
            }
        }

        base.FixedUpdate();
    }

    Vector2 CalculateReflection(Vector2 incomingDirection)
    {
        // incomingDirection을 기준으로 반사 각을 계산합니다.
        float angle = Vector2.Angle(Vector2.up, incomingDirection);

        // 반사 각을 기준으로 reflectAngle 만큼 회전시킵니다.
        angle += 45f;
        Vector2 finalDirection = Quaternion.Euler(0, 0, angle) * Vector2.up;

        return finalDirection;
    }

    IEnumerator IncreaseScale()
    {
        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = initialScale * 5f;
        float duration = 1.0f; // 스케일 증가 지속 시간

        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        StartProductionEnd();
    }

    public void StartProductionEnd()
    {
        bEnabled = true;
        bProductioning = false;
    }

    public override void ReduceHP(int Damage, HitParticleType eHitParticleType = HitParticleType.NONE)
    {
        base.ReduceHP(Damage, eHitParticleType);

        //int iPercent = Oracle.RandomDice(0, 100);
        //if (iPercent < iBossSkillPercent)
        if (Oracle.PercentSuccess(iBossSkillPercent))
        {
            bInterrupt = true;
        }
    }

    protected override void OnDie(bool beKilled)
    {
        if (m_Animator)
            m_Animator.SetTrigger("Die");

        base.OnDie(beKilled);
        RemoveBoss();
    }

    protected IEnumerator CallRemoveBossObject()
    {
        yield return new WaitForSeconds(fDelay);

        Rigidbody Rb = GetComponent<Rigidbody>();
        if (Rb)
        {
            Destroy(Rb);
        }

        ClearInterrupt();
        eRandomDieAction = DieActionState.NONE;
        gameObject.SetActive(false);

        MonsterPool.Instance.RemoveBoss(poolIndex);
        yield break;
    }

    protected virtual void RemoveBoss()
    {
        int iRandomDieAction = Oracle.RandomDice(0, 3);
        if (iRandomDieAction == 0)
        {
            // 돌면서 스케일값 줄어들면서 사라짐
            eRandomDieAction = DieActionState.SCALEDOWN;
            fDelay = 1f;
        }
        else if (iRandomDieAction == 1)
        {
            eRandomDieAction = DieActionState.FALL;
            fDelay = 3f;

            Rigidbody Rb = this.AddComponent<Rigidbody>();
            if (Rb)
            {
                Vector3 Direction = new Vector3(-1f, 1f, 0f).normalized;
                Rb.AddForce(Direction * 500f);
            }
        }
        else
        {
            eRandomDieAction = DieActionState.BALLOONDEFLATES;
            fDelay = 1f;

            mainCamera = Camera.main;
            dieDirection = LookVector.normalized;
        }

        BossState eBossState = (BossState)((int)(BossState.DIE0) + iRandomDieAction);
        SoundManager.Instance.PlayBossSfx(eBossState);

        if (m_dropItem)
            DropItem();

        StartCoroutine(CallRemoveBossObject());
    }

    protected virtual void DoInterrupt() { }

    protected virtual void ClearInterrupt()
    {
        bInterrupt = false;
    }

    protected virtual void HandleUpdateBossPatternEvent()
    {
        ClearInterrupt();
    }

    // 연출용
    public Vector2 GetMovePosition(int index)
    {
        return movePositions[index];
    }

    public void SetSpawnMapProduction(bool bDropItem)
    {
        SoundManager.Instance.PlayBossSfx(BossState.DOWN);
        UI_TargetBar BossTargetHPBar = UIManager.Instance.GetUI(UIIndexType.TARGETBAR) as UI_TargetBar;
        if (BossTargetHPBar)
        {
            BossTargetHPBar.SetUp(this);
            UIManager.Instance.ShowUI(UIIndexType.TARGETBAR);
        }

        m_dropItem = bDropItem; 
    }

    public void SetDropItem(bool bDropItem)
    {
        m_dropItem = bDropItem;
    }
}
