using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class MovementShapeBehaviour : ShapeBehaviour
{
    public Vector3 Velocity { get; set; }

    public override ShapeBehaviourType BehaviourType => ShapeBehaviourType.Movement;

    public override bool GameUpdate(Shape shape)
    {
        shape.transform.localPosition += Velocity * Time.deltaTime;
        return true;
    }

    public override void Load(GameDataReader reader)
    {
        Velocity = reader.ReadVector3();
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(Velocity);
    }

    public override void Recycle()
    {
        ShapeBehaviourPool<MovementShapeBehaviour>.Reclaim(this);
    }
}
