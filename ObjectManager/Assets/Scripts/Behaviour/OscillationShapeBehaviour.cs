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

    public override void GameUpdate(Shape shape)
    {
        throw new NotImplementedException();
    }

    public override void Load(GameDataReader reader)
    {
        throw new NotImplementedException();
    }

    public override void Recycle()
    {
        throw new NotImplementedException();
    }

    public override void Save(GameDataWriter writer)
    {
        ShapeBehaviourPool<OscillationShapeBehaviour>.Reclaim(this);
    }
}
