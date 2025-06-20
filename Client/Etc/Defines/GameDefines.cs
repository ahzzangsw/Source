using System.Collections.Generic;
using UnityEngine;

public static class TypeDefs
{
    public static readonly Vector3 VECTOR_NONE = new Vector3(0f, 0f, -99f);
}

namespace GameDefines
{
    ///////////////////////////////// Enum /////////////////////////////////
    public enum MapType
    {
        BUILD,
        SPAWN,
        ADVENTURE,
        MAX,
    }

    public enum SpeciesType    //종족
    {
        NONE = 0,
        ELF,
        HUMAN,
        DRAKE,
        ORC,
        SAMURAI,
        UNDEAD,
        ANDROID,
        DARKELF,
        DEMON,
        DWARF,
        FANATIC,
        FISHMAN,
        FURRY,
        GOBLIN,
        MONK,
        NECROMANCER,
        UNKNOWN,
        WIZARD,
        ZOMBIE,
        // Add...
        MAX,
        GOLDGOBLIN, // Special
    };

    public enum AttributeType   //속성 (상성관계 무 : 불 > 금 > 전기 > 물 > 불 : 어둠 >< 빛)
    {
        NONE = 0,
        FIRE,       ///< 불(금속 150% 추가 데미지, 물 150% 피해)    - 빨간색
        METAL,      ///< 금(전기 150% 추가 데미지, 불 150% 피해)    - 금색
        LIGHTNING,  ///< 전기(물 150% 추가 데미지, 금 150% 피해)    - 노란색
        WATER,      ///< 물(불 150% 추가 데미지, 전기 150% 피해)    - 파란색
        DARK,       ///< 어둠(빛 150% 추가 데미지, 어둠 150% 피해)  - 검은색
        LIGHT,      ///< 빛(어둠 150% 추가 데미지, 빛 150% 피해)    - 하얀색
        NO,         ///< 무(전체 120% 추가 데미지, 전체 100% 피해)  - 없음
        // Add...
        MAX,
    };

    public enum BuffType
    {
        // Monster
        NONE = 0,
        ARMORREDUCING0,     ///< 방깍 10%
        ARMORREDUCING1,     ///< 방깍 15%
        ARMORREDUCING2,     ///< 방깍 25%
        SLOW0,              ///< 슬로우 5%
        SLOW1,              ///< 슬로우 10%
        SLOW2,              ///< 슬로우 20%
        POISON0,            ///< 독(지속 데미지) 3초동안 1초마다 전체HP의 5%(10% 발생)
        POISON1,            ///< 독(지속 데미지) 3초동안 1초마다 전체HP의 5%(20% 발생)
        POISON2,            ///< 독(지속 데미지) 3초동안 1초마다 전체HP의 10%(30% 발생)
        BURN0,             ///< 화염(지속 데미지) 2초간 공격력 10%(100% 발생)
        BURN1,             ///< 화염(지속 데미지) 3초간 공격력 20%(100% 발생)
        BURN2,             ///< 화염(지속 데미지) 4초간 공격력 35%(100% 발생)
        STUN0,              ///< 1초 기절 5%
        STUN1,              ///< 1초 기절 7%
        STUN2,              ///< 1초 기절 10%
        CRITICAL0,          ///< 2배 데미지 10%
        CRITICAL1,          ///< 2배 데미지 15%
        CRITICAL2,          ///< 3배 데미지 12%
        KNOCKBACK0,         ///< 뒤로 1 밀려나기 10%
        KNOCKBACK1,         ///< 뒤로 1 밀려나기 12%
        KNOCKBACK2,         ///< 뒤로 1 밀려나기 15%

        // Building
        NONE2 = 1000,
        ATTACK_UP0,         ///< 공격력 증가 5%
        ATTACK_UP1,         ///< 공격력 증가 10%
        ATTACK_UP2,         ///< 공격력 증가 12%
        ATTACK_UP3,         ///< 공격력 증가 15%
        ATTACK_UP4,         ///< 공격력 증가 20%
        ATTACKSPEED_UP0,    ///< 공격속도 증가 10%
        ATTACKSPEED_UP1,    ///< 공격속도 증가 15%
        ATTACKSPEED_UP2,    ///< 공격속도 증가 20%
        ATTACKSPEED_UP3,    ///< 공격속도 증가 30%
        ATTACKSPEED_UP4,    ///< 공격속도 증가 40%
        RANGE_UP0,          ///< 사정거리 증가 10%
        RANGE_UP1,          ///< 사정거리 증가 20%
        RANGE_UP2,          ///< 사정거리 증가 30%
        RANGE_UP3,          ///< 사정거리 증가 50%
        RANGE_UP4,          ///< 사정거리 증가 100%
        STEAL0,             ///< 돈뺏기 1%확률로 빌딩 가격의 10% 얻기
        STEAL1,             ///< 돈뺏기 5%확률로 빌딩 가격의 10% 얻기
        STEAL2,             ///< 돈뺏기 10%확률로 빌딩 가격의 15% 얻기
        STEAL3,             ///< 돈뺏기 15%확률로 빌딩 가격의 20% 얻기
        INCAPACITATE,       ///< 무력화
        REDUCING,           ///< 감소
        DIE,                ///< 사망
        INJURY,             ///< 이속 99% 감속
        MAX,
    };

    public enum WeaponType
    { 
        NONE = 0,   // 발사체없음(근접공격)
        // Sprite
        ARROW,              // 화살
        BULLET,             // 총알
        MISSILE,            // 미사일
        BOMB,               // 폭탄
        BEE,                // 벌
        AXE,                // 도끼
        DANGGER,            // 단검
        SHURIKEN,           // 수리검
        FIREBALL,           // 파이어볼
        ICEBALL,            // 아이스볼
        BONE,               // 뼈
        BONETWINS,          // 쌍뼈
        // Paticle
        MAGIC,              // - 마법 대분류
        CHERRY,             // 체리
        LEAF,               // 나뭇잎
        SNOW,               // 눈
        SNOWFLAKE,          // 눈송이
        WATER,              // 물
        BUBBLE,             // 버블
        WIND,               // 바람
        LIGHT,              // 빛
        LIGHTNING,          // 번개
        LASER,              // 레이져
        STONE,              // 돌
        DEBRIS,             // 부수러기
        GRAVITY,            // 중력장
        MAGNETIC,           // 자기장
        POISON,             // 독
        SULFURICACID,       // 황산
        STAR,               // 별
        CONSTELLATION,      // 별자리
        ASTRAPHE,           // 아스트라페
        SMALLMETEOR,        // 작은 메테오
        METEOR,             // 메테오
        AURA,               // 아우라
        PULSE,              // 펄스
        STICKY,             // 끈적끈적
        BLOOD,              // 피
        HEART,              // 하트
        FIREWORKS,          // 폭죽
        PURPLESPARKS,       // 보라색스파크
        REDSPARKS,          // 빨간스파크
        CUTTER,             // 커터
        CROSSBOW,           // 석궁
        RIFLE,              // 라이플
        HEAT,               // 대전차무기
        SPEAR,              // 스피어
        SHOCKWAVE,          // 공기팡
        FORESTARROW,        // 숲 화살
        WINDARROW,          // 바람 화살
        LIGHTNINGARROW,     // 번개 화살
        SPEARARROW,         // 스피어 화살
        CUBE,               // 큐브
        BIGAXE,             // 큰 도끼
        ENEMYBALL,          // 적 총알
        ENEMYSTONE,         // 적 돌
        ENEMYSHOCKWAVE,     // 적 쇼크웨이브
        ENEMYMELEE,         // 적 근접공격
        MAX,
    }

    public enum MagicType
    {
        NONE = 0,       // 직선으로
        ONETIME_DOWN,   // 뿌악
        BOUNCE,         // 튕튕
        MAX,
    }

    public enum ProjectileType
    {
        NONE = 0,   ///< 근접공격
        SHOT,       ///< 기본(1개)
        MULTISHOT,  ///< 멀티샷(2~5개)
        SPLASH,     ///< 스플레시(100%, 50%)
        DOT,        ///< 레이져
        PENETRATE,  ///< 관통
        MAGIC,      ///< 마법(해당위치에 지속적 범위공격)
        BOUNCES,    ///< 바운스(2~5개)
        BREATH,     ///< 브레스
        BOMB,       ///< 폭탄(충돌후 폭파)
        ADDSHOT,    ///< 에드샷(2~5개)
        MAX,
    }

    public enum EffectType
    { 
        NONE = 0,
        EXPLOSION_MISSILE,
        EXPLOSION_STRAIGHT_CENTER,
        EXPLOSION_STRAIGHT_CORNER,
        EXPLOSION_STRAIGHT_MIDDLE,
        EXPLOSION_METEOR,
        MAX,
    }

    public enum ClickTargetType
    {
        NONE = 0,
        BUILDING,
        MONSTER,
        BOSS,
        FINAL,
        SPAWN,
        PLAYER,
        MAX,
    }

    public enum MeleeType 
    {
        NONE = 0, 
        HAND,
        STICK, 
        SWORD, 
        SPEAR,
    }

    public enum ItemType
    {
        NONE = 0,
        RUBY,
        MONEY,
        MAX,
    }

    public enum AdventureLevelType
    {
        LEVEL1,
        LEVEL2,
        LEVEL3,
        LEVEL4,
        NONE,
    }

    public enum AdventureLevelUpItemType
    {
        CHARACTER,
        STAT,
        UTIL,
        MAX,
    }

    public enum AdventureLevelUpStatType
    {
        DAMAGE,
        ATTACKSPEED,
        RANGE,
        MOVE,
        JUMP,
        MAX,
    }

    public enum AdventurePrefabsType : int
    {
        BOSS,
        BARREL,
        POOP0,
        POOP1,
        POOP2,
        LASER,
        BEE,
        GREENTROLL,
        BLUETROLL,
        BLACKTROLL,
        GREEENOGRE,
        REDOGRE,
        PURPLEOGRE,
        BROWNWEREWOLF,
        REDWEREVOLF,
        BLACKWEREWOLF,
        GREENREX,
        BLUEREX,
        REDREX,
        GREENMEGAPUMPKIN,
        PURPLEMEGAPUMPKIN,
        YELLOWMEGAPUMPKIN,
        GREENDRAGON,
        BLUEDRAGON,
        REDDRAGON,
        LASTBOSS,
        MAX,
    }

    public enum PhaseStep
    {
        None,
        PhaseStep0,
        PhaseStep1,
        PhaseStep2,
    }

    ///////////////////////////////// Struct /////////////////////////////////
    public struct StageInfo
    {
        public int Hp;
        public int Defense;
        public float moveSpeed;
        public short money;
        public short stageMoney;
        public short nextStartTime;

        public StageInfo(int i0, int i1, float f2, short i3, short i4, short i5)
        {
            Hp = i0;
            Defense = i1;
            moveSpeed = f2;
            money = i3;
            stageMoney = i4;
            nextStartTime = i5;
        }

        public StageInfo(StageInfo refStageInfo)
        {
            Hp = refStageInfo.Hp;
            Defense = refStageInfo.Defense;
            moveSpeed = refStageInfo.moveSpeed;
            money = refStageInfo.money;
            stageMoney = refStageInfo.stageMoney;
            nextStartTime = refStageInfo.nextStartTime; 
        }
    }
    public struct StageInfo_Adv
    {
        public int Hp;
        public int Defense;
        public float moveSpeed;
        public float Range;
        public int Attack;
        public int Count;

        public StageInfo_Adv(int i0, int i1, float f0, float f1, int i3, int i4)
        {
            Hp = i0;
            Defense = i1;
            moveSpeed = f0;
            Range = f1;
            Attack = i3;
            Count = i4;
        }

        public StageInfo_Adv(StageInfo_Adv refStageInfo)
        {
            Hp = refStageInfo.Hp;
            Defense = refStageInfo.Defense;
            moveSpeed = refStageInfo.moveSpeed;
            Range = refStageInfo.Range;
            Attack = refStageInfo.Attack;
            Count = refStageInfo.Count;
        }
    }

    public struct BuildingInfo
    {
        public int Damage;
        public float Range;
        public float AttackSpeed;
        public int Cost;
        public byte TargetCount;
        public BuffType eBuffType;
        public string Description;

        public BuildingInfo(int i0, float f0, float f1, int i1, byte b1, BuffType eBuff)
        {
            Damage = i0;
            Range = f0;
            AttackSpeed = f1;
            Cost = i1;
            TargetCount = b1;
            eBuffType = eBuff;
            Description = "";
        }

        public BuildingInfo(int i0, float f0, float f1, int i1, byte b1, BuffType eBuff, string s0)
        {
            Damage = i0;
            Range = f0;
            AttackSpeed = f1;
            Cost = i1;
            TargetCount = b1;
            eBuffType = eBuff;
            Description = s0;
        }
    }

    public struct CursorInfo
    {
        public short Money;
        public float Damage;
        public float Range;
        public float AttackSpeed;
        public string tooltip;

        public CursorInfo(short s0, float f0, float f1, float f2, string text)
        {
            Money = s0;
            Damage = f0;
            Range = f1;
            AttackSpeed = f2;
            tooltip = text;
        }
    }

    public struct ShopInfo
    {
        public string Name;
        public string Desc;
        public int Cost;
        public SpeciesType eSpeciesType;
        public int SlotId;

        public ShopInfo(string s0, string s1, int i0, SpeciesType e0, int i2)
        {
            Name = s0;
            Desc = s1;
            Cost = i0;
            SlotId = i2;

            if (e0 == SpeciesType.MAX)
            {
                eSpeciesType = SpeciesType.MAX;
            }
            else
            {
                eSpeciesType = e0;
            }

        }
    }

    public struct ControlInfo
    {
        public int Cost;
        public int[] UnitAtk;
        public int WorkmanCost;
        public int[] BossMoneyEarnedList;
        public int[] BossHPList;
        public int Grade;

        public ControlInfo(int i0, int i1, int i2, int i3, int i4, int i5, int i6, int[] ilist0, int[] ilist1)
        {
            Cost = i0;
            UnitAtk = new int[] { i1, i2, i3, i4, i5 };
            WorkmanCost = i6;
            BossMoneyEarnedList = ilist0;
            BossHPList = ilist1;
            Grade = 0;
        }

        public void SetGrade(int grade)
        {
            Grade = grade;
        }
    }

    public struct KeyAchievementInfo
    {
        public string Name;
        public string SteamName;
        public int Money;
        public KeyCode ChoiceKey;
        public List<KeyCode> AllKeyCodes;

        public KeyAchievementInfo(KeyAchievementInfo tempKeyAchievementInfo)
        {
            Name = tempKeyAchievementInfo.Name;
            SteamName = tempKeyAchievementInfo.SteamName;
            Money = tempKeyAchievementInfo.Money;
            ChoiceKey = tempKeyAchievementInfo.ChoiceKey;
            AllKeyCodes = new List<KeyCode>(); 
            for(int i = 0; i < tempKeyAchievementInfo.AllKeyCodes.Count; ++i)
            {
                AllKeyCodes.Add(tempKeyAchievementInfo.AllKeyCodes[i]);
            }
        }
        public KeyAchievementInfo(string s0, string s1, int i0, params KeyCode[] keyCodes)
        {
            Name = s0;
            SteamName = s1;
            Money = i0;
            ChoiceKey = KeyCode.None;
            AllKeyCodes = new List<KeyCode>();

            for (int i = 0; i < keyCodes.Length; ++i)
            {
                AllKeyCodes.Add(keyCodes[i]);
            }
        }

        public void ShuffleChoiceKey()
        {
            if (AllKeyCodes.Count == 0)
                return;

            KeyCode[] keyCodes = AllKeyCodes.ToArray();

            int index = Oracle.RandomDice(0, keyCodes.Length);
            ChoiceKey = keyCodes[index];

            AllKeyCodes.Clear();
            for (int i = 0; i < keyCodes.Length; ++i)
            {
                AllKeyCodes.Add(keyCodes[i]);
            }
        }
    }

    public struct UnitAchievementInfo
    {
        public string Name;
        public string SteamName;
        public int Money;
        public SpeciesType[] AchievementSpeciesType;

        public UnitAchievementInfo(string s0, string s1, int i0, params SpeciesType[] speciesTypes)
        {
            Name = s0;
            SteamName = s1;
            Money = i0;
            AchievementSpeciesType = new SpeciesType[speciesTypes.Length];
            if (speciesTypes.Length == 0)
                return;

            for (int i = 0; i < AchievementSpeciesType.Length; ++i)
            {
                AchievementSpeciesType[i] = speciesTypes[i];
            }
        }
    }

    public struct RankInfo_Spawn
    {
        public int score;
        public int time;
        public string name;
        public bool mine;
        public RankInfo_Spawn(int i0, int i1, string s0, bool b0)
        {
            score = i0;
            time = i1;
            name = s0;
            mine = b0;
        }
    }

    // 전투타입별 기본 추가 스탯
    public struct PlayerAdventureBasicInfo
    {
        //Stat
        public int AddDamagePer;
        public float AddRangePer;
        public float moveSpeed;
        public float jumpSpeed;
        public float AddatkSpeedPer;
        public byte TargetCount;

        public PlayerAdventureBasicInfo(int i0, float f0, float f1, float f2, float f3, int i1)
        {
            AddDamagePer = i0;
            AddRangePer = f0;
            moveSpeed = f1;
            jumpSpeed = f2;
            AddatkSpeedPer = f3;
            TargetCount = (byte)i1;
        }

        public static PlayerAdventureBasicInfo operator +(PlayerAdventureBasicInfo A, PlayerAdventureBasicInfo B)
        {
            return new PlayerAdventureBasicInfo(A.AddDamagePer + B.AddDamagePer,
                A.AddRangePer + B.AddRangePer,
                A.moveSpeed + B.moveSpeed,
                A.jumpSpeed + B.jumpSpeed,
                A.AddatkSpeedPer + B.AddatkSpeedPer,
                A.TargetCount + B.TargetCount);
        }
    }

    public struct LevelUpSystemInfo
    {
        public AdventureLevelUpItemType eAdventureLevelUpItemType;
        public string Description;

        // Character
        // Stat
        public AdventureLevelUpStatType eAdventureLevelUpStatType;
        // Skill
        public WeaponType eWeaponType;
        // Util
        public int ValueType;

        public float[] valueList;

        public LevelUpSystemInfo(AdventureLevelUpItemType keyType, string inDescription)
        {
            eAdventureLevelUpItemType = keyType;
            Description = inDescription;

            eAdventureLevelUpStatType = AdventureLevelUpStatType.MAX;
            eWeaponType = WeaponType.NONE;
            ValueType = -1;
            valueList = null;
        }

        public LevelUpSystemInfo(AdventureLevelUpItemType keyType, AdventureLevelUpStatType valueType, string inDescription, params float[] values)
        {
            eAdventureLevelUpItemType = keyType;
            eAdventureLevelUpStatType = valueType;
            Description = inDescription;

            valueList = new float[values.Length];
            for (int i = 0; i < valueList.Length; ++i)
            {
                valueList[i] = values[i];
            }

            eWeaponType = WeaponType.NONE;
            ValueType = -1;
        }

        public LevelUpSystemInfo(AdventureLevelUpItemType keyType, WeaponType valueType, string inDescription, params float[] values)
        {
            eAdventureLevelUpItemType = keyType;
            eWeaponType = valueType;
            Description = inDescription;

            valueList = new float[values.Length];
            for (int i = 0; i < valueList.Length; ++i)
            {
                valueList[i] = values[i];
            }

            eAdventureLevelUpStatType = AdventureLevelUpStatType.MAX;
            ValueType = -1;
        }

        public LevelUpSystemInfo(AdventureLevelUpItemType keyType, int valueType, string inDescription, params float[] values)
        {
            eAdventureLevelUpItemType = keyType;
            ValueType = valueType;
            Description = inDescription;

            valueList = new float[values.Length];
            for (int i = 0; i < valueList.Length; ++i)
            {
                valueList[i] = values[i];
            }

            eAdventureLevelUpStatType = AdventureLevelUpStatType.MAX;
            eWeaponType = WeaponType.NONE;
        }
    }
}