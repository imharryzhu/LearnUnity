using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : PersistableObject
{
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

    public int MaterialId { get; set; }

    public void SetMaterial(Material material, int materialId)
    {
        // 设置材质
        GetComponent<MeshRenderer>().material = material;
        // 赋值当前材质id
        MaterialId = materialId;
    }


    private Color color;
    public void SetColor(Color color)
    {
        this.color = color;
        GetComponent<MeshRenderer>().material.color = color;
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
}
