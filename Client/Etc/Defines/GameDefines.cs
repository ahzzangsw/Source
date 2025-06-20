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

    public enum SpeciesType    //����
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

    public enum AttributeType   //�Ӽ� (�󼺰��� �� : �� > �� > ���� > �� > �� : ��� >< ��)
    {
        NONE = 0,
        FIRE,       ///< ��(�ݼ� 150% �߰� ������, �� 150% ����)    - ������
        METAL,      ///< ��(���� 150% �߰� ������, �� 150% ����)    - �ݻ�
        LIGHTNING,  ///< ����(�� 150% �߰� ������, �� 150% ����)    - �����
        WATER,      ///< ��(�� 150% �߰� ������, ���� 150% ����)    - �Ķ���
        DARK,       ///< ���(�� 150% �߰� ������, ��� 150% ����)  - ������
        LIGHT,      ///< ��(��� 150% �߰� ������, �� 150% ����)    - �Ͼ��
        NO,         ///< ��(��ü 120% �߰� ������, ��ü 100% ����)  - ����
        // Add...
        MAX,
    };

    public enum BuffType
    {
        // Monster
        NONE = 0,
        ARMORREDUCING0,     ///< ��� 10%
        ARMORREDUCING1,     ///< ��� 15%
        ARMORREDUCING2,     ///< ��� 25%
        SLOW0,              ///< ���ο� 5%
        SLOW1,              ///< ���ο� 10%
        SLOW2,              ///< ���ο� 20%
        POISON0,            ///< ��(���� ������) 3�ʵ��� 1�ʸ��� ��üHP�� 5%(10% �߻�)
        POISON1,            ///< ��(���� ������) 3�ʵ��� 1�ʸ��� ��üHP�� 5%(20% �߻�)
        POISON2,            ///< ��(���� ������) 3�ʵ��� 1�ʸ��� ��üHP�� 10%(30% �߻�)
        BURN0,             ///< ȭ��(���� ������) 2�ʰ� ���ݷ� 10%(100% �߻�)
        BURN1,             ///< ȭ��(���� ������) 3�ʰ� ���ݷ� 20%(100% �߻�)
        BURN2,             ///< ȭ��(���� ������) 4�ʰ� ���ݷ� 35%(100% �߻�)
        STUN0,              ///< 1�� ���� 5%
        STUN1,              ///< 1�� ���� 7%
        STUN2,              ///< 1�� ���� 10%
        CRITICAL0,          ///< 2�� ������ 10%
        CRITICAL1,          ///< 2�� ������ 15%
        CRITICAL2,          ///< 3�� ������ 12%
        KNOCKBACK0,         ///< �ڷ� 1 �з����� 10%
        KNOCKBACK1,         ///< �ڷ� 1 �з����� 12%
        KNOCKBACK2,         ///< �ڷ� 1 �з����� 15%

        // Building
        NONE2 = 1000,
        ATTACK_UP0,         ///< ���ݷ� ���� 5%
        ATTACK_UP1,         ///< ���ݷ� ���� 10%
        ATTACK_UP2,         ///< ���ݷ� ���� 12%
        ATTACK_UP3,         ///< ���ݷ� ���� 15%
        ATTACK_UP4,         ///< ���ݷ� ���� 20%
        ATTACKSPEED_UP0,    ///< ���ݼӵ� ���� 10%
        ATTACKSPEED_UP1,    ///< ���ݼӵ� ���� 15%
        ATTACKSPEED_UP2,    ///< ���ݼӵ� ���� 20%
        ATTACKSPEED_UP3,    ///< ���ݼӵ� ���� 30%
        ATTACKSPEED_UP4,    ///< ���ݼӵ� ���� 40%
        RANGE_UP0,          ///< �����Ÿ� ���� 10%
        RANGE_UP1,          ///< �����Ÿ� ���� 20%
        RANGE_UP2,          ///< �����Ÿ� ���� 30%
        RANGE_UP3,          ///< �����Ÿ� ���� 50%
        RANGE_UP4,          ///< �����Ÿ� ���� 100%
        STEAL0,             ///< ������ 1%Ȯ���� ���� ������ 10% ���
        STEAL1,             ///< ������ 5%Ȯ���� ���� ������ 10% ���
        STEAL2,             ///< ������ 10%Ȯ���� ���� ������ 15% ���
        STEAL3,             ///< ������ 15%Ȯ���� ���� ������ 20% ���
        INCAPACITATE,       ///< ����ȭ
        REDUCING,           ///< ����
        DIE,                ///< ���
        INJURY,             ///< �̼� 99% ����
        MAX,
    };

    public enum WeaponType
    { 
        NONE = 0,   // �߻�ü����(��������)
        // Sprite
        ARROW,              // ȭ��
        BULLET,             // �Ѿ�
        MISSILE,            // �̻���
        BOMB,               // ��ź
        BEE,                // ��
        AXE,                // ����
        DANGGER,            // �ܰ�
        SHURIKEN,           // ������
        FIREBALL,           // ���̾
        ICEBALL,            // ���̽���
        BONE,               // ��
        BONETWINS,          // �ֻ�
        // Paticle
        MAGIC,              // - ���� ��з�
        CHERRY,             // ü��
        LEAF,               // ������
        SNOW,               // ��
        SNOWFLAKE,          // ������
        WATER,              // ��
        BUBBLE,             // ����
        WIND,               // �ٶ�
        LIGHT,              // ��
        LIGHTNING,          // ����
        LASER,              // ������
        STONE,              // ��
        DEBRIS,             // �μ�����
        GRAVITY,            // �߷���
        MAGNETIC,           // �ڱ���
        POISON,             // ��
        SULFURICACID,       // Ȳ��
        STAR,               // ��
        CONSTELLATION,      // ���ڸ�
        ASTRAPHE,           // �ƽ�Ʈ����
        SMALLMETEOR,        // ���� ���׿�
        METEOR,             // ���׿�
        AURA,               // �ƿ��
        PULSE,              // �޽�
        STICKY,             // ��������
        BLOOD,              // ��
        HEART,              // ��Ʈ
        FIREWORKS,          // ����
        PURPLESPARKS,       // ���������ũ
        REDSPARKS,          // ��������ũ
        CUTTER,             // Ŀ��
        CROSSBOW,           // ����
        RIFLE,              // ������
        HEAT,               // ����������
        SPEAR,              // ���Ǿ�
        SHOCKWAVE,          // ������
        FORESTARROW,        // �� ȭ��
        WINDARROW,          // �ٶ� ȭ��
        LIGHTNINGARROW,     // ���� ȭ��
        SPEARARROW,         // ���Ǿ� ȭ��
        CUBE,               // ť��
        BIGAXE,             // ū ����
        ENEMYBALL,          // �� �Ѿ�
        ENEMYSTONE,         // �� ��
        ENEMYSHOCKWAVE,     // �� ��ũ���̺�
        ENEMYMELEE,         // �� ��������
        MAX,
    }

    public enum MagicType
    {
        NONE = 0,       // ��������
        ONETIME_DOWN,   // �Ѿ�
        BOUNCE,         // ƨƨ
        MAX,
    }

    public enum ProjectileType
    {
        NONE = 0,   ///< ��������
        SHOT,       ///< �⺻(1��)
        MULTISHOT,  ///< ��Ƽ��(2~5��)
        SPLASH,     ///< ���÷���(100%, 50%)
        DOT,        ///< ������
        PENETRATE,  ///< ����
        MAGIC,      ///< ����(�ش���ġ�� ������ ��������)
        BOUNCES,    ///< �ٿ(2~5��)
        BREATH,     ///< �극��
        BOMB,       ///< ��ź(�浹�� ����)
        ADDSHOT,    ///< ���弦(2~5��)
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

    // ����Ÿ�Ժ� �⺻ �߰� ����
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