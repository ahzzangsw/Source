using CharacterDefines;
using GameDefines;
using OptionDefines;
using System.Collections;
using UIDefines;
using UnityEngine;

public class BossAdventure_Last_Skill : Character
{
    [SerializeField] public AdventurePrefabsType m_eAdventurePrefabsType = AdventurePrefabsType.MAX;
    [SerializeField] public int m_Damage = 0;
    
    private Gimmick m_Gimmick = null;

    public override void SetComponent(Character tempCharacter)
    {
        base.SetComponent(tempCharacter);
        m_Gimmick = GetComponent<Gimmick>();
    }

    public void SetTarget(Player_Adventure target, int iLevel, AdventurePrefabsType eAdventurePrefabsType)
    {
        if (m_Gimmick == null)
            return;

        m_Gimmick.SetTarget(target, iLevel, eAdventurePrefabsType);
    }

    public void WeaponFlashSound(WeaponType eWeaponType, bool bHit)
    {

    }

    // Animation
    public void Ready()
    {
        if (GetAnimationState() == LAnimationState.Running)
        {
            //EffectManager.Instance.CreateSpriteEffect(this, "Brake");
        }
        else if (GetAnimationState() == LAnimationState.Idle)
        {
            return;
        }

        SetAnimationState(LAnimationState.Ready);
    }

    public void Running()
    {
        //EffectManager.Instance.CreateSpriteEffect(this, "Jump");
        SetAnimationState(LAnimationState.Running);
    }

    public void Land()
    {
        //EffectManager.Instance.CreateSpriteEffect(this, "Fall");
    }

    public void Attack()
    {
        m_Animator.SetTrigger("Attack");
    }

    public void Fire()
    {
        m_Animator.SetTrigger("Fire");
    }

    public void Hit()
    {
        m_Animator.SetTrigger("Hit");
    }
}
