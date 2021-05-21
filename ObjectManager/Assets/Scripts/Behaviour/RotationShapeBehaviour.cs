using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class RotationShapeBehaviour : ShapeBehaviour
{
    public Vector3 AngularVelocity { get; set; }

    public override ShapeBehaviourType BehaviourType => ShapeBehaviourType.Rotaition;

    public override void GameUpdate(Shape shape)
    {
        shape.transform.Rotate(AngularVelocity * Time.deltaTime);
    }

    public override void Load(GameDataReader reader)
    {
        AngularVelocity = reader.ReadVector3();
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(AngularVelocity);
    }

    public override void Recycle()
    {
        ShapeBehaviourPool<RotationShapeBehaviour>.Reclaim(this);
    }
}
