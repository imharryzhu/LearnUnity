using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Game : MonoBehaviour
{
    // prefab
    public Transform prefab;

    // 存储当前场景物体的列表
    List<Transform> objects;

    // 存储路径
    string savePath;


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
        objects = new List<Transform>();
        savePath = Path.Combine(Application.persistentDataPath, "saveFile");
        Debug.Log("存储路径：" + savePath);
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
            Save();
        }
        else if (Input.GetKeyDown(loadKey))
        {
            Load();
        }
    }

    void CreateObject()
    {
        Transform t = Instantiate(prefab);

        // 在一个球体的空间内的随机一个点
        t.localPosition = Random.insideUnitSphere * 5f;
        t.localRotation = Random.rotation;
        t.localScale = Random.Range(0.1f, 1f) * Vector3.one;

        objects.Add(t);
    }

    void BeginNewGame()
    {
        foreach (var obj in objects)
        {
            Destroy(obj.gameObject);
        }
        objects.Clear();
    }

    void Save()
    {
        using(
            BinaryWriter writer =
                new BinaryWriter(File.Open(savePath, FileMode.Create))
        )
        {
            writer.Write(objects.Count);
            foreach(var obj in objects)
            {
                writer.Write(obj.localPosition.x);
                writer.Write(obj.localPosition.y);
                writer.Write(obj.localPosition.z);
            }
        }
    }

    void Load()
    {
        BeginNewGame();
        using (
            BinaryReader reader =
                new BinaryReader(File.Open(savePath, FileMode.Open))
        )
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                Vector3 p = new Vector3(
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadSingle());
                Transform t = Instantiate(prefab);
                t.localPosition = p;
                objects.Add(t);
            }
        }
    }
}
