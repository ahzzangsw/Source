using GameDefines;
using OptionDefines;
using System;
using UnityEngine;
public static class Oracle
{
    static public int m_iBossPrefabCount = 0;
    static public bool m_bIntro = false;
    static public MapType m_eGameType = MapType.MAX;
    static public int MaxSpeciesKeyIndex = 4;

    public static int RandomDice(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    public static float RandomDice(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    public static bool PercentSuccess(float fPercent)   // 퍼센트를 넣어야함 ex 50% = 50
    {
        const int RandAccuracy = 10000000;
        if (fPercent < 0.0000001f)
        {
            fPercent = 0.0000001f;
        }

        fPercent /= 100;
        var success = false;

        var randHitRange = fPercent * RandAccuracy;
        var rand = UnityEngine.Random.Range(1, RandAccuracy + 1);
        if (rand <= randHitRange)
        {
            success = true;
        }
        return success;
    }

    public static UnityEngine.Color GetColor(AttributeType eAttributeType)
    {
        System.Drawing.Color drawingColor = System.Drawing.Color.WhiteSmoke;
        switch (eAttributeType)
        {
            case AttributeType.FIRE:
                drawingColor = System.Drawing.Color.Firebrick;
                break;
            case AttributeType.METAL:
                drawingColor = System.Drawing.Color.Gold;
                break;
            case AttributeType.LIGHTNING:
                drawingColor = System.Drawing.Color.LightGoldenrodYellow;
                break;
            case AttributeType.WATER:
                drawingColor = System.Drawing.Color.DeepSkyBlue;
                break;
            case AttributeType.DARK:
                drawingColor = System.Drawing.Color.Black;
                break;
            case AttributeType.LIGHT:
                drawingColor = System.Drawing.Color.NavajoWhite;
                break;
            case AttributeType.NO:
                drawingColor = System.Drawing.Color.LightGray;
                break;
        }

        UnityEngine.Color unityColor = new UnityEngine.Color(
            drawingColor.R / 255.0f,
            drawingColor.G / 255.0f,
            drawingColor.B / 255.0f,
            drawingColor.A / 255.0f);

        return unityColor;
    }

    public static string ConvertNumberDigit<T>(T value, bool bNumberColor = false)
    {
        string strResult = "";
        if (value is string)
        {
            if (int.TryParse(value.ToString(), out int intValue))
            {
                strResult = string.Format("{0:#,0}", intValue);
            }
        }
        else
        {
            strResult = string.Format("{0:#,0}", value);
            if (bNumberColor)
            {
                int iValue = Convert.ToInt32(value);
                if (iValue < 999)
                {
                    // Not
                }
                else if (iValue < 9999)
                {
                    strResult = string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGB(new Color(230f / 255f, 145f / 255f, 56f / 255f)), strResult);
                }
                else if (iValue < 99999)
                {
                    strResult = string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGB(new Color(171f / 255f, 14f / 255f, 0f)), strResult);
                }
                else if (iValue < 999999)
                {
                    strResult = string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGB(new Color(94f / 255f, 31f / 255f, 31f / 255f)), strResult);
                }
                else
                {
                    strResult = string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGB(new Color(255f / 255f, 0f, 0f)), strResult);
                }
            }
        }

        return strResult;
    }

    public static string ConvertSplitTime(uint fTime, bool showHour)
    {
        byte hour = 0, min = 0, sec = 0;
        if(fTime > 3600)
        {
            hour = (byte)(fTime / 3600);
            fTime -= 3600;
        }

        min = (byte)(fTime / 60);
        sec = (byte)(fTime % 60);
        if (showHour)
            return string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", sec);
        else
            return string.Format("{0:00}", min) + ":" + string.Format("{0:00}", sec);
    }

    public static string GetSpeciesTypeString(SpeciesType eSpeciesType, bool upper = false)
    {
        string SpeciesTypeText = "None";
        switch (eSpeciesType)
        {
            case SpeciesType.ELF:
                SpeciesTypeText = "Elf";
                break;
            case SpeciesType.HUMAN:
                SpeciesTypeText = "Human";
                break;
            case SpeciesType.DRAKE:
                SpeciesTypeText = "Drake";
                break;
            case SpeciesType.ORC:
                SpeciesTypeText = "Orc";
                break;
            case SpeciesType.SAMURAI:
                SpeciesTypeText = "Samurai";
                break;
            case SpeciesType.UNDEAD:
                SpeciesTypeText = "Undead";
                break;
            case SpeciesType.ANDROID:
                SpeciesTypeText = "Android";
                break;
            case SpeciesType.DARKELF:
                SpeciesTypeText = "DarkElf";
                break;
            case SpeciesType.DEMON:
                SpeciesTypeText = "Demon";
                break;
            case SpeciesType.DWARF:
                SpeciesTypeText = "Dwarf";
                break;
            case SpeciesType.FANATIC:
                SpeciesTypeText = "Fanatic";
                break;
            case SpeciesType.FISHMAN:
                SpeciesTypeText = "Fishman";
                break;
            case SpeciesType.FURRY:
                SpeciesTypeText = "Furry";
                break;
            case SpeciesType.GOBLIN:
                SpeciesTypeText = "Goblin";
                break;
            case SpeciesType.MONK:
                SpeciesTypeText = "Monk";
                break;
            case SpeciesType.NECROMANCER:
                SpeciesTypeText = "Necromancer";
                break;
            case SpeciesType.UNKNOWN:
                SpeciesTypeText = "Unknown";
                break;
            case SpeciesType.WIZARD:
                SpeciesTypeText = "Wizard";
                break;
            case SpeciesType.ZOMBIE:
                SpeciesTypeText = "Zombie";
                break;
        }

        if (upper)
            SpeciesTypeText = SpeciesTypeText.ToUpper();

        return SpeciesTypeText;
    }

    public static string GetCharacterName(SpeciesType eSpeciesType, int index, bool upper = false)
    {
        string CharacterName = "";
        switch (eSpeciesType)
        {
            case SpeciesType.NONE:
                CharacterName = "123";
                break;
            case SpeciesType.ELF:
                if (index == 0)
                    CharacterName = "Quarter Elf";
                else if (index == 1)
                    CharacterName = "Half Elf";
                else if (index == 2)
                    CharacterName = "Green Elf";
                else if (index == 3)
                    CharacterName = "Blue Elf";
                else
                    CharacterName = "High Elf";
                break;
            case SpeciesType.HUMAN:
                if (index == 0)
                    CharacterName = "Warrior";
                else if (index == 1)
                    CharacterName = "Archer(Bow)";
                else if (index == 2)
                    CharacterName = "Berserker";
                else if (index == 3)
                    CharacterName = "Archer(Crossbow)";
                else
                    CharacterName = "Spearman";
                break;
            case SpeciesType.DRAKE:
                if (index == 0)
                    CharacterName = "Drake-A";
                else if (index == 1)
                    CharacterName = "Drake-B";
                else if (index == 2)
                    CharacterName = "Drake-C";
                else if (index == 3)
                    CharacterName = "Red Drake";
                else
                    CharacterName = "Black Drake";
                break;
            case SpeciesType.ORC:
                if (index == 0)
                    CharacterName = "Orc Slave";
                else if (index == 1)
                    CharacterName = "Orc Peon";
                else if (index == 2)
                    CharacterName = "Orc Warrior";
                else if (index == 3)
                    CharacterName = "Orc Ax";
                else
                    CharacterName = "Orc Ax Master";
                break;
            case SpeciesType.SAMURAI:
                if (index == 0)
                    CharacterName = "Samurai Sword";
                else if (index == 1)
                    CharacterName = "Samurai Katana";
                else if (index == 2)
                    CharacterName = "Ninja Jounin";
                else if (index == 3)
                    CharacterName = "Ninja Junin";
                else
                    CharacterName = "Shogun";
                break;
            case SpeciesType.UNDEAD:
                if (index == 0)
                    CharacterName = "Zombie";
                else if (index == 1)
                    CharacterName = "Zombie";
                else if (index == 2)
                    CharacterName = "Skeleton";
                else if (index == 3)
                    CharacterName = "Undead Wizard";
                else
                    CharacterName = "Lich";
                break;
            case SpeciesType.ANDROID:
                if (index == 0)
                    CharacterName = "Cupcake";
                else if (index == 1)
                    CharacterName = "Donut";
                else if (index == 2)
                    CharacterName = "Lolipop";
                else if (index == 3)
                    CharacterName = "Pie";
                else
                    CharacterName = "Red Velvet Cake";
                break;
            case SpeciesType.DARKELF:
                if (index == 0)
                    CharacterName = "Belper Blakvon";
                else if (index == 1)
                    CharacterName = "Kasin Zahdoc";
                else if (index == 2)
                    CharacterName = "Gausniel Komoguth";
                else if (index == 3)
                    CharacterName = "Upvai Giedendros";
                else
                    CharacterName = "Poidun Giedenlas";
                break;
            case SpeciesType.DEMON:
                if (index == 0)
                    CharacterName = "Lesser Demon";
                else if (index == 1)
                    CharacterName = "Intermediate Demon";
                else if (index == 2)
                    CharacterName = "Devil";
                else if (index == 3)
                    CharacterName = "Great Devil";
                else
                    CharacterName = "Satan";
                break;
            case SpeciesType.DWARF:
                if (index == 0)
                    CharacterName = "Halffront";
                else if (index == 1)
                    CharacterName = "Thunderback";
                else if (index == 2)
                    CharacterName = "Laekihag";
                else if (index == 3)
                    CharacterName = "Bonkerk";
                else
                    CharacterName = "Muradin";
                break;
            case SpeciesType.FANATIC:
                if (index == 0)
                    CharacterName = "Faith";
                else if (index == 1)
                    CharacterName = "Belief";
                else if (index == 2)
                    CharacterName = "Calling";
                else if (index == 3)
                    CharacterName = "Pseudo elder";
                else
                    CharacterName = "Cult";
                break;
            case SpeciesType.FISHMAN:
                if (index == 0)
                    CharacterName = "John Wilson";
                else if (index == 1)
                    CharacterName = "Mike Iaconelli";
                else if (index == 2)
                    CharacterName = "Zane Grey";
                else if (index == 3)
                    CharacterName = "Roland Martin";
                else
                    CharacterName = "Hemingway";
                break;
            case SpeciesType.FURRY:
                if (index == 0)
                    CharacterName = "Shaggy";
                else if (index == 1)
                    CharacterName = "Bushy";
                else if (index == 2)
                    CharacterName = "Fluffy";
                else if (index == 3)
                    CharacterName = "Hairy";
                else
                    CharacterName = "Abundant";
                break;
            case SpeciesType.GOBLIN:
                if (index == 0)
                    CharacterName = "Worker Goblin";
                else if (index == 1)
                    CharacterName = "Norman Osborn";
                else if (index == 2)
                    CharacterName = "Trahald";
                else if (index == 3)
                    CharacterName = "Gongyou";
                else
                    CharacterName = "Goblin Lord";
                break;
            case SpeciesType.MONK:
                if (index == 0)
                    CharacterName = "Sancho Park";
                else if (index == 1)
                    CharacterName = "Bristlecone";
                else if (index == 2)
                    CharacterName = "Cus";
                else if (index == 3)
                    CharacterName = "Lees";
                else
                    CharacterName = "Presbyter";
                break;
            case SpeciesType.NECROMANCER:
                if (index == 0)
                    CharacterName = "Bonechild";
                else if (index == 1)
                    CharacterName = "WitchEndor";
                else if (index == 2)
                    CharacterName = "BabaYaga";
                else if (index == 3)
                    CharacterName = "Hecate";
                else
                    CharacterName = "Merlin";
                break;
            case SpeciesType.UNKNOWN:
                if (index == 0)
                    CharacterName = "unknown";
                else if (index == 1)
                    CharacterName = "uNKnown";
                else if (index == 2)
                    CharacterName = "UnKnowN";
                else if (index == 3)
                    CharacterName = "UNKNOWN";
                else
                    CharacterName = "Unknown";
                break;
            case SpeciesType.WIZARD:
                if (index == 0)
                    CharacterName = "Magician";
                else if (index == 1)
                    CharacterName = "Jafar";
                else if (index == 2)
                    CharacterName = "Glinda";
                else if (index == 3)
                    CharacterName = "Gandalf";
                else
                    CharacterName = "Dumbledore";
                break;
            case SpeciesType.ZOMBIE:
                if (index == 0)
                    CharacterName = "Zombie";
                else if (index == 1)
                    CharacterName = "Zombie";
                else if (index == 2)
                    CharacterName = "Zombie";
                else if (index == 3)
                    CharacterName = "Zombie";
                else
                    CharacterName = "ZombieKing";
                break;
        }

        if (upper)
            CharacterName = CharacterName.ToUpper();

        return CharacterName;
    }

    public static BuffType GetBuffCategory(BuffType eBuffType, bool filterCategory = false)
    {
        switch (eBuffType)
        {
            case BuffType.ARMORREDUCING0:
            case BuffType.ARMORREDUCING1:
            case BuffType.ARMORREDUCING2:
                return BuffType.ARMORREDUCING0;
            case BuffType.SLOW0:
            case BuffType.SLOW1:
            case BuffType.SLOW2:
                return BuffType.SLOW0;
            case BuffType.POISON0:
            case BuffType.POISON1:
            case BuffType.POISON2:
                if (filterCategory && eBuffType == BuffType.POISON2)
                {
                    return BuffType.POISON2;
                }
                return BuffType.POISON0;
            case BuffType.BURN0:
            case BuffType.BURN1:
            case BuffType.BURN2:
                return BuffType.BURN0;
            case BuffType.STUN0:
            case BuffType.STUN1:
            case BuffType.STUN2:
                return BuffType.STUN0;
            case BuffType.CRITICAL0:
            case BuffType.CRITICAL1:
            case BuffType.CRITICAL2:
                return BuffType.CRITICAL0;
            case BuffType.KNOCKBACK0:
            case BuffType.KNOCKBACK1:
            case BuffType.KNOCKBACK2:
                return BuffType.KNOCKBACK0;
            case BuffType.ATTACKSPEED_UP0:
            case BuffType.ATTACKSPEED_UP1:
            case BuffType.ATTACKSPEED_UP2:
            case BuffType.ATTACKSPEED_UP3:
            case BuffType.ATTACKSPEED_UP4:
                return BuffType.ATTACKSPEED_UP0;
            case BuffType.RANGE_UP0:
            case BuffType.RANGE_UP1:
            case BuffType.RANGE_UP2:
            case BuffType.RANGE_UP3:
            case BuffType.RANGE_UP4:
                return BuffType.RANGE_UP0;
            case BuffType.ATTACK_UP0:
            case BuffType.ATTACK_UP1:
            case BuffType.ATTACK_UP2:
            case BuffType.ATTACK_UP3:
            case BuffType.ATTACK_UP4:
                return BuffType.ATTACK_UP0;
            case BuffType.REDUCING:
                return BuffType.REDUCING;
            case BuffType.INCAPACITATE:
                return BuffType.INCAPACITATE;
            case BuffType.INJURY:
                return BuffType.INJURY;
        };

        return BuffType.NONE;
    }

    public static bool IsBuff(BuffType eBuffType)
    {
        if ((int)eBuffType - (int)BuffType.NONE2 > 0)
            return true;

        return false;
    }

    public static void SetBossPrefabCount(int bossPrefabCount)
    {
        m_iBossPrefabCount = bossPrefabCount;
    }

    public static SFXType GetSFXSoundType(WeaponType eWeaponType, bool bHit)
    {
        SFXType eSFXType = SFXType.SFX_NONE;

        switch (eWeaponType)
        {
            case WeaponType.ARROW:
                eSFXType = bHit ? SFXType.SFX_ARROW_HIT : SFXType.SFX_ARROW;        break;
            case WeaponType.BULLET:
                eSFXType = bHit ? SFXType.SFX_NONE : SFXType.SFX_BULLET;            break;
            case WeaponType.MISSILE:
                eSFXType = bHit ? SFXType.SFX_EXPLOSION0 : SFXType.SFX_MISSILE;     break;
            case WeaponType.BOMB:
                eSFXType = bHit ? SFXType.SFX_BOMB : SFXType.SFX_NONE;              break;
            case WeaponType.BEE:
                eSFXType = bHit ? SFXType.SFX_NONE : SFXType.SFX_BEE;               break;
            case WeaponType.AXE:
            case WeaponType.DANGGER:
                eSFXType = bHit ? SFXType.SFX_ARROW_HIT : SFXType.SFX_SWORD;        break;
            case WeaponType.SHURIKEN:
                eSFXType = bHit ? SFXType.SFX_SHURIKEN_HIT : SFXType.SFX_SHURIKEN;  break;
            case WeaponType.FIREBALL:
                eSFXType = bHit ? SFXType.SFX_FIREWORK_HIT : SFXType.SFX_FIREBALL;  break;
            case WeaponType.ICEBALL:
                eSFXType = bHit ? SFXType.SFX_ICEBALL_HIT : SFXType.SFX_ICEBALL;    break;
            case WeaponType.BONE:
            case WeaponType.BONETWINS:
                eSFXType = bHit ? SFXType.SFX_BONE : SFXType.SFX_THROW;             break;
            case WeaponType.CHERRY:
                eSFXType = bHit ? SFXType.SFX_CHERRY_HIT : SFXType.SFX_ARROW;       break;
            case WeaponType.LEAF:
                eSFXType = bHit ? SFXType.SFX_LEAF_HIT : SFXType.SFX_ARROW; break;
            case WeaponType.SNOW:
                eSFXType = bHit ? SFXType.SFX_SNOW_HIT : SFXType.SFX_SNOW; break;
            case WeaponType.WATER:
                eSFXType = bHit ? SFXType.SFX_WATER_HIT : SFXType.SFX_THROW; break;
            case WeaponType.BUBBLE:
                eSFXType = bHit ? SFXType.SFX_BUBBLE_HIT : SFXType.SFX_THROW; break;
            case WeaponType.LIGHT:
                eSFXType = bHit ? SFXType.SFX_LIGHT_HIT : SFXType.SFX_THROW; break;
            case WeaponType.LIGHTNING:
                eSFXType = bHit ? SFXType.SFX_LIGHTNING_HIT : SFXType.SFX_THROW; break;
            case WeaponType.LASER:
                eSFXType = bHit ? SFXType.SFX_LASER_HIT : SFXType.SFX_THROW; break;
            case WeaponType.STONE:
                eSFXType = bHit ? SFXType.SFX_STONE_HIT : SFXType.SFX_THROW; break;
            case WeaponType.DEBRIS:
                eSFXType = bHit ? SFXType.SFX_DEBRIS_HIT : SFXType.SFX_THROW; break;
            case WeaponType.GRAVITY:
                eSFXType = bHit ? SFXType.SFX_GRAVITY_HIT : SFXType.SFX_THROW; break;
            case WeaponType.MAGNETIC:
                eSFXType = bHit ? SFXType.SFX_MAGNETIC_HIT : SFXType.SFX_THROW; break;
            case WeaponType.POISON:
                eSFXType = bHit ? SFXType.SFX_POISON : SFXType.SFX_THROW; break;
            case WeaponType.SULFURICACID:
                eSFXType = bHit ? SFXType.SFX_SULFURICACID_HIT : SFXType.SFX_SULFURICACID; break;
            case WeaponType.CONSTELLATION:
                eSFXType = bHit ? SFXType.SFX_CONSTELLATION_HIT : SFXType.SFX_THROW; break;
            case WeaponType.ASTRAPHE:
                eSFXType = bHit ? SFXType.SFX_ASTRAPHE_HIT : SFXType.SFX_THROW; break;
            case WeaponType.SMALLMETEOR:
                eSFXType = bHit ? SFXType.SFX_SMALLMETEOR_HIT : SFXType.SFX_THROW; break;
            case WeaponType.METEOR:
                eSFXType = bHit ? SFXType.SFX_EXPLOSION1 : SFXType.SFX_METEOR; break;
            case WeaponType.AURA:
                eSFXType = bHit ? SFXType.SFX_PULSE : SFXType.SFX_THROW; break;
            case WeaponType.PULSE:
                eSFXType = bHit ? SFXType.SFX_PULSE : SFXType.SFX_THROW; break;
            case WeaponType.BLOOD:
                eSFXType = bHit ? SFXType.SFX_BLOOD_HIT : SFXType.SFX_NONE; break;
            case WeaponType.HEART:
                eSFXType = bHit ? SFXType.SFX_HEART_HIT : SFXType.SFX_THROW; break;
            case WeaponType.FIREWORKS:
                eSFXType = bHit ? SFXType.SFX_FIREWORK_HIT : SFXType.SFX_THROW; break;
            case WeaponType.PURPLESPARKS:
                eSFXType = bHit ? SFXType.SFX_PURPLESPARKS : SFXType.SFX_THROW; break;
            case WeaponType.REDSPARKS:
                eSFXType = bHit ? SFXType.SFX_REDSPARKS : SFXType.SFX_THROW; break;
            case WeaponType.CUTTER:
                eSFXType = bHit ? SFXType.SFX_CUTTER_HIT : SFXType.SFX_THROW; break;
            case WeaponType.CROSSBOW:
                eSFXType = bHit ? SFXType.SFX_ARROW_HIT : SFXType.SFX_CROSSBOW; break;
            case WeaponType.RIFLE:
                eSFXType = bHit ? SFXType.SFX_NONE : SFXType.SFX_RIFLE; break;
            case WeaponType.HEAT:
                eSFXType = bHit ? SFXType.SFX_EXPLOSION2 : SFXType.SFX_NONE; break;
            case WeaponType.SPEAR:
                eSFXType = bHit ? SFXType.SFX_SPEAR_HIT : SFXType.SFX_ARROW; break;
            case WeaponType.SHOCKWAVE:
                eSFXType = bHit ? SFXType.SFX_SHOCKWAVE : SFXType.SFX_THROW; break;
            case WeaponType.FORESTARROW:
                eSFXType = bHit ? SFXType.SFX_LEAF_HIT : SFXType.SFX_ARROW; break;
            case WeaponType.CUBE:
                eSFXType = bHit ? SFXType.SFX_CUBE_HIT : SFXType.SFX_THROW; break;
            case WeaponType.BIGAXE:
                eSFXType = bHit ? SFXType.SFX_BIGAXE_HIT : SFXType.SFX_THROW; break;
        }

        return eSFXType;
    }

    //steam
    public static string GetAchievementAPIName_Stage(byte stageIndex)
    {
        switch (stageIndex) 
        {
            case 11:
                return "CURSOR10";
            case 21:
                return "CURSOR20";
            case 31:
                return "CURSOR30";
            case 41:
                return "CURSOR40";
            case 51:
                return "CURSOR50";
            case 61:
                return "CURSOR60";
            case 71:
                return "CURSOR70";
            case 81:
                return "CURSOR80";
            case 91:
                return "CURSOR90";
            case 101:
                return "CURSOR100";
        };

        return "";
    }
}
