using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShapeBehaviour
{
    public abstract ShapeBehaviourType BehaviourType { get; }
    public abstract void GameUpdate(Shape shape);
    public abstract void Save(GameDataWriter writer);
    public abstract void Load(GameDataReader reader);
    public abstract void Recycle();
}

/// <summary>
/// 定义行为类别
/// </summary>
public enum ShapeBehaviourType
{
    Movement, Rotaition
}
