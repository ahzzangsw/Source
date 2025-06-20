using GameDefines;
using CharacterDefines;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIDefines;

public class Character : Identity
{
    [SerializeField] public SpriteRenderer m_Body;
    [SerializeField] public Animator m_Animator;
    [SerializeField] protected ParticleSystem m_HitDust;
    public ClickTargetType m_eClickTargetType { get; protected set; } = ClickTargetType.NONE;
    public BuffType eBuffType { get; protected set; } = BuffType.NONE;
    public int m_CharacterIndex { get; protected set; } = -1;
    public BuffContainer m_BuffContainer { get; protected set; } = null;

    protected Dictionary<BuffType, GameObject> buffPrefabList;
    protected static Material DefaultMaterial;
    protected static Material BlinkMaterial;
    protected string m_AttackAnimState = "";
    protected float _activityTime;
    protected bool m_bStopAnim = false;
    protected float m_fAnimationSpeed = 0f;

    private bool m_bAnimationForceStop = false;
    protected ADVLayerType m_CurrentLayerFloor = ADVLayerType.ADVLayerType_None;

    public virtual void SetComponent(Character tempCharacter)
    {
        if (tempCharacter == null)
        {
            Debug.Log("SetComponent is character null" + name);
            return;
        }

        m_Body = tempCharacter.m_Body;
        m_Animator = tempCharacter.m_Animator;
        m_HitDust = tempCharacter.m_HitDust;

        if (buffPrefabList == null)
            buffPrefabList = new Dictionary<BuffType, GameObject>();

        if (m_Animator != null)
            m_fAnimationSpeed = m_Animator.speed;
    }

    protected virtual void FixedUpdate()
    {
        var targetState = Time.time - _activityTime > 1f ? LAnimationState.Idle : LAnimationState.Ready;
        if (GetAnimationState() != targetState)
        {
            SetAnimationState(targetState);
        }

        if (m_Animator.GetBool("Action") || m_bStopAnim)
        {
            _activityTime = Time.time;
        }
    }

    public void Blink()
    {
        if (DefaultMaterial == null) DefaultMaterial = m_Body.sharedMaterial;
        if (BlinkMaterial == null) BlinkMaterial = new Material(Shader.Find("GUI/Text Shader"));

        StartCoroutine(BlinkCoroutine());
    }

    private IEnumerator BlinkCoroutine()
    {
        m_Body.material = BlinkMaterial;
        yield return new WaitForSeconds(0.1f);
        m_Body.material = DefaultMaterial;
    }

    public void ChangeRanderOrder(int orderIndex)
    {
        if (m_Body)
        {
            m_Body.sortingOrder = orderIndex;
        }

        if (m_HitDust)
        {
        }
    }

    public void SetAnimationState(LAnimationState state)
    {
        if (m_eClickTargetType == ClickTargetType.BOSS)
            return;

        if (m_Animator == null)
        {
            Debug.Log("Character Animator is null = " + name);
            return;
        }

        if (GetAnimationState() == state)
            return;

        foreach (var variable in new[] { "Idle", "Ready", "Walking", "Running", "Crawling", "Jumping", "Climbing", "Blocking", "Dead" })
        {
            m_Animator.SetBool(variable, false);
        }

        switch (state)
        {
            case LAnimationState.Idle: m_Animator.SetBool("Idle", true); break;
            case LAnimationState.Ready: m_Animator.SetBool("Ready", true); break;
            case LAnimationState.Walking: m_Animator.SetBool("Walking", true); break;
            case LAnimationState.Running: m_Animator.SetBool("Running", true); break;
            case LAnimationState.Crawling: m_Animator.SetBool("Crawling", true); break;
            case LAnimationState.Jumping: m_Animator.SetBool("Jumping", true); break;
            case LAnimationState.Climbing: m_Animator.SetBool("Climbing", true); break;
            case LAnimationState.Blocking: m_Animator.SetBool("Blocking", true); break;
            case LAnimationState.Dead: m_Animator.SetBool("Dead", true); break;
            default: throw new NotSupportedException();
        }
    }

    public void SetAnimationActionState(LAnimationState state, bool reset = false)
    {
        if (m_eClickTargetType == ClickTargetType.BOSS || m_AttackAnimState.Length == 0)
            return;

        switch (state)
        {
            case LAnimationState.Attack:
                {
                    if (reset)
                        m_Animator.ResetTrigger(m_AttackAnimState);
                    else
                        m_Animator.SetTrigger(m_AttackAnimState);
                }
                break;
        }
    }

    public LAnimationState GetAnimationState()
    {
        if (m_eClickTargetType == ClickTargetType.BOSS)
            return LAnimationState.Ready;

        if (m_Animator.GetBool("Idle")) return LAnimationState.Idle;
        if (m_Animator.GetBool("Ready")) return LAnimationState.Ready;
        if (m_Animator.GetBool("Walking")) return LAnimationState.Walking;
        if (m_Animator.GetBool("Running")) return LAnimationState.Running;
        if (m_Animator.GetBool("Crawling")) return LAnimationState.Crawling;
        if (m_Animator.GetBool("Jumping")) return LAnimationState.Jumping;
        if (m_Animator.GetBool("Climbing")) return LAnimationState.Climbing;
        if (m_Animator.GetBool("Blocking")) return LAnimationState.Blocking;
        if (m_Animator.GetBool("Dead")) return LAnimationState.Dead;

        return LAnimationState.Ready;
    }

    public void StopAnimation(bool bStop)
    {
        if (m_bAnimationForceStop == bStop)
            return;

        m_bAnimationForceStop = bStop;
        if (bStop)
            m_Animator.speed = 0f;
        else
            m_Animator.speed = m_fAnimationSpeed;
    }

    public void Turn(float direction)
    {
        if (direction == 0f)
            return;

        //var scale = transform.localScale;
        //scale.x = Mathf.Sign(direction) * Mathf.Abs(scale.x);
        //transform.localScale = scale;

        var rotation = transform.localRotation;
        rotation.y = direction < 0f ? 180 : rotation.x;
        transform.localRotation = rotation;
    }
    public void UpAndDown(float direction)
    {
        if (direction > 0)
            SetAnimationState(LAnimationState.Climbing);
        else
            SetAnimationState(LAnimationState.Jumping);
    }

    public virtual void PlayHitParticle(bool bPlay, BuffType eBuffType)
    {
        switch (eBuffType)
        {
            case BuffType.POISON2:
                break;
            default:
                eBuffType = Oracle.GetBuffCategory(eBuffType);
                break;
        };

        if (eBuffType == BuffType.NONE)
            return;

        if (bPlay)
        {
            if (buffPrefabList.ContainsKey(eBuffType) == false)
            {
                GameObject BuffObject = ResourceAgent.Instance.GetBuffPrefab(eBuffType);
                if (BuffObject == null)
                    return;

                GameObject BuffClone = Instantiate(BuffObject, transform);
                if (BuffClone == null)
                    return;

                buffPrefabList.Add(eBuffType, BuffClone);
            }
        }
        else
        {
            if (buffPrefabList.ContainsKey(eBuffType))
            {
                Destroy(buffPrefabList[eBuffType]);
                buffPrefabList[eBuffType] = null;
                buffPrefabList.Remove(eBuffType);
            }
        }

        UIManager.Instance.ShowCharacterUI(UIIndexType.MAININFO, this, true);
    }

    public virtual void AddBuffActor(BuffType eBuffType)
    {

    }

    public virtual void AddDeBuffActor(Character giveDebuffObject)
    {

    }

    public void FadeOutRollback()
    {
        if (m_Body == null)
            return;

        Color originalColor = m_Body.color;
        m_Body.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
    }

    public void FadeOut(float fAlpha)
    {
        StartCoroutine(CallFadeOutRoutineBody(fAlpha));
    }

    protected virtual IEnumerator CallFadeOutRoutineBody(float fAlpha)
    {
        if (m_Body == null)
            yield break;

        Color originalColor = m_Body.color;

        float alpha = 1f;
        while (alpha > fAlpha)
        {
            alpha -= 0.01f;
            m_Body.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return new WaitForSeconds(0.01f);
        }

        // 완전히 투명하게 만들기 위해 알파 값을 0으로 설정
        m_Body.color = new Color(originalColor.r, originalColor.g, originalColor.b, fAlpha);
    }

    public virtual Collider GetCollider()
    {
        return GetComponent<Collider>();
    }

    public void SetFirstLayerFloor(ADVLayerType eADVLayerType)
    {
        m_CurrentLayerFloor = eADVLayerType;
    }

    public virtual ADVLayerType GetCurrentLayerFloor()
    {
        if (Oracle.m_eGameType != MapType.ADVENTURE)
            return ADVLayerType.ADVLayerType_None;

        return m_CurrentLayerFloor;
    }

    public Vector3 GetPosition()
    {
        if (Oracle.m_eGameType != MapType.ADVENTURE)
            return TypeDefs.VECTOR_NONE;

        return transform.position;
    }
}
