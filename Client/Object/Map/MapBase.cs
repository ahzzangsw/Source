using GameDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBase : MonoBehaviour
{
    [SerializeField] private bool bCameraControl = true;
    [SerializeField] public Vector2 StartCameraPosition;
    [SerializeField] public Transform spawnPoint;
    [SerializeField] public Transform spawnPoint1;
    [SerializeField] public Transform[] wayPoint;
    [SerializeField] public Rect limitCameraRect;
    [SerializeField] public Transform[] spawnPointList;

    protected virtual void Awake()
    {
        if (bCameraControl)
        {
            if (CameraManager.Instance)
            {
                CameraManager.Instance.SetCameraRect(limitCameraRect);
                CameraManager.Instance.StartCameraPosition(StartCameraPosition.x, StartCameraPosition.y);
            }
        }
    }

    public virtual int GetWayPointCount()
    {
        return wayPoint.Length;
    }
    public virtual Transform GetWayPointByTransform(int index)
    {
        if (index < 0 && index >= wayPoint.Length)
            return null;

        return wayPoint[index];
    }
    public virtual Vector2 GetWayPointByVector2(int index)
    {
        if (index < 0 && index >= wayPoint.Length)
            return new Vector2(0, 0);

        Transform transform = wayPoint[index];
        return (Vector2)transform.position;
    }
}
