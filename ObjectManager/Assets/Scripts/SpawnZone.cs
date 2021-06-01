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

        // 生成工厂
        public ShapeFactory[] factories;

        public SpawnMovementDirection spawnMovementDirection;
        public FloatRange spawnSpeed;
        public FloatRange angularSpeed;
        public FloatRange scale;
        public ColorRangeHSV color;
        public bool uniformColor; // 子形状是否使用统一颜色
        public SpawnMovementDirection oscillationDirection;
        public FloatRange oscillationAmplitude;
        public FloatRange oscillationFrequency;
    }

    [SerializeField, Tooltip("仅表面随机点")]
    protected bool surfaceOnly;

    [SerializeField, Tooltip("物体移动配置")]
    SpawnConfiguration spawnConfig;


    /// <summary>
    /// 生成物体位置的向量
    /// </summary>
    public abstract Vector3 SpawnPoint { get; }

    /// <summary>
    /// 生成形状
    /// </summary>
    public virtual Shape SpawnShape()
    {
        int factoryIndex = Random.Range(0, spawnConfig.factories.Length);
        var shape = spawnConfig.factories[factoryIndex].GetRandom();

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

        float angularSpeed = spawnConfig.angularSpeed.RandomValueInRange;
        if (angularSpeed != 0f) // 判断0值，避免组件被调用，节省性能
        {
            var rotation = shape.AddBehaviour<RotationShapeBehaviour>();
            rotation.AngularVelocity = Random.onUnitSphere * angularSpeed;
        }

        float speed = spawnConfig.spawnSpeed.RandomValueInRange;
        if (speed != 0f){
            var movement = shape.AddBehaviour<MovementShapeBehaviour>();
            movement.Velocity = GetDirectionVector(spawnConfig.spawnMovementDirection, t) * speed;
        }

        SetupOscillation(shape);
        return shape;
    }

    private Vector3 GetDirectionVector(SpawnConfiguration.SpawnMovementDirection direction, Transform t)
    {
        switch(direction)
        {
            case SpawnConfiguration.SpawnMovementDirection.Upward:
                return transform.up;
            case SpawnConfiguration.SpawnMovementDirection.Outward:
                return (t.localPosition - transform.position).normalized;
            case SpawnConfiguration.SpawnMovementDirection.Random:
                return Random.onUnitSphere;
            default:
                return transform.forward;
        }
    }

    private void SetupOscillation(Shape shape)
    {
        float amplitude = spawnConfig.oscillationAmplitude.RandomValueInRange;
        float frequency = spawnConfig.oscillationFrequency.RandomValueInRange;
        if (amplitude == 0f || frequency == 0f)
        {
            return;
        }
        var oscillation = shape.AddBehaviour<OscillationShapeBehaviour>();
        oscillation.Offset = GetDirectionVector(spawnConfig.oscillationDirection, shape.transform) * amplitude;
        oscillation.Frequency = frequency;
    }

}
