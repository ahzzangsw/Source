using CharacterDefines;
using GameDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OptionDefines;

public class BossAdventure_Last_Manager : Singleton<BossAdventure_Last_Manager>
{
    [SerializeField] public Sprite[] SlimeSpriteList;
    public bool m_bDie { get; private set; } = false;

    private BossAdventure_Last OwnerBoss = null;
    private List<AdventurePrefabsType> ActivateBossPrefabsList;
    private List<List<Vector3>> ActivateBossPositionList;
    private BossAdventure_Last_Skill[] m_ObjectpoolList;

    private bool m_bInit = false;
    private Player m_MyPlayer = null;
    private Map_Adventure m_Map_Adventure = null;
    private Player_Adventure m_Target = null;
    private StageInfo_Adv m_StageInfo;

    private AdventurePrefabsType m_ePickPrefab = AdventurePrefabsType.MAX;
    private float m_fGimmickSpeed = 1f;
    private int m_iLevel = -1;

    protected override void Awake()
    {
        m_ObjectpoolList = new BossAdventure_Last_Skill[10];

        ActivateBossPrefabsList = new List<AdventurePrefabsType>
        {
            AdventurePrefabsType.GREENTROLL,
            AdventurePrefabsType.GREEENOGRE,
            AdventurePrefabsType.GREENREX,
            AdventurePrefabsType.GREENMEGAPUMPKIN,
            AdventurePrefabsType.BROWNWEREWOLF,
        };

        ActivateBossPositionList = new List<List<Vector3>>()
        {
            /*트롤*/    new List<Vector3> { new Vector3(-16f, -9f, 0f), new Vector3(16f, -9f, 0f), new Vector3(-16f, 6f, 0f), new Vector3(16f, 6f, 0f) },
            /*오거*/    new List<Vector3> { new Vector3(0f, 6f, 0f)},
            /*렉스*/    new List<Vector3> { new Vector3(-20f, 11f, 0f), new Vector3(20f, 11f, 0f), new Vector3(-20f, -12f, 0f), new Vector3(20f, -12f, 0f) },
            /*호박*/    new List<Vector3> { new Vector3(0f, -9f, 0f), new Vector3(0f, 6f, 0f) },
            /*울프*/    new List<Vector3> { Vector3.zero },    //돼지 위치
        };

        m_bInit = false;
        m_bDie = false;
    }

    protected override void OnDestroy()
    {
        for (int i = 0; i < m_ObjectpoolList.Length; ++i)
        {
            Destroy(m_ObjectpoolList[i]);
            m_ObjectpoolList[i] = null;
        }
        ActivateBossPrefabsList.Clear();

        base.OnDestroy();
    }

    void Update()
    {
        if (!m_bInit)
            return;
    }

    // 생성후 연출 및 시작
    public void SetLastBossInfo_Adv(StageInfo_Adv stageInfo)
    {
        if (m_Map_Adventure == null)
        {
            MapBase pMapBase = MapManager.Instance.GetCurrentMapInfo();
            if (pMapBase is Map_Adventure == false)
                return;

            m_Map_Adventure = pMapBase as Map_Adventure;
            if (m_Map_Adventure == null)
                return;
        }

        if (m_MyPlayer == null)
        {
            m_MyPlayer = GameManager.Instance.GetPlayer();
            if (m_MyPlayer == null)
                return;
        }

        m_StageInfo = stageInfo;
        GameManager.Instance.SaveLastBossInfo();
        m_MyPlayer.isStopAction = true;
        StartCoroutine(Production());
    }

    public void ChangeTarget()
    {
        if (m_bInit == false)
            return;

        for (int i = 0; i < m_ObjectpoolList.Length; ++i)
        {
            if (m_ObjectpoolList[i] == null)
                continue;

            BossAdventure_Last_Skill Boss = m_ObjectpoolList[i];
            Boss.SetTarget(m_MyPlayer.GetAdventureMainPlayer(), m_iLevel, m_ePickPrefab);
        }

        SoundManager.Instance.PlayBGM(BGMType.LASTBOSS);
    }

    private IEnumerator Production()
    {
        yield return new WaitForSeconds(1f);

        GameObject refBossPrefabs = ResourceAgent.Instance.GetAdvBossPrefab(AdventurePrefabsType.LASTBOSS);
        if (refBossPrefabs == null)
        {
            Debug.Log("BossPrefabs is null Laststage");
            yield break;
        }

        GameObject newLastBossObject = Instantiate(refBossPrefabs);
        if (newLastBossObject == null)
        {
            Debug.Log("BossPrefabs is not Instantiate");
            yield break;
        }

        OwnerBoss = newLastBossObject.GetComponent<BossAdventure_Last>();
        if (OwnerBoss != null)
        {
            Vector3 pos = new Vector3(0f, -13f, 0f);
            OwnerBoss.SetComponent(OwnerBoss);
            OwnerBoss.SetInfo_Adv(0, m_StageInfo.Hp, m_StageInfo.Defense, m_StageInfo.moveSpeed, m_StageInfo.Range, m_StageInfo.Attack, pos);
            OwnerBoss.m_eSpeciesType = SpeciesType.MAX;
            OwnerBoss.m_eAttributeType = AttributeType.NONE;
            OwnerBoss.SetFirstLayerFloor(ADVLayerType.ADVLayerType_None);
            OwnerBoss.gameObject.SetActive(true);
            MonsterPool.Instance.AddMonsterInPool(newLastBossObject);
        }
    }

    public void Ready()
    {
        m_bInit = true;
        m_MyPlayer.isStopAction = false;

        ChangeTarget();
        ChangeLevel(0);
        StartCoroutine(CallBossBattle());
    }

    public void ChangeLevel(int iLevel)
    {
        switch(iLevel)
        {
            case 0:
                m_fGimmickSpeed = 7f;
                break;
            case 1:
                m_fGimmickSpeed = 5f;
                break;
            case 2:
                m_fGimmickSpeed = 4f;
                break;
            case 3:
                m_fGimmickSpeed = 2f;
                break;
            case 4:
                iLevel = 3;
                m_fGimmickSpeed = 1f;
                break;
        };

        if (m_iLevel != iLevel)
            m_iLevel = iLevel;
    }

    private IEnumerator CallBossBattle()
    {
        while (true)
        {
            if (!m_bInit)
                yield break;

            Player_Adventure PlayerAdventure = m_MyPlayer.GetAdventureMainPlayer();
            if (PlayerAdventure == null)
                continue;

            int iLevelIndex;
            if (m_iLevel <= 1)
                iLevelIndex = 0;
            else if (m_iLevel == 2)
                iLevelIndex = 1;
            else
                iLevelIndex = 2;

            //Test
            int RandomSkillIndex = Oracle.RandomDice(0, ActivateBossPrefabsList.Count);
            int SkillIndex = (int)(ActivateBossPrefabsList[RandomSkillIndex]);
            m_ePickPrefab = (AdventurePrefabsType)(SkillIndex + iLevelIndex);

            // 해당 기믹 실행
            GameObject refBossPrefabs = ResourceAgent.Instance.GetAdvBossPrefab(m_ePickPrefab);
            if (refBossPrefabs == null)
            {
                Debug.Log("BossPrefabs is null BossBattle Skill");
                yield break;
            }

            Vector3 StartPos = Vector3.zero;
            if (m_ePickPrefab == AdventurePrefabsType.BROWNWEREWOLF
                || m_ePickPrefab == AdventurePrefabsType.REDWEREVOLF
                || m_ePickPrefab == AdventurePrefabsType.BLACKWEREWOLF)
            {
                List<GameObject> pMonsterList = MonsterPool.Instance.GetMonsters();
                if (pMonsterList == null || pMonsterList.Count == 0)
                    yield break;

                if (pMonsterList[0] == null)
                    yield break;

                StartPos = pMonsterList[0].transform.position;
            }
            else
            {
                int RandomSkillPosition = Oracle.RandomDice(0, ActivateBossPositionList[RandomSkillIndex].Count);
                StartPos = ActivateBossPositionList[RandomSkillIndex][RandomSkillPosition];
            }

            GameObject newLastBossObject = Instantiate(refBossPrefabs, StartPos, Quaternion.identity);
            if (newLastBossObject == null)
            {
                Debug.Log("BossBattle Skill is not Instantiate");
                yield break;
            }

            BossAdventure_Last_Skill BossSkill = newLastBossObject.GetComponent<BossAdventure_Last_Skill>();
            if (BossSkill)
            {
                BossSkill.SetComponent(BossSkill);
                BossSkill.SetTarget(PlayerAdventure, m_iLevel, m_ePickPrefab);
                BossSkill.Turn(StartPos.x > 0 ? -1 : 1);

                bool bFull = true;
                for (int i = 0; i < m_ObjectpoolList.Length; ++i)
                {
                    if (m_ObjectpoolList[i] == null)
                    {
                        m_ObjectpoolList[i] = BossSkill;
                        bFull = false;
                        break;
                    }
                }

                if (bFull)
                {
                    Debug.Log("BossAdventure_Last_Manager CallBossBattle = Full");
                }
            }

            yield return new WaitForSeconds(m_fGimmickSpeed);
        }
    }

    public void OnDie()
    {
        m_bInit = false;
        m_bDie = true;
        StopAllCoroutines();

        DestoryObjectpoolList(null);
    }

    public void DestoryObjectpoolList(BossAdventure_Last_Skill ownerBoss)
    {
        if (ownerBoss == null)
        {
            for (int i = 0; i < m_ObjectpoolList.Length; ++i)
            {
                m_ObjectpoolList[i] = null;
            }
        }
        else
        {
            for (int i = 0; i < m_ObjectpoolList.Length; ++i)
            {
                if (m_ObjectpoolList[i] == null)
                    continue;

                if (ownerBoss != m_ObjectpoolList[i])
                    continue;

                m_ObjectpoolList[i] = null;
                break;
            }
        }
    }
}