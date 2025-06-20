using GameDefines;
using System;
using System.Collections;
using UnityEngine;
using CharacterDefines;
using OptionDefines;

public enum BuildingPosState { Side, Back, Front, }

public class Building : Character
{
    public Vector3 m_SpawnPosition { get; protected set; }
    public int m_ID { get; protected set; } = -1;
    public int Cost { get; protected set; } = 0;
    public byte TargetCount { get; protected set; } = 1;
    public int Damage { get; set; } = 0;
    public float Range { get; set; } = 0f;
    public float AttackSpeed { get; set; } = 0f;
    public int Luck { get; protected set; } = 0;
    public WeaponType eWeaponType { get; protected set; } = WeaponType.NONE;

    protected Projectile ProjectileClass = null;

    public int SpawnIndex = -1;
    private int CurrentUpgradeGrade = 0;

    protected virtual void Update()
    {
        m_BuffContainer.Tick();
    }
    public void SetInfo(Vector3 position, int id, SpeciesType eSpeciesType, int BuildIndex)
    {
        m_eClickTargetType = ClickTargetType.BUILDING;
        EquipWeapon equipWeapon = GetComponent<EquipWeapon>();
        eWeaponType = equipWeapon.eWeaponType;
        m_AttackAnimState = equipWeapon.AttackAnim;
        ProjectileClass = GetComponent<Projectile>();

        m_SpawnPosition = position;
        m_ID = id;
        m_eSpeciesType = eSpeciesType;
        m_CharacterIndex = BuildIndex;

        transform.SetSiblingIndex(1);

        BuildingInfo buildingInfo = ResourceAgent.Instance.GetBuildingInfo(m_eSpeciesType, m_CharacterIndex);
        if (buildingInfo.Damage != 0)
        {
            Damage = buildingInfo.Damage;
            Range = buildingInfo.Range;
            AttackSpeed = buildingInfo.AttackSpeed;
            Cost = buildingInfo.Cost;
            eBuffType = buildingInfo.eBuffType;

            SetAddStat();

            if (ProjectileClass == null)
            {
                Debug.Log("ProjectileClass is null = " + name);
                return;
            }

            TargetCount = buildingInfo.TargetCount > 0 ? buildingInfo.TargetCount : (byte)1;
            TargetCount = buildingInfo.TargetCount > 5 ? (byte)5 : buildingInfo.TargetCount;

            ProjectileClass.SetMaster(this);
            m_BuffContainer = new BuffContainer(this);

            SoundManager.Instance.PlayCharacterSfx(eSpeciesType, CharacterState.SPAWN_BASE); 
            SoundManager.Instance.PlayCharacterSfx(eSpeciesType, CharacterState.SPAWN);
        }
        else
        {
            Debug.Log("buildingInfo Error SpeciesType = " + Enum.GetName(typeof(SpeciesType), m_eSpeciesType) + ", index = " + m_CharacterIndex);
        }
    }

    private void SetAddStat()
    {
        CursorInfo AddInfo = UnlockManager.Instance.GetCursorInfoList(OptionManager.Instance.iCursorIndex);
        if (AddInfo.tooltip.Length == 0)
            return;

        if (AddInfo.Damage != 0f)
        {
            if (AddInfo.Damage < 1f)
            {
                float addDamage = (float)Damage * AddInfo.Damage;
                Damage += (int)addDamage;
            }
            else
            {
                Damage += (int)(AddInfo.Damage);
            }
        }

        if (AddInfo.Range != 0f)
        {
            Range += AddInfo.Range;
        }

        if (AddInfo.AttackSpeed != 0f)
        {
            AttackSpeed += AddInfo.AttackSpeed;
        }
    }

    public bool HitMonster(MonsterBase hitMonster, WeaponBase weapon, HitParticleType eHitParticleType)
    {
        if (HitMonster(hitMonster, eHitParticleType) == false)
            return false;

        if (weapon != null)
        {
            if (ProjectileClass.eProjectileType == ProjectileType.BOUNCES)
            {
                Bounces bounces = ProjectileClass as Bounces;
                if (bounces)
                {
                    bounces.NextFire(hitMonster, this, weapon);
                }
            }
        }
        return true;
    }

    public bool HitMonster(MonsterBase hitMonster, HitParticleType eHitParticleType)
    {
        if (hitMonster == null)
            return false;

        if (hitMonster.IsDie())
            return false;

        int CalculationDamage = CalculationDamageFormula.CalculationDamage(this, hitMonster);
        hitMonster.ReduceHP(CalculationDamage, eHitParticleType);
        return true;
    }

    public virtual void SetIncapacitate(bool bIncapacitate)
    {
        if (bIncapacitate)
        {
            //m_bStopAnim = true;
            m_Animator.SetBool("Action", true);
            SetAnimationState(LAnimationState.Dead);
        }
        else
        {
            m_Animator.SetBool("Action", false);
            //m_bStopAnim = false;
        }

        ProjectileClass.SetIncapacitate(bIncapacitate);
    }

    public virtual void SetVolatilityStat(BuffType eBuffType, bool bAdd)
    {

    }

    public override void AddBuffActor(BuffType eBuffType)
    {
        if (eBuffType == BuffType.NONE2 || m_BuffContainer == null)
            return;

        m_BuffContainer.AddBuff(eBuffType, 0);
    }

    public override void AddDeBuffActor(Character giveDebuffObject)
    {

    }

    public void WeaponFlashSound(bool bHit, bool bMelee = false)
    {
        SFXType eSFXType = SFXType.SFX_NONE;
        if (bMelee)
        {
            if (m_AttackAnimState == "Jab")
                eSFXType = SFXType.SFX_JAB;
            else
                eSFXType = SFXType.SFX_SLASH;
        }
        else
        {
            eSFXType = Oracle.GetSFXSoundType(eWeaponType, bHit);
        }

        if (eSFXType != SFXType.SFX_NONE)
            SoundManager.Instance.PlaySfx(eSFXType);
    }

    public void WeaponFlashSound(WeaponType eWeaponType, bool bHit)
    {
        SFXType eSFXType = Oracle.GetSFXSoundType(eWeaponType, bHit);
        if (eSFXType != SFXType.SFX_NONE)
            SoundManager.Instance.PlaySfx(eSFXType);
    }

    public void Die()
    {
        ProjectileClass.DieProjectile();

        //m_bStopAnim = true;
        SetAnimationState(LAnimationState.Dead);

        StartCoroutine(CallDie());
    }

    IEnumerator CallDie()
    {
        float PosY = -20f;
        while (transform.position.y > PosY)
        {
            float newY = Mathf.MoveTowards(transform.position.y, PosY, Time.deltaTime * 5f);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        BuildingPool.Instance.RemoveBuilding(this, false);
    }

    public void SetUpgradeGrade(int iGrade)
    {
        if (CurrentUpgradeGrade == iGrade || CurrentUpgradeGrade > iGrade)
            return;

        CurrentUpgradeGrade = iGrade;
        Damage *= 2;
    }
}
