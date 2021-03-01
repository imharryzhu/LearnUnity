using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractal : MonoBehaviour
{
    // 分形的最大深度
    [SerializeField, Range(1, 8)]
    int depth = 4;

    void Start()
    {
        // name属性设置该物体名称
        name = "Fractal" + depth;

        if (depth <= 1)
        {
            return;
        }

        // 用Start而不用Awake，是因为Start在下一帧执行，Awake是实例化当帧执行
        Fractal child = Instantiate<Fractal>(this);
        child.depth -= 1;
    }
}
