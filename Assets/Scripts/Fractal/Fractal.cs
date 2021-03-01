using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractal : MonoBehaviour
{
    // 分形的最大深度
    [SerializeField, Range(1, 8)]
    int depth = 4;

    // 使用Start而非Awake，因为Start在创建下一帧执行
    void Start()
    {
        // name属性设置该物体名称
        name = "Fractal" + depth;

        if (depth <= 1)
        {
            return;
        }

        var childA = CreateChild(Vector3.up, Quaternion.identity);
        var childB = CreateChild(Vector3.right, Quaternion.Euler(0, 0, -90));
        var childC = CreateChild(Vector3.left, Quaternion.Euler(0, 0, 90));
        var childD = CreateChild(Vector3.forward, Quaternion.Euler(90, 0, 0));
        var childE = CreateChild(Vector3.back, Quaternion.Euler(-90, 0, 0));
        
        childA.transform.SetParent(transform, false);
        childB.transform.SetParent(transform, false);
        childC.transform.SetParent(transform, false);
        childD.transform.SetParent(transform, false);
        childE.transform.SetParent(transform, false);
    }

    void Update()
    {
        transform.Rotate(0, 22.5f * Time.deltaTime, 0);    
    }

    Fractal CreateChild(Vector3 direction, Quaternion rotation)
    {
        Fractal child = Instantiate(this);
        child.depth -= 1;
        child.transform.localPosition = 0.75f * direction;
        child.transform.localScale = 0.5f * Vector3.one;
        child.transform.localRotation = rotation;
        return child;
    }
}
