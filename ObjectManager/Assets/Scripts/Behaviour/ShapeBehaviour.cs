using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShapeBehaviour
#if UNITY_EDITOR
: ScriptableObject
#endif
{
    public abstract ShapeBehaviourType BehaviourType { get; }
    public abstract void GameUpdate(Shape shape);
    public abstract void Save(GameDataWriter writer);
    public abstract void Load(GameDataReader reader);
    public abstract void Recycle();

#if UNITY_EDITOR
    public bool IsReclaimed { get; set; }
    public void OnEnable()
    {
        if(IsReclaimed)
        {
            Recycle();
        }
    }
#endif
}

/// <summary>
/// 定义行为类别
/// </summary>
public enum ShapeBehaviourType
{
    Movement, Rotaition
}
