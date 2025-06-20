using GameDefines;
using CharacterDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;

public class ImpedimentsPool : Singleton<ImpedimentsPool>
{
    private Dictionary<AdventurePrefabsType, IObjectPool<ImpedimentsBase>> poolsList;
    private List<ImpedimentsBase> allImpediments;

    private GameObject ImpedimentsPrefab = null;
    private Vector3 ImpedimentsPrefabPosition = Vector3.zero;
    private Map_Adventure m_MapAdventure = null;

    private List<Coroutine> CoroutineList = null;
    private bool bEnabled = true;

    protected override void Awake()
    {
        allImpediments = new List<ImpedimentsBase>();
        poolsList = new Dictionary<AdventurePrefabsType, IObjectPool<ImpedimentsBase>>();
        for (int i = 0; i < (int)AdventurePrefabsType.MAX; ++i)
        {
            ObjectPool<ImpedimentsBase> pool = new ObjectPool<ImpedimentsBase>(CreateImpediments, OnGetImpediments, OnReleaseImpediments, OnDestroyImpediments, maxSize: 100);
            poolsList.Add((AdventurePrefabsType)i, pool);
        }

        CoroutineList = new List<Coroutine>();
    }

    private ImpedimentsBase CreateImpediments()
    {
        if (ImpedimentsPrefab == null)
            return null;

        GameObject ImpedimentsClone = Instantiate(ImpedimentsPrefab, ImpedimentsPrefabPosition, Quaternion.identity);
        if (ImpedimentsClone == null)
            return null;

        ImpedimentsBase ImpedimentsBase = ImpedimentsClone.GetComponent<ImpedimentsBase>();
        if (ImpedimentsBase)
            ImpedimentsBase.SetManagedPool(poolsList[ImpedimentsBase.m_eAdventurePrefabsType]);

        ImpedimentsPrefab = null;
        ImpedimentsPrefabPosition = Vector3.zero;
        return ImpedimentsBase;
    }
    private void OnGetImpediments(ImpedimentsBase Impediments)
    {
        if (ImpedimentsPrefabPosition != Vector3.zero)
            Impediments.gameObject.transform.position = ImpedimentsPrefabPosition;
        Impediments.gameObject.SetActive(true);
        ImpedimentsPrefabPosition = Vector3.zero;
    }
    private void OnReleaseImpediments(ImpedimentsBase Impediments)
    {
        allImpediments.Remove(Impediments);
        Impediments.gameObject.SetActive(false);
    }
    private void OnDestroyImpediments(ImpedimentsBase Impediments)
    {
        allImpediments.Remove(Impediments);
        Destroy(Impediments.gameObject);
    }

    public ImpedimentsBase GetImpediments(AdventurePrefabsType eImpedimentsType, Vector3 StartPosition)
    {
        if (eImpedimentsType == AdventurePrefabsType.BOSS || eImpedimentsType == AdventurePrefabsType.MAX)
            return null;

        ImpedimentsPrefab = ResourceAgent.Instance.GetAdvBossPrefab(eImpedimentsType);
        ImpedimentsPrefabPosition = StartPosition;
        return poolsList[eImpedimentsType].Get();
    }

    public void SetWaveInfo(int stageIndex)
    {
        if (Oracle.m_eGameType != MapType.ADVENTURE)
            return;

        if (stageIndex < 10 || stageIndex % 10 != 1)
            return;

        if (m_MapAdventure == null)
        {
            MapBase pMapBase = MapManager.Instance.GetCurrentMapInfo();
            if (pMapBase is Map_Adventure)
            {
                if ((m_MapAdventure = pMapBase as Map_Adventure) == null)
                    return;
            }
        }

        foreach(Coroutine curCoroutine in CoroutineList)
        {
            StopCoroutine(curCoroutine);
        }
        CoroutineList.Clear();

        int iLevel = stageIndex / 10;
        switch (stageIndex)
        {
            case 11:
                Spawn(AdventurePrefabsType.BARREL, 1);
                break;
            case 21:
                Spawn(AdventurePrefabsType.BARREL, 2);
                break;
            case 31:
                Spawn(AdventurePrefabsType.BARREL, 2);
                Spawn(AdventurePrefabsType.POOP0, 5);
                break;
            case 41:
                Spawn(AdventurePrefabsType.BARREL, 2);
                Spawn(AdventurePrefabsType.POOP0, 10);
                break;
            case 51:
                Spawn(AdventurePrefabsType.BARREL, 2);
                Spawn(AdventurePrefabsType.POOP0, 10);
                Spawn(AdventurePrefabsType.LASER, 1);
                break;
            case 61:
                Spawn(AdventurePrefabsType.BARREL, 2);
                Spawn(AdventurePrefabsType.POOP0, 10);
                Spawn(AdventurePrefabsType.LASER, 2);
                break;
            case 71:
                Spawn(AdventurePrefabsType.BARREL, 2);
                Spawn(AdventurePrefabsType.POOP0, 10);
                Spawn(AdventurePrefabsType.LASER, 2);
                Spawn(AdventurePrefabsType.BEE, 1); 
                break;
            case 81:
                Spawn(AdventurePrefabsType.BARREL, 2);
                Spawn(AdventurePrefabsType.POOP0, 10);
                Spawn(AdventurePrefabsType.LASER, 2);
                Spawn(AdventurePrefabsType.BEE, 2);
                break;
            case 91:
                Spawn(AdventurePrefabsType.BARREL, 2);
                Spawn(AdventurePrefabsType.POOP0, 10);
                Spawn(AdventurePrefabsType.LASER, 2);
                Spawn(AdventurePrefabsType.BEE, 3);
                break;
        }
    }

    private void Spawn(AdventurePrefabsType eAdventurePrefabsType, int iLevel)
    {
        if (eAdventurePrefabsType == AdventurePrefabsType.BOSS || eAdventurePrefabsType == AdventurePrefabsType.MAX)
            return;

        Vector3[] vStartPositionList;
        if (eAdventurePrefabsType == AdventurePrefabsType.BARREL)
        {
            vStartPositionList = new Vector3[]
            {
                m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, 0),
                m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, 1),
                m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, 5),
                m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, 6),
                m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, 7),
                m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, 8),
                m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, 9),
                m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, 10),
            };
        }
        else if (eAdventurePrefabsType == AdventurePrefabsType.LASER)
        {
            vStartPositionList = new Vector3[]
            {
                m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, 0),
                m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, 5),
                m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, 7),
                m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, 9)
            };
        }
        else if (eAdventurePrefabsType == AdventurePrefabsType.BEE)
        {
            vStartPositionList = new Vector3[]
            {
                m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, 2),
                m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, 3),
                m_MapAdventure.GetSpawnPointPosition(AdventureLevelType.NONE, 4)
            };
        }
        else
        {
            vStartPositionList = new Vector3[]
            {
                m_MapAdventure.GetFloorBothEnds(ADVLayerType.ADVLayerType_4, true),
                m_MapAdventure.GetFloorBothEnds(ADVLayerType.ADVLayerType_4, false)
            };
        }

        CoroutineList.Add(StartCoroutine(CallSpawnImpediment(eAdventurePrefabsType, vStartPositionList, iLevel)));
    }

    private IEnumerator CallSpawnImpediment(AdventurePrefabsType eAdventurePrefabsType, Vector3[] vecStartPositionList, int iLevel)
    {
        Player MyPlayer = GameManager.Instance.GetPlayer();
        if (MyPlayer == null)
            yield break;

        int preventDuplicationIndex = -1;
        while (true)
        {
            if (bEnabled == false)
                yield break;

            Player_Adventure PlayerAdventure = MyPlayer.GetAdventureMainPlayer();
            if (PlayerAdventure == null)
                continue;

            Vector3 StartPosition;
            ADVLayerType eADVLayerType = ADVLayerType.ADVLayerType_4;
            if (eAdventurePrefabsType == AdventurePrefabsType.BARREL)
            {
                List<int> indexList = new List<int>();
                for (int i = 0; i < vecStartPositionList.Length; ++i)
                {
                    if (preventDuplicationIndex == i)
                        continue;

                    indexList.Add(i);
                }

                int randomIndex = Oracle.RandomDice(0, indexList.Count);
                preventDuplicationIndex = indexList[randomIndex];
                if (preventDuplicationIndex < 2)        // 0, 1
                {
                    eADVLayerType = ADVLayerType.ADVLayerType_4;
                }
                else if (preventDuplicationIndex < 4)   // 5, 6
                {
                    eADVLayerType = ADVLayerType.ADVLayerType_3;
                }
                else if (preventDuplicationIndex < 6)   // 7,8
                {
                    eADVLayerType = ADVLayerType.ADVLayerType_2;
                }
                else                                    // 9, 10
                {
                    eADVLayerType = ADVLayerType.ADVLayerType_1;
                }

                //int randomindex = Oracle.RandomDice(0, vecStartPositionList.Length);
                //StartPosition = vecStartPositionList[randomindex];
                StartPosition = vecStartPositionList[preventDuplicationIndex];
            }
            else if (eAdventurePrefabsType == AdventurePrefabsType.LASER)
            {
                int randomindex = Oracle.RandomDice(0, vecStartPositionList.Length);
                StartPosition = vecStartPositionList[randomindex];
                StartPosition.x = 0f;
                StartPosition.y += 1f;
            }
            else if (eAdventurePrefabsType == AdventurePrefabsType.BEE)
            {
                List<int> indexList = new List<int>();
                for(int i = 0; i < vecStartPositionList.Length; ++i)
                {
                    indexList.Add(i);
                }

                int randomindex = Oracle.RandomDice(0, indexList.Count);
                if (iLevel > 2)
                {
                    Vector3 StartPosition_Clone = vecStartPositionList[indexList[randomindex]];
                    indexList.RemoveAt(randomindex);
                    
                    ImpedimentsBase impediments_Clone = GetImpediments(eAdventurePrefabsType, StartPosition_Clone);
                    if (impediments_Clone != null)
                    {
                        impediments_Clone.SetInfo(PlayerAdventure.transform, eADVLayerType, iLevel);
                    }
                    allImpediments.Add(impediments_Clone);
                }

                randomindex = Oracle.RandomDice(0, indexList.Count);
                StartPosition = vecStartPositionList[indexList[randomindex]];
            }
            else
            {
                StartPosition = new Vector3(Oracle.RandomDice(vecStartPositionList[0].x, vecStartPositionList[1].x), 13f, 0f);

                int PrefabIndex = Oracle.RandomDice((int)AdventurePrefabsType.POOP0, (int)AdventurePrefabsType.POOP2 + 1);
                eAdventurePrefabsType = (AdventurePrefabsType)PrefabIndex;
            }

            ImpedimentsBase impediments = GetImpediments(eAdventurePrefabsType, StartPosition);
            if (impediments != null)
            {
                if (eAdventurePrefabsType == AdventurePrefabsType.LASER)
                {
                    ImpedimentLaser laser = impediments as ImpedimentLaser;
                    if (laser != null)
                    {
                        laser.SetInfo(PlayerAdventure.transform, eADVLayerType, iLevel);
                    }
                }
                else
                {
                    impediments.SetInfo(PlayerAdventure.transform, eADVLayerType, iLevel);
                }

                allImpediments.Add(impediments);
            }

            yield return new WaitForSeconds(10f / iLevel);
        }
    }

    public void ForceSpawn(AdventurePrefabsType eAdventurePrefabsType, int iLevel, bool bDuplicate)
    {
        if (bDuplicate == false)
        {
            foreach (Coroutine curCoroutine in CoroutineList)
            {
                StopCoroutine(curCoroutine);
            }
            CoroutineList.Clear();
        }

        Spawn(eAdventurePrefabsType, iLevel);
    }

    public void OnDie()
    {
        bEnabled = false;

        foreach (Coroutine curCoroutine in CoroutineList)
        {
            StopCoroutine(curCoroutine);
        }
        CoroutineList.Clear();
    }

    public void ChangeTarget()
    {
        Player MyPlayer = GameManager.Instance.GetPlayer();
        if (MyPlayer == null)
            return;

        Player_Adventure PlayerAdventure = MyPlayer.GetAdventureMainPlayer();
        if (PlayerAdventure == null)
            return;

        for (int i = 0; i < allImpediments.Count; ++i)
        {
            ImpedimentsBase impediments = allImpediments[i];
            if (impediments == null)
                continue;

            impediments.ChangeTargetPlayer(PlayerAdventure.transform);
        }
    }
}
