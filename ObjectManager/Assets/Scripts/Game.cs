using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Game : PersistableObject
{
    // prefab
    public PersistableObject prefab;

    // 存储当前场景物体的列表
    List<PersistableObject> objects;

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
        objects = new List<PersistableObject>();
    }

    void Update()
    {
        if (Input.GetKeyUp(createKey))
        {
            CreateObject();
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

    void CreateObject()
    {
        PersistableObject o = Instantiate(prefab);

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
        writer.Write(objects.Count);
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int count = reader.ReadInt();
        for (int i = 0; i < count; i++)
        {
            PersistableObject obj = Instantiate(prefab);
            obj.Load(reader);
            objects.Add(obj);
        }
    }
}
