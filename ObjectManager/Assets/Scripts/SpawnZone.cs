using UnityEngine;
using System.Collections;

public abstract class SpawnZone : PersistableObject
{
    [SerializeField, Tooltip("仅表面随机点")]
    protected bool surfaceOnly;

    /// <summary>
    /// 生成物体位置的向量
    /// </summary>
    public abstract Vector3 SpawnPoint { get; }
}
