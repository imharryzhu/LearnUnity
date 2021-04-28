using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ShapeFactory : ScriptableObject
{
    [SerializeField, Tooltip("形状列表")]
    Shape[] prefabs;

    [SerializeField, Tooltip("材质列表")]
    Material[] materials;

    [SerializeField, Tooltip("内存回收优化")]
    bool recycle;

    // 物体对象池
    List<Shape>[] pools;

    public Shape Get(int shapeId = 0, int materialId = 0)
    {
        Shape instance = null;
        if (recycle)
        {
            if (pools == null)
                CreatePools();
            var pool = pools[shapeId];
            if (pool.Count > 0)
            {
                int laststIndex = pool.Count - 1;
                instance = pool[laststIndex];
                instance.gameObject.SetActive(true);
                pool.RemoveAt(laststIndex);
            }
        }
        if (instance == null){
            instance = Instantiate(prefabs[shapeId]);
            instance.ShapeId = shapeId;
        }
        instance.SetMaterial(materials[materialId], materialId);
        return instance;
    }

    public Shape GetRandom()
    {
        return Get(
            Random.Range(0, prefabs.Length),
            Random.Range(0, materials.Length)
        );
    }

    /// <summary>
    /// 回收对象
    /// </summary>
    /// <param name="shape">要回收的对象</param>
    public void Reclaim(Shape shape)
    {
        if (recycle)
        {
            if (pools == null)
                CreatePools();
            var pool = pools[shape.ShapeId];
            pool.Add(shape);
            // 对象状态设置为未激活
            shape.gameObject.SetActive(false);
        }
        else
        {
            Destroy(shape.gameObject);
        }
    }

    /// <summary>
    /// 初始化对象池，根据形状列表数量
    /// </summary>
    void CreatePools()
    {
        pools = new List<Shape>[prefabs.Length];
        for (int i = 0; i < pools.Length; i++)
        {
            pools[i] = new List<Shape>();
        }
    }
}
