﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ShapeInstance
{
    public Shape Shape { get; private set; }
    int instanceId;

    public ShapeInstance(Shape shape)
    {
        Shape = shape;
        instanceId = shape.InstanceId;
    }

    public bool IsVaild
    {
        get
        {
            return Shape && instanceId == Shape.InstanceId;
        }
    }

    public static implicit operator ShapeInstance(Shape shape)
    {
        return new ShapeInstance(shape);
    }
}

public class Shape : PersistableObject
{
    /// <summary>
    /// 形状
    /// </summary>
    int shapeId = int.MinValue;

    public int ShapeId
    {
        get {
            return shapeId;
        }

        set
        {
            if (shapeId == int.MinValue && value != int.MinValue)
            {
                shapeId = value;
            }
            else
            {
                Debug.LogError("不能够修改shapeId!");
            }
        }
    }

    /// <summary>
    /// 材质
    /// </summary>
    public int MaterialId { get; set; }

    public void SetMaterial(Material material, int materialId)
    {
        // 设置材质
        foreach (var renderer in meshRenderers)
        {
            renderer.material = material;
        }
        // 赋值当前材质id
        MaterialId = materialId;
    }

    public ShapeFactory OriginFactory
    {
        get
        {
            return originFactory;
        }
        set
        {
            if (originFactory == null)
            {
                originFactory = value;
            }
            else
            {
                Debug.LogError("不允许动态修改工厂！");
            }
        }
    }

    private ShapeFactory originFactory;

    /// <summary>
    /// 颜色
    /// </summary>
    private Color color;
    private static int colorPropertyId = Shader.PropertyToID("_Color");
    private static MaterialPropertyBlock materialPropertyBlock;
    public void SetColor(Color color)
    {
        // WARN：设置材质颜色，会导致创建一个新的材质。
        //meshRenderer.material.color = color;

        // 所以这么设置颜色
        if (materialPropertyBlock == null)
        {
            materialPropertyBlock = new MaterialPropertyBlock();
        }
        materialPropertyBlock.SetColor(colorPropertyId, color);
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].SetPropertyBlock(materialPropertyBlock);
            colors[i] = color;
        }
    }

    public void SetColor(Color color, int index)
    {
        // 所以这么设置颜色
        if (materialPropertyBlock == null)
        {
            materialPropertyBlock = new MaterialPropertyBlock();
        }
        materialPropertyBlock.SetColor(colorPropertyId, color);
        meshRenderers[index].SetPropertyBlock(materialPropertyBlock);
        colors[index] = color;
    }

    /// <summary>
    /// 记录自己经过的时间 
    /// </summary>
    public float Age { get; private set; }

    public int InstanceId { get; private set; }

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        writer.Write(colors.Length);
        for (int i = 0; i < colors.Length; i++)
        {
            writer.Write(colors[i]);
        }
        writer.Write(behaviours.Count);
        for (int i = 0; i < behaviours.Count; i++)
        {
            writer.Write((int)behaviours[i].BehaviourType);
            behaviours[i].Save(writer);
        }
        writer.Write(Age);
    }

    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        if (reader.Version >= 6)
        {
            LoadColors(reader);
        }
        else
        {
            // 颜色在version2才支持
            SetColor(reader.Version > 1 ? reader.ReadColor() : Color.white);
        }
        if (reader.Version >= 8)
        {
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                var type = ((ShapeBehaviourType)reader.ReadInt());
                var behaviour = type.GetInstance();
                type.Print();
                behaviours.Add(behaviour);
                behaviour.Load(reader);
            }
        }
        else if (reader.Version >= 4)
        {
            AddBehaviour<RotationShapeBehaviour>().AngularVelocity = reader.ReadVector3();
            AddBehaviour<MovementShapeBehaviour>().Velocity = reader.ReadVector3();
        }

        if (reader.Version >= 9)
        {
            Age = reader.ReadFloat();
        }
    }

    [SerializeField]
    MeshRenderer[] meshRenderers;

    Color[] colors;

    void Awake()
    {
        if (meshRenderers.Length == 0)
        {
            meshRenderers = new MeshRenderer[] {
                GetComponent<MeshRenderer>()
            };
        }

        colors = new Color[meshRenderers.Length];
    }

    public void GameUpdate()
    {
        Age += Time.deltaTime;
        for (int i = 0; i < behaviours.Count; i++)
        {
            if (!behaviours[i].GameUpdate(this))
            {
                behaviours[i].Recycle();
                behaviours.RemoveAt(i--);
            }
        }
    }

    public int ColorCount
    {
        get
        {
            return colors.Length;
        }
    }

    private void LoadColors(GameDataReader reader)
    {
        int count = reader.ReadInt();
        int max = count <= colors.Length ? count : colors.Length;
        int i = 0;
        for (; i < max; i++)
        {
            SetColor(reader.ReadColor(), i);
        }
        if (count > colors.Length)
        {
            for (; i < count; i++)
            {
                reader.ReadColor();
            }
        }
        else if(count < colors.Length)
        {
            for (; i < colors.Length; i++)
            {
                SetColor(Color.white, i);
            }
        }
    }

    public int SaveIndex { get; set; }

    public void Recycle()
    {
        Age = 0f;
        InstanceId++;
        // 回收的时候移除所有的行为
        foreach (var behaviour in behaviours)
        {
            behaviour.Recycle();
        }
        behaviours.Clear();
        OriginFactory.Reclaim(this);
    }

    /// <summary>
    /// 对象行为列表
    /// </summary>
    List<ShapeBehaviour> behaviours = new List<ShapeBehaviour>();

    /// <summary>
    /// 泛型强制约束传入的类型，new()声明了T必须有构造函数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T AddBehaviour<T>() where T : ShapeBehaviour, new()
    {
        T behaviour = ShapeBehaviourPool<T>.Get();
        behaviours.Add(behaviour);
        return behaviour;
    }
}
