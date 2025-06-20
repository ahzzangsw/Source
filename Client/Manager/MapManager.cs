using GameDefines;
using OptionDefines;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : Singleton<MapManager>
{
    [SerializeField] public GameObject[] mapPrefabs;
    [SerializeField] private GameObject[] debrisPrefabs;

    public int mapIndex { get; set; } = -1;

    private MapBase m_CurrentMap = null;

    private enum DebrisType
    {
        Stone,

    }
    private Tilemap BuildTileInfo = null;

    public void Clear()
    {
        mapIndex = -1;
    }

    public void LoadMap(MapType eMapType)
    {
        Clear();

        if (mapPrefabs.Length == 0)
        {
            Debug.Log("mapPrefabs is Null");
            return;
        }

        int index = -1;
        switch(eMapType)
        {
            case MapType.BUILD:
                index = 1;
                break;
            case MapType.SPAWN:
                index = 2;
                break;
            case MapType.ADVENTURE:
                index = 3;
                break;
        }

        if (index < 0 || mapPrefabs.Length <= index)
        {
            Debug.Log("mapPrefabs list is overflow index = " + index);
            return;
        }

        if (mapPrefabs[index] == null)
        {
            Debug.Log("mapPrefabs is Null index = " + index);
            return;
        }

        if (mapIndex == index)
            return;

        mapIndex = index;
        GameObject instantiatedMapPrefabs = Instantiate(mapPrefabs[index]);
        if (instantiatedMapPrefabs)
        {
            instantiatedMapPrefabs.SetActive(true);
            //instantiatedMapPrefabs.transform.position = new Vector3(0f, 0f, 0f);

            m_CurrentMap = instantiatedMapPrefabs.GetComponent<MapBase>();
            if (m_CurrentMap == null)
                return;

            if (eMapType == MapType.BUILD)
            {
                GameObject pTileObject = GameObject.FindWithTag("BuildTile");
                BuildTileInfo = pTileObject.GetComponent<Tilemap>();
                BuildingPool.Instance.SetTileInfo();
            }
            else if (eMapType == MapType.SPAWN)
            {
                for (int i = 0; i < m_CurrentMap.spawnPointList.Length; ++i)
                {
                    GameObject pSpawnSphere = BuildingPool.Instance.GetSpawnObject(true);
                    if (pSpawnSphere == null)
                        continue;

                    Transform spawnPoint = m_CurrentMap.spawnPointList[i];
                    SpawnSphere targetSpawnSphere = Instantiate(pSpawnSphere, spawnPoint).GetComponent<SpawnSphere>();
                    if (targetSpawnSphere)
                    {
                        targetSpawnSphere.SpawnIndex = i;
                    }
                }
            }
            else
            {

            }
        }
        else
        {
            Debug.Log("mapPrefabs is do not Instantiate index = " + index);
        }
    }

    public MapBase GetCurrentMapInfo()
    {
        if (mapPrefabs.Length == 0)
            return null;

        if (mapIndex < 0 || mapPrefabs.Length <= mapIndex)
        {
            Debug.Log("GetCurrentMapInfo mapPrefabs list is overflow index = " + mapIndex);
            return null;
        }

        if (m_CurrentMap == null)
        {
            GameObject MapObject = mapPrefabs[mapIndex];
            if (MapObject == null)
                return null;

            m_CurrentMap = MapObject.GetComponent<MapBase>();
        }

        return m_CurrentMap;
    }

    public Transform GetCurrentMapSpawn(int index)
    {
        if (m_CurrentMap == null)
            return null;

        if (index < 0 || index >= m_CurrentMap.spawnPointList.Length)
            return null;

        return m_CurrentMap.spawnPointList[index];
    }

    public void DestroyBuildTile()
    {
        if (BuildTileInfo == null)
            return;

        BuildTileBase buildTileBase = BuildTileInfo.GetComponent<BuildTileBase>();
        if (buildTileBase == null)
            return;

        List<Vector3Int> tilePositions = new List<Vector3Int>();
        BoundsInt bounds = BuildTileInfo.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                TileBase tile = BuildTileInfo.GetTile(cellPosition);
                if(tile != null)
                    tilePositions.Add(cellPosition);
            }
        }

        StartCoroutine(CallBrokeTile(tilePositions));
    }

    IEnumerator CallBrokeTile(List<Vector3Int> tilePositions)
    {
        for (int i = 0; i < tilePositions.Count; ++i)
        {
            Vector3Int tilePosition = tilePositions[i];
            BuildTileInfo.SetTile(tilePosition, null);
            tilePosition.z = -1;
            GameObject debris = Instantiate(debrisPrefabs[(int)(DebrisType.Stone)], tilePosition, Quaternion.identity);
            Destroy(debris, 1.0f);
            SoundManager.Instance.PlayBossSfx(BossState.TILEBROKEN);
            yield return new WaitForSeconds(0.08f);
        }
    }
}
