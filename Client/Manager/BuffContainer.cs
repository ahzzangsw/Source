using GameDefines;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Timers;
using static BuffContainer;

public class BuffContainer
{
    private class BuffValue
    {
        public BuffType m_eBuffType { get; set; } = BuffType.NONE;
        public int m_Duration { get; set; } = 0;
    }

    private Dictionary<BuffType, BuffValue> buffDictionary = new Dictionary<BuffType, BuffValue>();
    private Timer timer;

    private bool m_bUpdate = false;
    private Character m_Owner = null;
    private OriginData m_OriginData = null;
    private List<BuffType> m_ExpiredBuffs = null;
    private List<BuffType> m_UpdateBuffs = null; 

    public class OriginData
    {
        // Monster
        public float moveSpeed = 0;

        public int DamagePoison = 0;
        public int DamageFlame = 0;

        // Building
        public int Damage = 0;
        public float AttackSpeed = 0f;
    }

    public BuffContainer(Character pTarget)
    {
        m_Owner = pTarget;
        m_OriginData = new OriginData();
        m_ExpiredBuffs = new List<BuffType>();
        m_UpdateBuffs = new List<BuffType>();
        if (m_Owner.m_eClickTargetType == ClickTargetType.BUILDING || m_Owner.m_eClickTargetType == ClickTargetType.PLAYER)
        {
            Building building = m_Owner as Building;
            if (building == null)
                return;

            int addDamage = 0;
            if (Oracle.m_eGameType == MapType.SPAWN)
            {
                Player MyPlayer = GameManager.Instance.GetPlayer();
                if (MyPlayer != null)
                {
                    ControlInfo controlInfoData = MyPlayer.GetCurrentUpgradeData(building.m_eSpeciesType);
                    addDamage = controlInfoData.UnitAtk[building.m_CharacterIndex];
                }
            }

            m_OriginData.Damage = building.Damage + addDamage;
            m_OriginData.AttackSpeed = building.AttackSpeed;
        }
        else
        {
            MonsterBase monster = m_Owner as MonsterBase;
            if (monster)
            {
                m_OriginData.moveSpeed = monster.moveSpeed;
            }
        }

        timer = new Timer(1000);
        timer.Elapsed += CheckBuffs;
        timer.Start();
    }
    ~BuffContainer()
    {
        timer.Stop();
        timer.Elapsed -= CheckBuffs;
        timer = null;

        m_Owner = null;
        m_OriginData = null;

        buffDictionary.Clear();
    }

    public void UpdateMonsterInfo(float moveSpeed)
    {
        m_OriginData.moveSpeed = moveSpeed;
    }

    private void CheckBuffs(object sender, ElapsedEventArgs e)
    {
        // 모든 버프를 확인하고 만료된 버프를 제거
        foreach (var data in buffDictionary)
        {
            data.Value.m_Duration--;
            if (data.Value.m_Duration <= 0)
            {
                m_ExpiredBuffs.Add(data.Key);
            }
            else
            {
                m_UpdateBuffs.Add(data.Key);
            }
        }

        m_bUpdate = true;
    }

    private bool ImmediatelyBuff(BuffType eBuffType)
    {
        switch (eBuffType)
        {
            case BuffType.CRITICAL0:
            case BuffType.CRITICAL1:
            case BuffType.CRITICAL2:
            case BuffType.KNOCKBACK0:
            case BuffType.KNOCKBACK1:
            case BuffType.KNOCKBACK2:
            case BuffType.STEAL0:
            case BuffType.STEAL1:
            case BuffType.STEAL2:
            case BuffType.STEAL3:
                return true;
        };

        return false;
    }
    public void RemoveBuff(BuffType eBuffType)
    {
        buffDictionary.Remove(eBuffType);
        
        BuffType rootBuffType = Oracle.GetBuffCategory(eBuffType);
        if (m_Owner.m_eClickTargetType == ClickTargetType.BUILDING || m_Owner.m_eClickTargetType == ClickTargetType.PLAYER)
        {
            Building building = m_Owner as Building;
            if (building == null)
                return;

        }
        else
        {
            MonsterBase monster = m_Owner as MonsterBase;
            if (monster == null)
                return;

            if (rootBuffType == BuffType.POISON0)
            {
                m_OriginData.DamagePoison = 0;
            }
            else if (rootBuffType == BuffType.BURN0)
            {
                m_OriginData.DamageFlame = 0;
            }
        }
    }
    public void AddBuff(BuffType eBuffType, int Damage)
    {
        // 중복 검사
        if (eBuffType == BuffType.INJURY)
        {
            RemoveBuff(eBuffType);
            m_ExpiredBuffs.Remove(eBuffType);
        }
        else
        {
            foreach (var Key in buffDictionary.Keys)
            {
                if (eBuffType == Key)
                    return;

                if (Oracle.GetBuffCategory(eBuffType) == Oracle.GetBuffCategory(Key))
                {
                    if (eBuffType > Key)
                    {
                        RemoveBuff(Key);
                        break;
                    }
                    else
                        return;
                }
            }
        }

        if (ImmediatelyBuff(eBuffType) == false)
        {
            BuffType rootBuffType = Oracle.GetBuffCategory(eBuffType);
            BuffType currentBuffType = BuffType.NONE;
            foreach (var key in buffDictionary.Keys)
            {
                BuffType refRootBuffType = Oracle.GetBuffCategory(key);
                if (rootBuffType == refRootBuffType)
                {
                    currentBuffType = key;
                    break;
                }
            }

            if (currentBuffType != BuffType.NONE)
                RemoveBuff(eBuffType);

            BuffValue buffValue = new BuffValue();
            buffValue.m_eBuffType = eBuffType;
            buffValue.m_Duration = 3;
            if (m_Owner.m_eClickTargetType != ClickTargetType.BUILDING && m_Owner.m_eClickTargetType != ClickTargetType.PLAYER)
            {
                MonsterBase monster = m_Owner as MonsterBase;
                if (monster == null)
                    return;

                switch (eBuffType)
                {
                    case BuffType.POISON0:
                    case BuffType.POISON1:
                        m_OriginData.DamagePoison = monster.Hp * 5 / 100;
                        break;
                    case BuffType.POISON2:
                        m_OriginData.DamagePoison = monster.Hp / 10;
                        break;
                    case BuffType.BURN0:
                        m_OriginData.DamageFlame = Damage / 10;
                        break;
                    case BuffType.BURN1:
                        m_OriginData.DamageFlame = Damage / 10 * 2;
                        break;
                    case BuffType.BURN2:
                        m_OriginData.DamageFlame = Damage / 10 * 3;
                        break;
                }

                if (eBuffType == BuffType.STUN0 || eBuffType == BuffType.STUN1 || eBuffType == BuffType.STUN2)
                {
                    if (monster.m_eClickTargetType == ClickTargetType.BOSS || monster.m_eClickTargetType == ClickTargetType.FINAL)
                        buffValue.m_Duration = 1;
                    else
                        buffValue.m_Duration = 2;
                }
                else
                {
                    if (monster.m_eClickTargetType == ClickTargetType.FINAL)
                        buffValue.m_Duration = 1;
                    else if (monster.m_eClickTargetType == ClickTargetType.BOSS)
                        buffValue.m_Duration = 2;
                    else
                        buffValue.m_Duration = 3;
                }
            }

            buffDictionary.Add(eBuffType, buffValue);
        }
        else
        {
            BuffValue buffValue = new BuffValue();
            buffValue.m_eBuffType = eBuffType;
            buffValue.m_Duration = 0;
            buffDictionary.Add(eBuffType, buffValue);
        }

        UpdateBuff(eBuffType, true, 0);
    }

    public void UpdateBuff(BuffType eBuffType, bool bAdd, int iProcessIndex)
    {
        if (buffDictionary.ContainsKey(eBuffType))
        {
            bool bRemove = false;
            if (m_Owner.m_eClickTargetType == ClickTargetType.BUILDING || m_Owner.m_eClickTargetType == ClickTargetType.PLAYER)
            {
                Building building = m_Owner as Building;
                if (building == null)
                    return;

                int iAttackUpValue = 0;
                float fSpeedUpValue = 0;
                switch (eBuffType)
                {
                    case BuffType.ATTACK_UP0:
                        iAttackUpValue += 5;
                        break;
                    case BuffType.ATTACK_UP1:
                        iAttackUpValue += 10;
                        break;
                    case BuffType.ATTACK_UP2:
                        iAttackUpValue += 12;
                        break;
                    case BuffType.ATTACK_UP3:
                        iAttackUpValue += 15;
                        break;
                    case BuffType.ATTACK_UP4:
                        iAttackUpValue += 20;
                        break;
                    case BuffType.ATTACKSPEED_UP0:
                        fSpeedUpValue += 10f;
                        break;
                    case BuffType.ATTACKSPEED_UP1:
                        fSpeedUpValue += 15f;
                        break;
                    case BuffType.ATTACKSPEED_UP2:
                        fSpeedUpValue += 20f;
                        break;
                    case BuffType.ATTACKSPEED_UP3:
                        fSpeedUpValue += 30f;
                        break;
                    case BuffType.ATTACKSPEED_UP4:
                        fSpeedUpValue += 40f;
                        break;
                    case BuffType.STEAL0:
                    case BuffType.STEAL1:
                    case BuffType.STEAL2:
                    case BuffType.STEAL3:
                        {
                            Player MyPlayer = GameManager.Instance.GetPlayer();
                            if (MyPlayer)
                            {
                                int AddCost = 1;
                                if (eBuffType == BuffType.STEAL2)
                                    AddCost = 2;
                                else if (eBuffType == BuffType.STEAL3)
                                    AddCost = 3;

                                MyPlayer.AddMoney(AddCost);
                            }
                            bRemove = true;
                        }
                        break;
                    case BuffType.INCAPACITATE:
                        if (bAdd)
                        {
                            building.SetIncapacitate(true);
                        }
                        else
                        {
                            if (iProcessIndex == 0)
                                building.SetIncapacitate(false);
                        }
                        break;
                    case BuffType.REDUCING:
                        iAttackUpValue -= 30;
                        fSpeedUpValue -= 30;
                        break;
                    case BuffType.DIE:
                        building.Die();
                        break;
                    case BuffType.INJURY:
                        building.SetVolatilityStat(BuffType.INJURY, bAdd);
                        break;
                }
                
                if (iAttackUpValue != 0)
                {
                    if (bAdd)
                    {
                        int damege = building.Damage * iAttackUpValue / 100;
                        building.Damage = m_OriginData.Damage;
                        building.Damage += damege;
                    }
                    else
                    {
                        if (iProcessIndex == 0)
                            building.Damage = m_OriginData.Damage;
                    }
                }
                if (fSpeedUpValue != 0f)
                {
                    if (bAdd)
                    {
                        float AttackSpeed = building.AttackSpeed * fSpeedUpValue / 100f;
                        building.AttackSpeed = m_OriginData.AttackSpeed;
                        building.AttackSpeed += AttackSpeed;
                    }
                    else
                    {
                        if (iProcessIndex == 0)
                            building.AttackSpeed = m_OriginData.AttackSpeed;
                    }
                }

                if (iProcessIndex == 0)
                    building.PlayHitParticle(bAdd, eBuffType);
            }
            else
            {
                MonsterBase monster = m_Owner as MonsterBase;
                if (monster == null)
                    return;

                switch (eBuffType)
                {
                    case BuffType.ARMORREDUCING0:
                    case BuffType.ARMORREDUCING1:
                    case BuffType.ARMORREDUCING2:
                        {
                            if (iProcessIndex == 0)
                                monster.PlayHitParticle(bAdd, BuffType.ARMORREDUCING0);
                        }
                        break;
                    case BuffType.SLOW0:
                    case BuffType.SLOW1:
                    case BuffType.SLOW2:
                        {
                            if (iProcessIndex == 0)
                            {
                                if (bAdd)
                                {
                                    float speed = monster.moveSpeed * 0.2f;
                                    monster.moveSpeed = m_OriginData.moveSpeed;
                                    monster.moveSpeed -= speed;
                                }
                                else
                                {
                                    monster.moveSpeed = m_OriginData.moveSpeed;
                                }

                                monster.bSlow = true;
                                monster.PlayHitParticle(bAdd, BuffType.SLOW0);
                            }
                        }
                        break;
                    case BuffType.POISON0:
                    case BuffType.POISON1:
                    case BuffType.POISON2:
                        {
                            if (iProcessIndex == 0)
                                monster.PlayHitParticle(bAdd, BuffType.POISON0);
                            else if (iProcessIndex == 1)
                                monster.ReduceHP(m_OriginData.DamagePoison);
                        }
                        break;
                    case BuffType.BURN0:
                    case BuffType.BURN1:
                    case BuffType.BURN2:
                        {
                            if (iProcessIndex == 0)
                                monster.PlayHitParticle(bAdd, BuffType.BURN0);
                            else if (iProcessIndex == 1)
                                monster.ReduceHP(m_OriginData.DamageFlame);
                        }
                        break;
                    case BuffType.STUN0:
                    case BuffType.STUN1:
                    case BuffType.STUN2:
                        {
                            if (iProcessIndex == 0)
                            {
                                if (bAdd)
                                    monster.bStun = true;
                                else
                                    monster.bStun = false;

                                monster.PlayHitParticle(bAdd, BuffType.STUN0);
                            }
                        }
                        break;
                    case BuffType.KNOCKBACK0:
                    case BuffType.KNOCKBACK1:
                    case BuffType.KNOCKBACK2:
                        {
                            monster.bKnockback = true;
                            bRemove = true;
                        }
                        break;
                }
            }

            if (bRemove)
                buffDictionary.Remove(eBuffType);
        }
    }

    public void OnDie()
    {
        foreach (var data in buffDictionary)
        {
            data.Value.m_Duration = 0;
            m_ExpiredBuffs.Add(data.Key);
        }
    }

    public bool IsBuff(BuffType eBuffType, bool bRoot)
    {
        BuffType compareBuffType = bRoot ? Oracle.GetBuffCategory(eBuffType) : eBuffType;
        foreach (var value in buffDictionary.Values)
        {
            if (value.m_eBuffType == eBuffType && value.m_Duration > 0)
            {
                return true;
            }
        }

        return false;
    }

    public BuffType GetBuffAndCheck(BuffType eBuffType)
    {
        BuffType compareBuffType = Oracle.GetBuffCategory(eBuffType);
        foreach (BuffType Key in buffDictionary.Keys)
        {
            if (compareBuffType == Oracle.GetBuffCategory(Key))
            {
                return Key;
            }
        }

        return BuffType.NONE;
    }

    public List<BuffType> GetBuffList()
    {
        List<BuffType> Buffs = new List<BuffType>();

        foreach (var Key in buffDictionary.Keys)
        {
            Buffs.Add(Key);
        }

        return Buffs;
    }

    public void Tick()
    {
        if (m_bUpdate == false)
            return;

        for (int i = 0; i < m_ExpiredBuffs.Count; ++i)
        {
            BuffType eBuffType = m_ExpiredBuffs[i];

            UpdateBuff(eBuffType, false, 0);
            RemoveBuff(eBuffType);
        }

        for (int i = 0; i < m_UpdateBuffs.Count; ++i)
        {
            BuffType eBuffType = m_UpdateBuffs[i];
            UpdateBuff(eBuffType, false, 1);
        }

        m_ExpiredBuffs.Clear();
        m_UpdateBuffs.Clear();

        m_bUpdate = false;
    }
    
    public int Length()
    {
        return buffDictionary.Count;
    }
}
