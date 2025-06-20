using GameDefines;
using OptionDefines;
using System;
using UnityEngine;

public class Player_Adventure : Building
{
    protected AdventureMoveController AdventureController = null; // 너도 가지고 있어라
    protected PlayerAdventureBasicInfo DefaultBasicInfo;
    protected bool isIncapacitate = false;

    protected override void Update()
    {
        // 기본공격
        if (Input.GetButton("Fire1"))
        {
            Attack();
        }

        base.Update();
    }

    public void SetInfo(SpeciesType eSpeciesType, int BuildIndex, PlayerAdventureBasicInfo playerAdventureBasicInfo)
    {
        m_eClickTargetType = ClickTargetType.PLAYER;

        EquipWeapon equipWeapon = GetComponent<EquipWeapon>();
        eWeaponType = equipWeapon.eWeaponType;
        m_AttackAnimState = equipWeapon.AttackAnim;
        ProjectileClass = GetComponent<Projectile>();

        m_ID = 0;
        m_eSpeciesType = eSpeciesType;
        m_CharacterIndex = BuildIndex;

        //transform.SetSiblingIndex(1);

        BuildingInfo buildingInfo = ResourceAgent.Instance.GetBuildingInfo(m_eSpeciesType, m_CharacterIndex);
        if (buildingInfo.Damage != 0)
        {
            Damage = buildingInfo.Damage;
            Range = buildingInfo.Range;
            AttackSpeed = buildingInfo.AttackSpeed;
            Cost = buildingInfo.Cost;
            eBuffType = buildingInfo.eBuffType;

            if (ProjectileClass == null)
            {
                Debug.Log("ProjectileClass is null = " + name);
                return;
            }

            TargetCount = buildingInfo.TargetCount > 0 ? buildingInfo.TargetCount : (byte)1;
            TargetCount = buildingInfo.TargetCount > 5 ? (byte)5 : buildingInfo.TargetCount;

            DefaultBasicInfo = new PlayerAdventureBasicInfo(Damage, Range, 0f, 0f, AttackSpeed, TargetCount);
            SetAddStat(playerAdventureBasicInfo);

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

    protected virtual void Attack()
    {
        if (GameManager.Instance.GetPlayer().isStopAction)
            return;

        if (ProjectileClass == null)
            return;

        // 사다리일때는 공격불가
        if (AdventureController.GetOnLadder())
            return;

        // 무력화일때 공격불가
        if (isIncapacitate)
            return;

        ProjectileClass.AdventureAttack();
    }

    public void SetAddStat(PlayerAdventureBasicInfo playerAdventureBasicInfo)
    {
        float AddD = (float)(playerAdventureBasicInfo.AddDamagePer) / 100f * DefaultBasicInfo.AddDamagePer;
        Damage = DefaultBasicInfo.AddDamagePer + (int)AddD;

        float AddR = (float)(playerAdventureBasicInfo.AddRangePer) / 100f * DefaultBasicInfo.AddRangePer;
        Range = DefaultBasicInfo.AddRangePer + AddR;

        float AddA = (float)(playerAdventureBasicInfo.AddatkSpeedPer) / 100f * DefaultBasicInfo.AddatkSpeedPer;
        AttackSpeed = DefaultBasicInfo.AddatkSpeedPer + AddA;
    }

    public override void SetIncapacitate(bool bIncapacitate)
    {
        base.SetIncapacitate(bIncapacitate);
        isIncapacitate = bIncapacitate;

        if (AdventureController)
        {
            AdventureController.SetIncapacitate(bIncapacitate);
        }
    }

    public override void SetVolatilityStat(BuffType eBuffType, bool bAdd)
    {
        if (AdventureController == null)
            return;

        AdventureController.SetBuffInfo(eBuffType, bAdd);
    }

    public void SetAdventureController()
    {
        if (AdventureController == null)
        {
            AdventureController = GetComponent<AdventureMoveController>();
        }
    }

    public AdventureMoveController GetAdventureController()
    {
        return AdventureController;
    }

    public void Ondie()
    {
        if (AdventureController == null)
            return;

        Destroy(AdventureController);
        AdventureController = null;
    }
}
