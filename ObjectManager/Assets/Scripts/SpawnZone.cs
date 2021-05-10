using UnityEngine;
using System.Collections;

public abstract class SpawnZone : PersistableObject
{
    
    [System.Serializable]
    public struct SpawnConfiguration
    {
        public enum SpawnMovementDirection
        {
            Forward,
            Upward,
            Outward, // 从中心点向出生点移动
            Random
        }

        public SpawnMovementDirection spawnMovementDirection;
        public FloatRange spawnSpeed;
        public FloatRange angularSpeed;
        public FloatRange scale;
        public ColorRangeHSV color;
        public bool uniformColor; // 子形状是否使用统一颜色
    }

    [SerializeField, Tooltip("仅表面随机点")]
    protected bool surfaceOnly;

    [SerializeField, Tooltip("物体移动配置")]
    SpawnConfiguration spawnConfig;


    /// <summary>
    /// 生成物体位置的向量
    /// </summary>
    public abstract Vector3 SpawnPoint { get; }

    public virtual void ConfigureSpawn(Shape shape)
    {
        Transform t = shape.transform;

        // 在一个球体的空间内的随机一个点
        t.localPosition = SpawnPoint;
        t.localRotation = Random.rotation;
        t.localScale = spawnConfig.scale.RandomValueInRange * Vector3.one;
        if (spawnConfig.uniformColor)
        {
            shape.SetColor(spawnConfig.color.RandomInRange);
        }
        else
        {
            for (int i = 0; i < shape.ColorCount; i++)
            {
                shape.SetColor(spawnConfig.color.RandomInRange, i);
            }
        }
        

        shape.AngularVelocity = Random.onUnitSphere * spawnConfig.angularSpeed.RandomValueInRange;

        Vector3 dir;
        switch (spawnConfig.spawnMovementDirection)
        {
            case SpawnConfiguration.SpawnMovementDirection.Upward:
                dir = transform.up;
                break;
            case SpawnConfiguration.SpawnMovementDirection.Outward:
                dir = (t.localPosition - transform.position).normalized;
                break;
            case SpawnConfiguration.SpawnMovementDirection.Random:
                dir = Random.onUnitSphere;
                break;
            default:
                dir = transform.forward;
                break;
        }
        shape.Velocity = dir * spawnConfig.spawnSpeed.RandomValueInRange;
    }
}
