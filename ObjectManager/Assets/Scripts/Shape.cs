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
        meshRenderer.material = material;
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
        this.color = color;
        // WARN：设置材质颜色，会导致创建一个新的材质。
        //meshRenderer.material.color = color;

        // 所以这么设置颜色
        if (materialPropertyBlock == null)
        {
            materialPropertyBlock = new MaterialPropertyBlock();
        }
        materialPropertyBlock.SetColor(colorPropertyId, color);
        meshRenderer.SetPropertyBlock(materialPropertyBlock);
    }

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        writer.Write(color);
    }

    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        // 颜色在version2才支持
        SetColor(reader.Version > 1 ? reader.ReadColor() : Color.white);
    }

    MeshRenderer meshRenderer;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
}
