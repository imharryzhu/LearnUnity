using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Game : PersistableObject
{

    // 版本号
    const int saveVersion = 0;

    // prefab
    public ShapeFactory shapeFactory;

    // 存储当前场景物体的列表
    List<Shape> objects;

    [SerializeField, Tooltip("存储器")]
    public PersistentStorage storage;

    [SerializeField, Tooltip("创建物体快捷键")]
    public KeyCode createKey = KeyCode.C;

    [SerializeField, Tooltip("保存快捷键")]
    public KeyCode saveKey = KeyCode.S;

    [SerializeField, Tooltip("新建游戏快捷键")]
    public KeyCode newGameKey = KeyCode.N;

    [SerializeField, Tooltip("加载物体快捷键")]
    public KeyCode loadKey = KeyCode.L;


    void Awake()
    {
        objects = new List<Shape>();
    }

    void Update()
    {
        if (Input.GetKeyUp(createKey))
        {
            CreateShape();
        }
        else if(Input.GetKeyUp(newGameKey))
        {
            BeginNewGame();
        }
        else if(Input.GetKeyUp(saveKey))
        {
            storage.Save(this);
        }
        else if (Input.GetKeyDown(loadKey))
        {
            BeginNewGame();
            storage.Load(this);
        }
    }

    void CreateShape()
    {
        Shape o = shapeFactory.GetRandom();

        Transform t = o.transform;

        // 在一个球体的空间内的随机一个点
        t.localPosition = Random.insideUnitSphere * 5f;
        t.localRotation = Random.rotation;
        t.localScale = Random.Range(0.1f, 1f) * Vector3.one;

        objects.Add(o);
    }

    void BeginNewGame()
    {
        foreach (var obj in objects)
        {
            Destroy(obj.gameObject);
        }
        objects.Clear();
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(saveVersion);
        writer.Write(objects.Count);
        for (int i = 0; i < objects.Count; i++)
        {
            writer.Write(objects[i].ShapeId);
            objects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int version = reader.ReadInt();
        if (version > saveVersion)
        {
            Debug.LogError("文件版本号大于程序版本号，无法解析！");
            return;
        }
        int count = reader.ReadInt();
        for (int i = 0; i < count; i++)
        {
            int shapeId = reader.ReadInt();
            Shape obj = shapeFactory.Get(shapeId);
            obj.Load(reader);
            objects.Add(obj);
        }
    }
}
