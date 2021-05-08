using UnityEngine;
using System.Collections;

public class GameLevel : PersistableObject
{
    [SerializeField, Tooltip("空间生成器")]
    SpawnZone spawnZone;

    [SerializeField]
    PersistableObject[] persistableObjects;

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
        if (persistableObjects == null)
        {
            persistableObjects = new PersistableObject[0];
        }
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(persistableObjects.Length);
        for (int i = 0; i < persistableObjects.Length; i++)
        {
            persistableObjects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int saveCount = reader.ReadInt();
        for (int i = 0; i < saveCount; i++)
        {
            persistableObjects[i].Load(reader);
        }
    }

    /// <summary>
    /// 配置物体的各种数据信息
    /// </summary>
    /// <param name="shape"></param>
    public void ConfigureSpawn(Shape shape)
    {
        spawnZone.ConfigureSpawn(shape);
    }
}
