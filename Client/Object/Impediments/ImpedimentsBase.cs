using CharacterDefines;
using OptionDefines;
using GameDefines;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ImpedimentsBase : MonoBehaviour
{
    protected IObjectPool<ImpedimentsBase> ManagedPool;
    [SerializeField] public AdventurePrefabsType m_eAdventurePrefabsType = AdventurePrefabsType.MAX;
    protected Transform m_Target = null;
    protected Map_Adventure m_MapAdventure = null;

    /*Move*/
    private Vector2 initialPosition = Vector2.zero;
    protected float startTime = 0f;
    protected float journeyFraction = 0f;
    protected float journeyLength = 0f;
    protected float m_fAdjustment = 0f;

    protected GameObject ExplosionPrefeb = null;
    protected float ImpedimentLevel = 1f;
    protected int Damage = 0;
    protected float moveSpeed = 0f;

    protected Tween tween;

    protected enum PathStepType
    {
        NONE,
        LADDERDOWN, ///< 사다리내려가기
        FLOORDOWN,  ///< 지형무시
        GOSTRAIGHT, ///< 직진
        TELEPORT,   ///< 순간이동
        END,        ///< 끝
    };

    protected struct PathInfo
    {
        public int curIndex;
        public ADVLayerType ownerADVLayerType;

        public List<(PathStepType, Vector3)> pathList;

        public PathInfo(ADVLayerType Owner)
        {
            curIndex = 0;
            ownerADVLayerType = Owner;
            pathList = new List<(PathStepType, Vector3)>();
        }
    }

    protected PathInfo m_PathInfo;
    protected bool m_bActionEnd = false;

    private void Clear()
    {

    }

    public virtual void SetManagedPool(IObjectPool<ImpedimentsBase> pool)
    {
        ManagedPool = pool;
    }

    public virtual void DestroyPool()
    {
        Clear();
        ManagedPool.Release(this);
    }

    protected virtual void OnTriggerEnter(Collider collision)
    {
        if (collision.transform != m_Target)
            return;

        if (gameObject == null)
            return;

        Player MyPlayer = GameManager.Instance.GetPlayer();
        if (MyPlayer == null)
            return;

        BuffType eBuffType = BuffType.NONE;
        SFXType eSFXType = SFXType.SFX_NONE;
        if (m_eAdventurePrefabsType == AdventurePrefabsType.BARREL)
        {
            eBuffType = BuffType.INCAPACITATE;
            eSFXType = SFXType.SFX_BARREL;
        }
        else if (m_eAdventurePrefabsType == AdventurePrefabsType.BEE)
        {
            eBuffType = BuffType.INCAPACITATE;
            eSFXType = SFXType.SFX_BEESTING;
        }
        else
        {
            eBuffType = BuffType.INJURY;
            if (m_eAdventurePrefabsType == AdventurePrefabsType.POOP0)
                eSFXType = SFXType.SFX_POOP0;
            else
                eSFXType = SFXType.SFX_POOP1;

            MyPlayer.ReduceHP(Damage);
        }
        
        MyPlayer.AddBuffActor(eBuffType, true);

        Explode(ExplosionPrefeb);
        SoundManager.Instance.PlaySfx(eSFXType);

        DestroyPool();
    }

    public virtual void SetInfo(Transform player, ADVLayerType eADVLayerType, int ilevel)
    {
        if (player == null)
        {
            Debug.Log("ImpedimentsBase set error =" + m_eAdventurePrefabsType);
        }

        m_Target = player;
        ImpedimentLevel = (float)ilevel;

        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        switch (m_eAdventurePrefabsType)
        {
            case AdventurePrefabsType.BARREL:
                m_fAdjustment = 0.4f;
                transform.position = new Vector3(transform.position.x, transform.position.y + m_fAdjustment, transform.position.z);

                ExplosionPrefeb = EffectPool.Instance.GetEffectPrefab(EffectType.EXPLOSION_METEOR);
                Damage = 30;
                moveSpeed = 2f;
                break;
            case AdventurePrefabsType.POOP0:
                moveSpeed = 5f;
                transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
                break;
            case AdventurePrefabsType.POOP1:
            case AdventurePrefabsType.POOP2:
                moveSpeed = 6f;
                break;
        };

        if (m_MapAdventure == null)
        {
            MapBase pMapBase = MapManager.Instance.GetCurrentMapInfo();
            if (pMapBase is Map_Adventure)
            {
                m_MapAdventure = pMapBase as Map_Adventure;
            }
        }

        m_PathInfo = new PathInfo(eADVLayerType);
        FindAway();
        StartCoroutine(Move());
    }

    public void ChangeTargetPlayer(Transform player)
    {
        if (player == null)
        {
            Debug.Log("ChangeTargetPlayer set error =" + m_eAdventurePrefabsType);
        }

        m_Target = player;
    }

    private void Explode(GameObject ExplosionPrefeb)
    {
        if (m_Target == null || ExplosionPrefeb == null)
            return;

        Vector3 newPosition = m_Target.position;

        EffectBase pEffectBase = EffectPool.Instance.GetEffect(newPosition, ExplosionPrefeb);
        if (pEffectBase != null)
        {
            pEffectBase.SetInfo(null);
        }
    }

    // Path
    private void FindAway()
    {
        if (m_PathInfo.curIndex < 0)
            return;

        if (m_eAdventurePrefabsType == AdventurePrefabsType.BARREL)
        {
            // 1층
            if (m_PathInfo.ownerADVLayerType == ADVLayerType.ADVLayerType_1)
            {
                //Simulation_Last();
                //return;
            }
            // 4층
            else if (m_PathInfo.ownerADVLayerType == ADVLayerType.ADVLayerType_4)
            {
                //LadderType[] LadderTypeList = new LadderType[] { LadderType.FLOOR_3_LEFT_1, LadderType.FLOOR_3_LEFT_2, LadderType.FLOOR_3_RIGHT_1, LadderType.FLOOR_3_RIGHT_2 };
                //Simulation(LadderTypeList, ADVLayerType.ADVLayerType_3);
            }
            // 3층
            else if (m_PathInfo.ownerADVLayerType == ADVLayerType.ADVLayerType_3)
            {
                //LadderType[] LadderTypeList = new LadderType[] { LadderType.FLOOR_2_LEFT, LadderType.FLOOR_2_RIGHT };
                //Simulation(LadderTypeList, ADVLayerType.ADVLayerType_2);
            }
            // 2층
            else if (m_PathInfo.ownerADVLayerType == ADVLayerType.ADVLayerType_2)
            {
                //LadderType[] LadderTypeList = new LadderType[] { LadderType.FLOOR_1 };
                //Simulation(LadderTypeList, ADVLayerType.ADVLayerType_1);
            }

            //FindAway();
            Simulation_Last();
        }
        else
        {
            Simulation_Last();
        }
    }

    private void Simulation(LadderType[] LadderTypeList, ADVLayerType eADVLayerType)
    {
        int index = Oracle.RandomDice(0, LadderTypeList.Length);

        Vector3 goalPosition = m_MapAdventure.GetLaddersPosition(LadderTypeList[index], true);
        goalPosition.y += m_fAdjustment;
        m_PathInfo.pathList.Add((PathStepType.GOSTRAIGHT, goalPosition));
        m_PathInfo.pathList.Add((PathStepType.LADDERDOWN, goalPosition));

        m_PathInfo.ownerADVLayerType = eADVLayerType;
        m_PathInfo.curIndex++;
    }

    private void Simulation_Last()
    {
        m_PathInfo.curIndex = -1;

        if (m_eAdventurePrefabsType == AdventurePrefabsType.BARREL)
        {
            //int index = Oracle.RandomDice(9, 11);

            int index = 0;
            if (m_PathInfo.ownerADVLayerType == ADVLayerType.ADVLayerType_3)
                index = 5;
            else if (m_PathInfo.ownerADVLayerType == ADVLayerType.ADVLayerType_2)
                index = 7;
            else if (m_PathInfo.ownerADVLayerType == ADVLayerType.ADVLayerType_1)
                index = 9;

            if (transform.position.x < 0)
                ++index;

            Vector3 vArrivalPosition = m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, index);
            vArrivalPosition.y += m_fAdjustment;
            m_PathInfo.pathList.Add((PathStepType.END, vArrivalPosition));
        }
        else
        {
            Vector3 vecMyPosition = transform.position;
            vecMyPosition.y = -9.1f;
            m_PathInfo.pathList.Add((PathStepType.END, vecMyPosition));
        }
    }

    private IEnumerator Move()
    {
        foreach ((PathStepType, Vector3) action in m_PathInfo.pathList)
        {
            m_bActionEnd = false;
            switch ((PathStepType)action.Item1)
            {
                case PathStepType.LADDERDOWN:
                    {
                        StartCoroutine(GoLadderDown(action.Item2));
                    }
                    break;
                case PathStepType.FLOORDOWN:
                    {
                        StartCoroutine(GoFloorDown());
                    }
                    break;
                case PathStepType.GOSTRAIGHT:
                    {
                        StartCoroutine(GoStraight(action.Item2, false));
                    }
                    break;
                case PathStepType.TELEPORT:
                    {
                        StartCoroutine(GoTeleport(action.Item2));
                    }
                    break;
                case PathStepType.END:
                    {
                        StartCoroutine(GoStraight(action.Item2, true));
                    }
                    break;
            };

            yield return new WaitUntil(() => m_bActionEnd);
        }
    }

    private IEnumerator GoLadderDown(Vector3 vecDestination)
    {
        vecDestination = new Vector3(vecDestination.x, vecDestination.y - 5f, vecDestination.z);

        journeyFraction = 0f;
        initialPosition = transform.position;
        journeyLength = Vector2.Distance(initialPosition, vecDestination);
        startTime = Time.time;

        while (journeyFraction < 1.0f)
        {
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            journeyFraction = journeyLength != 0 ? distanceCovered / journeyLength : 0;
            transform.position = Vector2.Lerp(initialPosition, vecDestination, journeyFraction);
            yield return null;
        }

        m_bActionEnd = true;
    }
    private IEnumerator GoStraight(Vector3 vecDestination, bool bEnd)
    {
        journeyFraction = 0f;
        initialPosition = transform.position;
        journeyLength = Vector2.Distance(initialPosition, vecDestination);
        startTime = Time.time;

        Vector3 vecLook = (vecDestination - transform.position).normalized;

        tween.Kill(transform);
        if (m_eAdventurePrefabsType == AdventurePrefabsType.BARREL)
        {
            tween = transform.DORotate(new Vector3(0f, 0f, (vecLook.x > 0 ? -90f : 90f) * (moveSpeed * ImpedimentLevel)), 1f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear) // 선형으로 회전
            .SetLoops(-1, LoopType.Incremental); // 무한 반복
        }

        while (journeyFraction < 1.0f)
        {
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            journeyFraction = journeyLength != 0 ? distanceCovered / journeyLength : 0;
            transform.position = Vector2.Lerp(initialPosition, vecDestination, journeyFraction);
            yield return null;
        }

        m_bActionEnd = true;

        if (bEnd)
        {
            // 삭제해야함
            DestroyPool();
            yield break;
        }
    }
    private IEnumerator GoFloorDown()
    {
        yield return null;
    }
    private IEnumerator GoTeleport(Vector3 vecDestination)
    {
        transform.position = vecDestination;
        yield return null;
        m_bActionEnd = true;
    }
}
