using CharacterDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSphere : MonoBehaviour
{
    [SerializeField] public SpawnSphereType eSpawnSphereType;
    [SerializeField] public int Cost;

    public int SpawnIndex { get; set; } = -1;
}
