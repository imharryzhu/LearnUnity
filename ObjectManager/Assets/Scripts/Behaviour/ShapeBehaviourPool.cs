using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ShapeBehaviourPool<T> where T : ShapeBehaviour, new()
{
    // 空闲的列表
    static Stack<T> stack = new Stack<T>();

    public static T Get()
    {
        if (stack.Count > 0)
        {
            return stack.Pop();
        }
        return new T();
    }

    public static void Reclaim(T behaviour)
    {
        stack.Push(behaviour);
    }

}
