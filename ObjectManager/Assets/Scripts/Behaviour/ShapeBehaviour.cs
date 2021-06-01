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
    Movement, Rotaition, Oscillation
}

/// <summary>
/// 枚举扩展方法
/// </summary>
public static class ShapeBehaviourTypeMethods
{
    public static ShapeBehaviour GetInstance(this ShapeBehaviourType type)
    {
        switch(type)
        {
            case ShapeBehaviourType.Movement:
                return ShapeBehaviourPool<MovementShapeBehaviour>.Get();
            case ShapeBehaviourType.Rotaition:
                return ShapeBehaviourPool<RotationShapeBehaviour>.Get();
            case ShapeBehaviourType.Oscillation:
                return ShapeBehaviourPool<OscillationShapeBehaviour>.Get();
        }
        Debug.Log("Forgot to support " + type);
        return null;
    }

    public static void Print(this ShapeBehaviourType type)
    {
        Debug.Log("枚举扩展方法Print: " + type.ToString());
    }
}
