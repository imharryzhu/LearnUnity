using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementShapeBehaviour : ShapeBehaviour
{
    public Vector3 Velocity { get; set; }

    public override void GameUpdate(Shape shape)
    {
        shape.transform.localPosition += Velocity * Time.deltaTime;
    }

    public override void Load(GameDataReader reader)
    {
        Velocity = reader.ReadVector3();
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(Velocity);
    }
}
