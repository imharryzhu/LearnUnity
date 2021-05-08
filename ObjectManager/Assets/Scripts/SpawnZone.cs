using UnityEngine;
using System.Collections;

public abstract class SpawnZone : PersistableObject
{
    public enum SpawnMovementDirection
    {
        Forward,
        Upward,
        Outward, // 从中心点向出生点移动
        Random
    }

    [SerializeField, Tooltip("仅表面随机点")]
    protected bool surfaceOnly;

    [SerializeField, Tooltip("物体移动方向")]
    SpawnMovementDirection spawnMovementDirection;

    [SerializeField, Tooltip("物体移动速度配置")]
    FloatRange spawnSpeed;

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
        t.localScale = Random.Range(0.1f, 1f) * Vector3.one;
        shape.SetColor(Random.ColorHSV(
            // 色彩
            hueMin: 0f, hueMax: 1f,
            // 饱和度
            saturationMin: 0.5f, saturationMax: 1f,
            valueMin: 0.25f, valueMax: 1f,
            alphaMin: 1f, alphaMax: 1f
        ));
        shape.AngularVelocity = Random.onUnitSphere * Random.Range(0f, 90f);

        Vector3 dir;
        if (spawnMovementDirection == SpawnMovementDirection.Upward)
        {
            dir = transform.up;
        }
        else if (spawnMovementDirection == SpawnMovementDirection.Outward)
        {
            dir = (t.localPosition - transform.position).normalized;
        }
        else if(spawnMovementDirection == SpawnMovementDirection.Random)
        {
            dir = Random.onUnitSphere;
        }
        else
        {
            dir = transform.forward;
        }
        shape.Velocity = dir * spawnSpeed.RandomValueInRange;
    }
}
