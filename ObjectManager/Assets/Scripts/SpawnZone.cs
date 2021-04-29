using UnityEngine;
using System.Collections;

public class SpawnZone : MonoBehaviour
{

    [SerializeField, Tooltip("仅表面随机点")]
    bool surfaceOnly;

    /// <summary>
    /// 生成物体位置的向量
    /// </summary>
    public Vector3 SpawnPoint
    {
        get
        {
            return transform.TransformPoint(surfaceOnly ? 
                Random.onUnitSphere :
                Random.insideUnitSphere);
        }
    }
}
