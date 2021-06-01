using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ShapeBehaviourPool<T> where T : ShapeBehaviour, new()
{
    // 空闲的列表
    static Stack<T> stack = new Stack<T>();

    public static T Get()
    {
        if (stack.Count > 0)
        {
            T behaviour = stack.Pop();
#if UNITY_EDITOR
            behaviour.IsReclaimed = false;
#endif
            return behaviour;
        }
#if UNITY_EDITOR
        return ScriptableObject.CreateInstance<T>();
#else
         return new T();
#endif
    }

    public static void Reclaim(T behaviour)
    {
#if UNITY_EDITOR
        behaviour.IsReclaimed = true;
#endif
        stack.Push(behaviour);
    }

}
