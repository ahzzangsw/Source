using CharacterDefines;
using GameDefines;
using OptionDefines;
using UnityEngine;

public class MonsterAdventure : MonsterBase
{
    public float Range { get; protected set; } = 0f;
    public int Damage { get; protected set; } = 0;

    protected override void Awake()
    {
        m_eClickTargetType = ClickTargetType.MONSTER;

        EquipWeapon equipWeapon = GetComponent<EquipWeapon>();
        if (equipWeapon != null)
            m_AttackAnimState = equipWeapon.AttackAnim;
        else
            m_AttackAnimState = "Jab";
    }

    // 어드벤쳐모드는 MonsterPathfinder에서 처리된다.
    protected override void FixedUpdate()
    {
        if (!bEnabled)
            return;

        if (currentHP <= 0)
        {
            OnDie(true);
            return;
        }
    }

    public virtual void SetInfo_Adv(int index, int hp, int defense, float movespeed, float range, int damage, Vector3 pos)
    {
        ID = index;
        Hp = hp;
        currentHP = hp;
        Defense = defense;
        moveSpeed = movespeed;
        Range = range;
        Damage = damage;

        transform.position = pos;

        m_BuffContainer = new BuffContainer(this);
        bOnBuff = true;

        SetBasicInfo();
        // 몬스터 플레이어 몸 충돌 체크 해제
        GameManager.Instance.GetPlayer().PlayerIgnoreCollision(GetComponent<Collider>());
    }

    public void HitPrey()
    {
        //Player MyPlayer = GameManager.Instance.GetPlayer();
        //if (MyPlayer == null)
        //    return;

        //SetAnimationActionState(LAnimationState.Attack);
        //MyPlayer.ReduceHP(Damage);

        if (m_eClickTargetType == ClickTargetType.FINAL)
            SoundManager.Instance.PlayBossSfx(BossState.HIT);

        SoundManager.Instance.PlayCharacterSfx_SaveIndex(CharacterState.OINK);

        VersatilityDie(false);
    }
}
