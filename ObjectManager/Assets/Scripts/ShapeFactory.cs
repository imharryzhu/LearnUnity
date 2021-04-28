using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    /**
     *  为什么要将这些放在新Scene中？
     *  当游戏物体过多时，Hierarchy窗口将会变得卡顿。
     *  解决方案就是将物体放在父结点上并折叠。
     *  由于放在普通的GameObject上，本物体的状态发生变化时，会通知父节点，影响性能。
     *  所以更好的方案是将这些物体放在新的场景上。
     */
    Scene poolScene;

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
            else
            {
                instance = Instantiate(prefabs[shapeId]);
                instance.ShapeId = shapeId;
            }
            // 放到新场景中
            SceneManager.MoveGameObjectToScene(instance.gameObject, poolScene);
        }
        else
        {
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
        // 判断是否在编辑器中
        if (Application.isEditor)
        {
            // 当编辑器在运行模式下重新编译时。ScriptableObject对象不会被保存。
            // 重新构建pool
            poolScene = SceneManager.GetSceneByName(this.name);
            if (poolScene.isLoaded)
            {
                var rootObjects = poolScene.GetRootGameObjects();
                for (int i = 0; i < rootObjects.Length; i++)
                {
                    Shape pooledShape = rootObjects[i].GetComponent<Shape>();
                    if (!pooledShape.gameObject.activeSelf)
                    {
                        pools[pooledShape.ShapeId].Add(pooledShape);
                    }
                }
                return;
            }
        }
        poolScene = SceneManager.CreateScene(this.name);
    }
}
