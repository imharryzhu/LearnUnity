﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        for (int i = 0; i < colors.Length; i++)
        {
            writer.Write(colors[i]);
        }
        writer.Write(AngularVelocity);
        writer.Write(Velocity);
    }

    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        if (reader.Version >= 5)
        {
            for (int i = 0; i < colors.Length; i++)
            {
                SetColor(reader.ReadColor(), i);
            }
        }
        else
        {
            // 颜色在version2才支持
            SetColor(reader.Version > 1 ? reader.ReadColor() : Color.white);
        }
        // 自旋转角度
        AngularVelocity = reader.Version >= 4 ? reader.ReadVector3() : Vector3.zero;
        Velocity = reader.Version >= 5 ? reader.ReadVector3() : Vector3.zero;
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

    /// <summary>
    /// 自旋转角度
    /// </summary>
    public Vector3 AngularVelocity { get; set; }

    /// <summary>
    /// 移动速度
    /// </summary>
    public Vector3 Velocity { get; set; }

    public void GameUpdate()
    {
        transform.Rotate(AngularVelocity * Time.deltaTime);
        transform.localPosition += Velocity * Time.deltaTime;
    }

    public int ColorCount
    {
        get
        {
            return colors.Length;
        }
    }
}
