using GameDefines;
using System.Collections.Generic;
using UnityEngine;

public static class CalculationDamageFormula
{
    public static int CalculationDamage(Building hitBuilding, MonsterBase hitMonster)
    {
        float fDamage = 1f;
        if (hitBuilding && hitMonster)
        {
            int addDamage = 0;
            Player MyPlayer = GameManager.Instance.GetPlayer();
            if (MyPlayer != null)
            {
                ControlInfo controlInfoData = MyPlayer.GetCurrentUpgradeData(hitBuilding.m_eSpeciesType);
                addDamage = controlInfoData.UnitAtk[hitBuilding.m_CharacterIndex];
            }

            fDamage = hitBuilding.Damage + addDamage;

            // CalculationCritical
            if (Oracle.GetBuffCategory(hitBuilding.eBuffType) == BuffType.CRITICAL0)
            {
                int iValue = ConvertBuffPercentAndValue(hitBuilding.eBuffType);
                //int iPercent = Oracle.RandomDice(0, 100);
                //if (iPercent < iValue)
                if (Oracle.PercentSuccess(iValue))
                {
                    if (hitBuilding.eBuffType == BuffType.CRITICAL2)
                        fDamage *= 3f;
                    else
                        fDamage *= 2f;

                    // DamageFont
                    hitMonster.BeHitCritical(fDamage);
                }
            }

            // CalculationDefense
            int Defense = hitMonster.Defense;
            if (Defense > 0)
            {
                if (hitMonster.m_BuffContainer.IsBuff(BuffType.ARMORREDUCING0, true))
                {
                    BuffType curBuffType = hitMonster.m_BuffContainer.GetBuffAndCheck(BuffType.ARMORREDUCING0);

                    int fDefense_Reducing = ConvertBuffPercentAndValue(curBuffType);
                    Defense -= fDefense_Reducing;
                    if (Defense < 0)
                        Defense = 0;
                }

                fDamage -= (fDamage * Defense / 100);
            }

            // CalculationSameSpecie - 50%
            if (hitBuilding.m_eSpeciesType == hitMonster.m_eSpeciesType)
            {
                fDamage -= (fDamage * 0.5f);
            }

            // CalculationAttribute
            AttributeType hitAttributeType = AttributeType.NONE;// hitBuilding.m_eAttributeType;
            AttributeType beHitAttributeType = AttributeType.NONE; //hitMonster.m_eAttributeType;
            fDamage *= CalculationAttribute(hitAttributeType, beHitAttributeType);

            // ApplyBuff
            AddBuff(hitBuilding, hitMonster);
        }

        fDamage += 0.5f;    // 반올림 처리
        return (int)fDamage;
    }

    public static float CalculationAttribute(AttributeType hitAttributeType, AttributeType beHitAttributeType)
    {
        switch (hitAttributeType)
        {
            case AttributeType.FIRE:       ///< 불(금속 150% 추가 데미지, 물 150% 피해)    - 빨간색
                if (beHitAttributeType == AttributeType.METAL)
                {
                    return 1.5f;
                }
                break;
            case AttributeType.METAL:      ///< 금(전기 150% 추가 데미지, 불 150% 피해)    - 금색
                if (beHitAttributeType == AttributeType.LIGHTNING)
                {
                    return 1.5f;
                }
                break;
            case AttributeType.LIGHTNING:  ///< 전기(물 150% 추가 데미지, 금 150% 피해)    - 노란색
                if (beHitAttributeType == AttributeType.WATER)
                {
                    return 1.5f;
                }
                break;
            case AttributeType.WATER:      ///< 물(불 150% 추가 데미지, 전기 150% 피해)    - 파란색
                if (beHitAttributeType == AttributeType.FIRE)
                {
                    return 1.5f;
                }
                break;
            case AttributeType.DARK:       ///< 어둠(빛 150% 추가 데미지, 어둠 150% 피해)  - 검은색
                if (beHitAttributeType == AttributeType.LIGHT)
                {
                    return 1.5f;
                }
                break;
            case AttributeType.LIGHT:      ///< 빛(어둠 150% 추가 데미지, 빛 150% 피해)    - 하얀색
                if (beHitAttributeType == AttributeType.DARK)
                {
                    return 1.5f;
                }
                break;
            case AttributeType.NO:         ///< 무(전체 120% 추가 데미지, 전체 100% 피해)  - 회색
                return 1.5f;
        }

        return 1f;
    }

    public static int ConvertBuffPercentAndValue(BuffType eBuff)
    {
        switch (eBuff)
        {
            case BuffType.STEAL0:
            case BuffType.STEAL1:
            case BuffType.STEAL2:
            case BuffType.STEAL3:
                if (Oracle.m_eGameType == MapType.ADVENTURE)
                    return 0;

                return 1;
            case BuffType.STUN0:
            case BuffType.ATTACK_UP0:
                return 5;
            case BuffType.STUN1:
                return 7;
            case BuffType.SLOW0:
            case BuffType.STUN2:
            case BuffType.CRITICAL0:
            case BuffType.KNOCKBACK0:
            case BuffType.POISON0:
            case BuffType.ATTACK_UP1:
            case BuffType.ATTACKSPEED_UP0:
            case BuffType.ARMORREDUCING0:
                return 10;
            case BuffType.CRITICAL2:
            case BuffType.KNOCKBACK1:
            case BuffType.ATTACK_UP2:
                return 12;
            case BuffType.CRITICAL1:
            case BuffType.KNOCKBACK2:
            case BuffType.ATTACK_UP3:
            case BuffType.ATTACKSPEED_UP1:
                return 15;
            case BuffType.SLOW1:
            case BuffType.ARMORREDUCING1:
            case BuffType.POISON1:
            case BuffType.ATTACK_UP4:
            case BuffType.ATTACKSPEED_UP2:
                return 20;
            case BuffType.SLOW2:
            case BuffType.ARMORREDUCING2:
            case BuffType.ATTACKSPEED_UP3:
            case BuffType.POISON2:
                return 30;
            case BuffType.ATTACKSPEED_UP4:
                return 40;
            case BuffType.BURN0:
            case BuffType.BURN1:
            case BuffType.BURN2:
                return 100;
        };           

        return 0;
    }

    public static void AddBuff(Building hitBuilding, MonsterBase hitMonster)
    {
        int iValue = ConvertBuffPercentAndValue(hitBuilding.eBuffType);
        //int iPercent = Oracle.RandomDice(0, 100);
        //if (iPercent < iValue)
        if (Oracle.PercentSuccess(iValue))
        {
            if (Oracle.IsBuff(hitBuilding.eBuffType))
            {
                if (Oracle.m_eGameType == MapType.ADVENTURE)
                {
                    Player MyPlayer = GameManager.Instance.GetPlayer();
                    if (MyPlayer)
                    {
                        MyPlayer.AddBuffActor(hitBuilding.eBuffType);
                    }
                }
                else
                {
                    List<GameObject> buildingObjectList = BuildingPool.Instance.GetBuildingList();
                    if (buildingObjectList != null)
                    {
                        for (int i = 0; i < buildingObjectList.Count; ++i)
                        {
                            GameObject buildingObject = buildingObjectList[i];
                            if (buildingObject == null)
                                continue;

                            float distance = Vector3.Distance(buildingObject.transform.position, hitBuilding.transform.position);
                            if (distance <= hitBuilding.Range)
                            {
                                Building nearBuilding = buildingObject.GetComponent<Building>();
                                if (nearBuilding)
                                    nearBuilding.AddBuffActor(hitBuilding.eBuffType);
                            }
                        }
                    }
                }
            }
            else
            {
                if (hitMonster.bOnBuff)
                    hitMonster.AddDeBuffActor(hitBuilding);
            }
        }
    }
}
