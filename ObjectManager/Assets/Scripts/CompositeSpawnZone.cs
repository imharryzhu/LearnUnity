using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositeSpawnZone : SpawnZone
{
    [SerializeField]
    SpawnZone[] spawns;

    [SerializeField, Tooltip("顺序在索引区域中的位置")]
    bool sequential;

    private int nextSequentialIndex;

    public override Vector3 SpawnPoint
    {
        get
        {
            int index;
            if (sequential)
            {
                index = nextSequentialIndex++;
                nextSequentialIndex %= spawns.Length;
            }
            else
            {
                index = Random.Range(0, spawns.Length);
            }
            return spawns[index].SpawnPoint;
        }
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(nextSequentialIndex);
    }

    public override void Load(GameDataReader reader)
    {
        nextSequentialIndex = reader.ReadInt();
    }
}
