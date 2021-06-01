using System;
using UnityEngine;

public sealed class OscillationShapeBehaviour : ShapeBehaviour
{
    public override ShapeBehaviourType BehaviourType
    {
        get
        {
            return ShapeBehaviourType.Oscillation;
        }
    }
    /// <summary>
    /// 最大偏移量
    /// </summary>
    public Vector3 Offset { get; set; }

    /// <summary>
    /// 摆动频率
    /// </summary>
    public float Frequency { get; set; }

    private float previousOscillation;

    public override void GameUpdate(Shape shape)
    {
        float oscillation = Mathf.Sin(2f * Mathf.PI * Frequency * shape.Age);
        shape.transform.localPosition += 
            (oscillation - previousOscillation) * Offset;
        previousOscillation = oscillation;
    }

    public override void Recycle()
    {
        previousOscillation = 0f;
        ShapeBehaviourPool<OscillationShapeBehaviour>.Reclaim(this);
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(Offset);
        writer.Write(Frequency);
        writer.Write(previousOscillation);
    }

    public override void Load(GameDataReader reader)
    {
        Offset = reader.ReadVector3();
        Frequency = reader.ReadFloat();
        previousOscillation = reader.ReadFloat();
    }
}
