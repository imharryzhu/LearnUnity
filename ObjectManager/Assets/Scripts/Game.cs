using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Game : PersistableObject
{

    // 版本号
    const int saveVersion = 2;

    // prefab
    public ShapeFactory shapeFactory;

    // 存储当前场景物体的列表
    List<Shape> shapes;

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

    [SerializeField, Tooltip("加载物体快捷键")]
    public KeyCode destoryKey = KeyCode.X;


    void Awake()
    {
        shapes = new List<Shape>();
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
            storage.Save(this, saveVersion);
        }
        else if (Input.GetKeyDown(loadKey))
        {
            BeginNewGame();
            storage.Load(this);
        }
        else if(Input.GetKeyDown(destoryKey))
        {
            DestoryShape();
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
        o.SetColor(Random.ColorHSV(
            // 色彩
            hueMin: 0f, hueMax: 1f,
            // 饱和度
            saturationMin: 0.5f, saturationMax: 1f,
            valueMin: 0.25f, valueMax: 1f,
            alphaMin: 1f, alphaMax: 1f
        ));
        shapes.Add(o);
    }

    void BeginNewGame()
    {
        foreach (var obj in shapes)
        {
            Destroy(obj.gameObject);
        }
        shapes.Clear();
    }

    void DestoryShape()
    {
        if (shapes.Count > 0)
        {
            int index = Random.Range(0, shapes.Count);
            Destroy(shapes[index].gameObject);
            shapes.RemoveAt(index);
        }
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(shapes.Count);
        for (int i = 0; i < shapes.Count; i++)
        {
            var obj = shapes[i];
            writer.Write(obj.ShapeId);
            writer.Write(obj.MaterialId);
            obj.Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int version = reader.Version;
        if (version > saveVersion)
        {
            Debug.LogError("文件版本号大于程序版本号，无法解析！");
            return;
        }
        int count = reader.ReadInt();
        for (int i = 0; i < count; i++)
        {
            int shapeId = reader.ReadInt();
            // materialId在版本1时，才支持
            int materialId = version > 0 ? reader.ReadInt() : 0;
            Shape obj = shapeFactory.Get(shapeId, materialId);
            obj.Load(reader);
            shapes.Add(obj);
        }
    }
}
