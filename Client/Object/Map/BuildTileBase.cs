using GameDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTileBase : MonoBehaviour
{
    private Dictionary<Vector2, Building> BuildTileInfo;

    private void Awake()
    {
        BuildTileInfo = new Dictionary<Vector2, Building>();
    }

    public bool CheckTile(Vector3 vKey, out Building outBuildingInfo)
    {
        outBuildingInfo = null;
        if (BuildTileInfo.ContainsKey(vKey))
        {
            outBuildingInfo = BuildTileInfo[vKey];
            return outBuildingInfo == null ? false : true;
        }
        
        return false;
    }

    public void SetTile(Vector3 vKey, Building tempBuildingInfo)
    {
        if (BuildTileInfo.ContainsKey(vKey))
        {
            BuildTileInfo[vKey] = tempBuildingInfo;
        }
        else
        {
            BuildTileInfo.Add(vKey, tempBuildingInfo);
        }
    }

    public void ClearTile(Vector3 vKey)
    {
        if (BuildTileInfo.ContainsKey(vKey) == false)
        {
            Debug.Log("TIle error - why nothing" + vKey);
            return;
        }

        BuildTileInfo[vKey] = null;
    }

    public List<Vector2> GetBuildTileHaveBuilding()
    {
        List<Vector2> list = new List<Vector2>();

        if (BuildTileInfo == null)
            return null;

        foreach (var data in BuildTileInfo)
        {
            if (data.Value != null)
            {
                Vector2 position = data.Key;
                position -= new Vector2(0.5f, 0f);
                list.Add(position);
            }
        }

        return list;
    }
}
