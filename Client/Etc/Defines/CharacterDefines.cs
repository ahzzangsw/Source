namespace CharacterDefines
{
    public enum LAnimationState
    {
        Idle,
        Ready,
        Walking,
        Running,
        Jumping,
        Blocking,
        Crawling,
        Climbing,
        Dead,

        // Action
        Attack,
        Fire,
        Hit,

        Max
    };

    public enum HitParticleType
    {
        NONE = 0,
        BE_BITTEN,
    }

    public enum SpawnSphereType
    {
        SpawnSphere_0,
        SpawnSphere_1,
        SpawnSphere_2,
    }

    public enum ADVLayerType
    {
        ADVLayerType_None = 10,
        ADVLayerType_1,
        ADVLayerType_2,
        ADVLayerType_3,
        ADVLayerType_4,
        ADVLayerType_Max,
    }

    public enum StairType
    {
        BIG,
        LEFTTOP,
        RIGHTTOP,
        NONE,
    }

    public enum LadderType
    {
        FLOOR_1,
        FLOOR_2_LEFT,
        FLOOR_2_RIGHT,
        FLOOR_3_LEFT_1,
        FLOOR_3_LEFT_2,
        FLOOR_3_RIGHT_1,
        FLOOR_3_RIGHT_2,
        NONE,
    }

    public enum BuildingActionState 
    {
        Search,
        Attack,
        Ready,
    }
}