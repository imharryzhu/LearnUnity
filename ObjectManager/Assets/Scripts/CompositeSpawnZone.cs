using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositeSpawnZone : SpawnZone
{
    [SerializeField]
    SpawnZone[] spawns;

    public override Vector3 SpawnPoint
    {
        get
        {
            int index = Random.Range(0, spawns.Length);
            return spawns[index].SpawnPoint;
        }
    }
}
