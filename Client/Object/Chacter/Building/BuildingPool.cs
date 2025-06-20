using CharacterDefines;
using GameDefines;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingPool : Singleton<BuildingPool>
{
    [SerializeField] public byte maxCount = 200;

    [Header("GAME2")]
    [SerializeField] private GameObject[] SpawnSphereList;
    [SerializeField] private int game2DeckMaxCount = 0;

    private Tilemap BuildTileInfo;
    private List<GameObject> poolsList;
    private List<SpeciesType> deckSpeciesTypeList;

    protected override void Awake()
    {
        poolsList = new List<GameObject>();
        for (int i = 0; i < maxCount; ++i)
        {
            poolsList.Add(null);
        }

        deckSpeciesTypeList = new List<SpeciesType>();
    }

    public void SetTileInfo()
    {
        if (BuildTileInfo == null)
        {
            GameObject pTileObject = GameObject.FindWithTag("BuildTile");
            BuildTileInfo = pTileObject.GetComponent<Tilemap>();
        }
    }

    public GameObject GetSpawnObject(bool Init)
    {
        SpawnSphereType eSpawnSphereType = SpawnSphereType.SpawnSphere_0;
        if (!Init)
        {
            if (Oracle.PercentSuccess(1))
            {
                eSpawnSphereType = SpawnSphereType.SpawnSphere_2;
            }
            else if (Oracle.PercentSuccess(10))
            {
                eSpawnSphereType = SpawnSphereType.SpawnSphere_1;
            }
        }

        return SpawnSphereList[(int)eSpawnSphereType];
    }

    public void SpawnBuilding(SpeciesType eKey, int iKey, Vector3 mouseWorldPos)
    {
        Vector3Int cellPosition = BuildTileInfo.WorldToCell(mouseWorldPos);
        TileBase clickedTile = BuildTileInfo.GetTile(cellPosition);
        if (clickedTile == null)
            return;

        Vector3 SpawnPosition = cellPosition;
        SpawnPosition += new Vector3(0.5f, 0f, -1f);

        BuildTileBase buildTileBase = BuildTileInfo.GetComponent<BuildTileBase>();
        if (buildTileBase == null)
            return;

        Building outBuildingInfo = null;
        if (buildTileBase.CheckTile(SpawnPosition, out outBuildingInfo))
        {
            return;
        }

        int selectedindex = -1;
        for (int i = 0; i < maxCount; ++i)
        {
            if (poolsList[i] == null)
            {
                selectedindex = i;
                break;
            }
        }

        if (selectedindex < 0)
        {
            Debug.Log("don't Spawn because Building Max or poolMax");
            return;
        }

        GameObject prefab = ResourceAgent.Instance.GetPrefab(eKey, iKey);
        if (prefab == null)
            return;

        poolsList[selectedindex] = Instantiate(prefab, SpawnPosition, Quaternion.identity);
        if (poolsList[selectedindex] != null)
        {
            Character tempCharacterInfo = poolsList[selectedindex].GetComponent<Character>();
            if (tempCharacterInfo != null)
            {
                Building tempBuildingInfo = poolsList[selectedindex].AddComponent<Building>();
                tempBuildingInfo.SetComponent(tempCharacterInfo);
                tempBuildingInfo.SetInfo(SpawnPosition, selectedindex, tempCharacterInfo.m_eSpeciesType, iKey);
                Destroy(tempCharacterInfo);

                buildTileBase.SetTile(SpawnPosition, tempBuildingInfo);

                Player MyPlayer = GameManager.Instance.GetPlayer();
                if (MyPlayer)
                {
                    MyPlayer.AddMoney(-tempBuildingInfo.Cost);
                    MyPlayer.OffPlayerUI();
                }

                UnlockManager.Instance.CheckBowMasters(eKey, iKey);
                //return tempBuildingInfo;  생성시 바로 UI정보나 사정거리를 보여주려면 주석제거하자
            }
        }
    }

    public void SpawnSpawn(SpawnSphere pSpawnSphere)
    {
        if (pSpawnSphere == null)
        {
            Debug.Log("don't Spawn because SpawnSphere is null");
            return;
        }

        int selectedindex = -1;
        for (int i = 0; i < maxCount; ++i)
        {
            if (poolsList[i] == null)
            {
                selectedindex = i;
                break;
            }
        }

        if (selectedindex < 0)
        {
            Debug.Log("don't Spawn because Building Max or poolMax");
            return;
        }

        int iKey = 0;
        SpawnSphereType eSpawnSphereType = pSpawnSphere.eSpawnSphereType;
        if (eSpawnSphereType == SpawnSphereType.SpawnSphere_1)
        {
            iKey = 1;
        }
        if (eSpawnSphereType == SpawnSphereType.SpawnSphere_2)
        {
            iKey = 2;
        }

        SpeciesType eKey = GetDeckSpeciesType();

        GameObject prefab = ResourceAgent.Instance.GetPrefab(eKey, iKey);
        if (prefab == null)
            return;

        Vector3 SpawnPosition = pSpawnSphere.transform.position;
        SpawnPosition += new Vector3(0f, -0.5f, -1f);

        poolsList[selectedindex] = Instantiate(prefab, SpawnPosition, Quaternion.identity);
        if (poolsList[selectedindex] != null)
        {
            Character tempCharacterInfo = poolsList[selectedindex].GetComponent<Character>();
            if (tempCharacterInfo != null)
            {
                Building tempBuildingInfo = poolsList[selectedindex].AddComponent<Building>();
                tempBuildingInfo.SetComponent(tempCharacterInfo);
                tempBuildingInfo.SetInfo(SpawnPosition, selectedindex, tempCharacterInfo.m_eSpeciesType, iKey);
                tempBuildingInfo.SpawnIndex = pSpawnSphere.SpawnIndex;
                Destroy(tempCharacterInfo);

                Player MyPlayer = GameManager.Instance.GetPlayer();
                if (MyPlayer)
                {
                    MyPlayer.AddMoney(-pSpawnSphere.Cost);
                    MyPlayer.isUIMouseOver = false;
                }

                UnlockManager.Instance.CheckBowMasters(eKey, iKey);

                // 스폰너 지우기
                pSpawnSphere.gameObject.SetActive(false);
                Destroy(pSpawnSphere.gameObject);
            }
        }

        CheckUnitAchievement();
    }

    public List<GameObject> GetBuildingList()
    {
        List<GameObject> buildings = new List<GameObject>();
        for (int i = 0; i < poolsList.Count; ++i)
        {
            GameObject targetObject = poolsList[i];
            if (targetObject == null)
                continue;

            if (targetObject.gameObject.activeSelf == false)
                continue;

            buildings.Add(targetObject);
        }
        return buildings;
    }

    public bool RemoveBuilding(Building RemoveBuilding, bool bOnlyBuilding)
    {
        if (RemoveBuilding == null)
            return false;

        if (Oracle.m_eGameType == MapType.BUILD)
        {
            if (BuildTileInfo == null)
                return false;

            BuildTileBase buildTileBase = BuildTileInfo.GetComponent<BuildTileBase>();
            if (buildTileBase)
            {
                buildTileBase.ClearTile(RemoveBuilding.m_SpawnPosition);
            }
        }
        else if (Oracle.m_eGameType == MapType.SPAWN)
        {
            if (!bOnlyBuilding)
                ReCreateSpawnSphere(RemoveBuilding.SpawnIndex);
        }

        RemoveBuilding.gameObject.SetActive(false);
        DestroyImmediate(RemoveBuilding.gameObject);
        poolsList[RemoveBuilding.m_ID] = null;
        return true;
    }

    public void DeckComposition(List<SpeciesType> SpeciesTypeList)
    {
        if (SpeciesTypeList.Count != game2DeckMaxCount)
            return;

        deckSpeciesTypeList.Clear();
        for (int i = 0; i < game2DeckMaxCount; ++i)
        {
            SpeciesType eType = SpeciesTypeList[i];
            if (deckSpeciesTypeList.Contains(eType))
            {
                deckSpeciesTypeList.Clear();
                Debug.Log("Duplicate deck - comfirm");
                return;
            }

            deckSpeciesTypeList.Add(eType);
        }

        Player MyPlayer = GameManager.Instance.GetPlayer();
        if (MyPlayer != null)
            MyPlayer.InitUpgradeData();
    }

    public List<SpeciesType> GetDeckSpeciesTypeList()
    {
        return deckSpeciesTypeList;
    }

    public SpeciesType GetDeckSpeciesType()
    {
        int maxCount = deckSpeciesTypeList.Count;
        if (maxCount == 0)
            return SpeciesType.NONE;

        int index = Oracle.RandomDice(0, game2DeckMaxCount);
        return deckSpeciesTypeList[index];
    }

    public void UpgradeBuilding(Building targetBuilding)
    {
        // Remove material
        List<GameObject> allBuildinglist = GetBuildingList();
        for (int i = allBuildinglist.Count - 1; i >= 0; --i)
        {
            Building building = allBuildinglist[i].GetComponent<Building>();
            if (building == null)
                continue;

            if (building == targetBuilding)
                continue;

            if (targetBuilding.m_eSpeciesType == building.m_eSpeciesType && targetBuilding.m_CharacterIndex == building.m_CharacterIndex)
            {
                RemoveBuilding(building, false);
                break;
            }
        }

        // Change target
        SpeciesType eKey = GetDeckSpeciesType();
        int iKey = targetBuilding.m_CharacterIndex + 1;

        GameObject prefab = ResourceAgent.Instance.GetPrefab(eKey, iKey);
        if (prefab == null)
            return;

        Vector3 SpawnPosition = targetBuilding.transform.position;

        int selectedindex = -1;
        for (int i = 0; i < maxCount; ++i)
        {
            if (poolsList[i] == null)
            {
                selectedindex = i;
                break;
            }
        }

        poolsList[selectedindex] = Instantiate(prefab, SpawnPosition, Quaternion.identity);
        if (poolsList[selectedindex] == null)
            return;

        Character tempCharacterInfo = poolsList[selectedindex].GetComponent<Character>();
        if (tempCharacterInfo == null)
            return;

        Building tempBuildingInfo = poolsList[selectedindex].AddComponent<Building>();
        tempBuildingInfo.SetComponent(tempCharacterInfo);
        tempBuildingInfo.SetInfo(SpawnPosition, selectedindex, tempCharacterInfo.m_eSpeciesType, iKey);
        tempBuildingInfo.SpawnIndex = targetBuilding.SpawnIndex;
        Destroy(tempCharacterInfo);

        Player MyPlayer = GameManager.Instance.GetPlayer();
        if (MyPlayer)
        {
            MyPlayer.isUIMouseOver = false;
        }

        UnlockManager.Instance.CheckBowMasters(eKey, iKey);

        RemoveBuilding(targetBuilding, true);
        CheckUnitAchievement();
    }

    public void ReCreateSpawnSphere(int index)
    {
        GameObject pSpawnSphere = GetSpawnObject(false);
        if (pSpawnSphere == null)
            return;

        MapBase currentMap = MapManager.Instance.GetCurrentMapInfo();
        if (currentMap == null)
            return;

        Transform spawnPoint = currentMap.spawnPointList[index];
        GameObject targetObject = Instantiate(pSpawnSphere, spawnPoint);
        SpawnSphere targetSpawnSphere = targetObject.GetComponent<SpawnSphere>();
        if (targetSpawnSphere)
        {
            targetSpawnSphere.SpawnIndex = index;
        }
    }

    public bool CheckUpgrade(Building checkBuilding)
    {
        if (Oracle.m_eGameType != MapType.SPAWN)
            return false;

        if (checkBuilding == null)
            return false;

        if (checkBuilding.m_CharacterIndex >= Oracle.MaxSpeciesKeyIndex)
            return false;

        List<GameObject> allBuildinglist = GetBuildingList();

        int iCount = 0;
        foreach (GameObject BuildingObject in allBuildinglist)
        {
            Building building = BuildingObject.GetComponent<Building>();
            if (building == null)
                continue;

            if (checkBuilding.m_eSpeciesType == building.m_eSpeciesType && checkBuilding.m_CharacterIndex == building.m_CharacterIndex)
            {
                ++iCount;
            }

            if (iCount > 1)
                break;
        }

        return iCount > 1;
    }

    private void CheckUnitAchievement()
    {
        Player MyPlayer = GameManager.Instance.GetPlayer();
        if (MyPlayer)
            MyPlayer.CheckUnitAchievement();
    }

    public void SetBuildingBasicDamage(SpeciesType eSpeciesType, int iGrade)
    {
        List<GameObject> allBuildinglist = GetBuildingList();
        foreach (GameObject BuildingObject in allBuildinglist)
        {
            Building building = BuildingObject.GetComponent<Building>();
            if (building == null)
                continue;

            if (building.m_eSpeciesType != eSpeciesType)
                continue;

            building.SetUpgradeGrade(iGrade);
        }
    }
}