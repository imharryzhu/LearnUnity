﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : PersistableObject
{
    // 版本号
    const int saveVersion = 8;

    // 存储当前场景物体的列表
    List<Shape> shapes;

    [SerializeField, Tooltip("存储器")]
    PersistentStorage storage;

    [SerializeField, Tooltip("创建物体快捷键")]
    KeyCode createKey = KeyCode.C;

    [SerializeField, Tooltip("保存快捷键")]
    KeyCode saveKey = KeyCode.S;

    [SerializeField, Tooltip("新建游戏快捷键")]
    KeyCode newGameKey = KeyCode.N;

    [SerializeField, Tooltip("加载物体快捷键")]
    KeyCode loadKey = KeyCode.L;

    [SerializeField, Tooltip("删除物体快捷键")]
    KeyCode destoryKey = KeyCode.X;

    [SerializeField, Tooltip("子场景数量")]
    int levelCount;

    [SerializeField, Tooltip("加载时重置随机数种子")]
    bool reseedOnLoad;

    [SerializeField, Tooltip("工厂数组")]
    ShapeFactory[] shapeFactories;
    
    // 物体创建速度
    public float CreationSpeed { get; set; }
    
    // 物体销毁速度
    public float DestructionSpeed { get; set; }

    // 创建、删除物体的速度
    float creationProgress, destructionProgress;

    // 当前随机器的状态
    Random.State mainRandomState;

    // 当前关卡ID
    int loadedLevelBuildIndex;

    void Start()
    {
        shapes = new List<Shape>();
        mainRandomState = Random.state;

        if (Application.isEditor)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(i);
                if (loadedScene.name.Contains("Level"))
                {
                    SceneManager.SetActiveScene(loadedScene);
                    loadedLevelBuildIndex = loadedScene.buildIndex;
                    return;
                }
            }
        }

        BeginNewGame();
        StartCoroutine(LoadLevel(1));
    }

    private void OnEnable()
    {
        if (shapeFactories[0].FactoryId != 0)
        {
            for (int i = 0; i < shapeFactories.Length; i++)
            {
                shapeFactories[i].FactoryId = i;
            }
        }
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
            StartCoroutine(LoadLevel(loadedLevelBuildIndex));
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
        else
        {
            for (int i = 1; i <= levelCount; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    BeginNewGame();
                    StartCoroutine(LoadLevel(i));
                    return;
                }
            }
        }

        creationProgress += Time.deltaTime * CreationSpeed;
        while (creationProgress >= 1f)
        {
            creationProgress -= 1f;
            CreateShape();
        }

        destructionProgress += Time.deltaTime * DestructionSpeed;
        while (destructionProgress >= 1f)
        {
            destructionProgress -= 1f;
            DestoryShape();
        }
    }

    void FixedUpdate()
    {
        for (int i = 0; i < shapes.Count; i++)
        {
            shapes[i].GameUpdate();
        }
    }

    void CreateShape()
    {
        Shape instance = GameLevel.CurrentLevel.SpawnShape();
        shapes.Add(instance);
    }

    void BeginNewGame()
    {
        Random.state = mainRandomState;
        int seed = Random.Range(0, int.MaxValue);
        mainRandomState = Random.state;
        Random.InitState(seed);

        foreach (var obj in shapes)
        {
            obj.Recycle();
        }
        shapes.Clear();
    }

    void DestoryShape()
    {
        if (shapes.Count > 0)
        {
            int index = Random.Range(0, shapes.Count);
            shapes[index].Recycle();
            shapes.RemoveAt(index);
        }
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(shapes.Count);
        writer.Write(Random.state);
        writer.Write(loadedLevelBuildIndex);
        GameLevel.CurrentLevel.Save(writer);
        for (int i = 0; i < shapes.Count; i++)
        {
            var obj = shapes[i];
            writer.Write(shapes[i].OriginFactory.FactoryId);
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

        StartCoroutine(LoadGame(reader));
    }

    IEnumerator LoadGame(GameDataReader reader)
    {
        int version = reader.Version;
        int count = reader.ReadInt();
        if (version >= 3)
        {
            Random.State state = reader.ReadRandomState();
            if (!reseedOnLoad)
            {
                // 保持随机器的状态
                Random.state = state;
            }
        }

        yield return LoadLevel(version <= 2 ? 1 : reader.ReadInt());
        
        if (reader.Version >=3)
        {
            GameLevel.CurrentLevel.Load(reader);
        }

        for (int i = 0; i < count; i++)
        {
            int factoryId = version >= 7 ? reader.ReadInt() : 0;
            int shapeId = reader.ReadInt();
            // materialId在版本1时，才支持
            int materialId = version > 0 ? reader.ReadInt() : 0;
            Shape obj = shapeFactories[factoryId].Get(shapeId, materialId);
            obj.Load(reader);
            shapes.Add(obj);
        }
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        this.enabled = false;
        // 由于加载场景是异步的，所以使用协程，使得下面的代码下一帧再执行
        if (loadedLevelBuildIndex > 0)
        {
            yield return SceneManager.UnloadSceneAsync(loadedLevelBuildIndex);
        }

        // 加载模式为混合，默认为单场景，相当于双击打开。
        yield return 
            SceneManager.LoadSceneAsync(levelIndex, LoadSceneMode.Additive);

        // 还需要将Level1场景设置为ActiveScene
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelIndex));
        loadedLevelBuildIndex = levelIndex;
        this.enabled = true;
    }
}
