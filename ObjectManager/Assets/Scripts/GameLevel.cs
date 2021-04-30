using UnityEngine;
using System.Collections;

public class GameLevel : PersistableObject
{
    [SerializeField, Tooltip("空间生成器")]
    SpawnZone spawnZone;

    // 当前关卡
    public static GameLevel CurrentLevel { get; private set; }

    public Vector3 SpawnPoint
    {
        get
        {
            return spawnZone.SpawnPoint;
        }
    }

    void OnEnable()
    {
        CurrentLevel = this;
    }

    public override void Save(GameDataWriter writer)
    {
    }

    public override void Load(GameDataReader reader)
    {
    }
}
