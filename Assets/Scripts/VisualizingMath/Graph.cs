using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField]
    Transform pointPrefab;

    [SerializeField, Range(10, 100)]
    int resolution = 10;

    [SerializeField]
    FunctionLibrary.FunctionName functionName = default;

    /// <summary>
    /// 函数切换模式，线性、随机
    /// </summary>
    [SerializeField]
    TransitionMode transitionMode = TransitionMode.Cycle;

    /// <summary>
    /// 函数切换的时间间隔
    /// </summary>
    [SerializeField, Min(0.3f)]
    float functionDuration = 1f;

    public enum TransitionMode { Cycle, Random }

    Transform[] points;

    // 当前持续时间
    float duration;

    void Awake()
    {
        var step = 2f / resolution;
        var scale = Vector3.one * step;
        points = new Transform[resolution * resolution];
        for (int i = 0; i < points.Length; i++)
        {
            Transform point = Instantiate(pointPrefab);
            point.localScale = scale;
            point.SetParent(this.transform, false);
            points[i] = point;
        }
    }

    void Update()
    {
        duration += Time.deltaTime;
        if (duration >= functionDuration)
        {
            duration -= functionDuration;
            PickNextFunction();
        }
        UpdateFunction();
    }

    void UpdateFunction()
    {
        FunctionLibrary.Function func = FunctionLibrary.GetFunction(functionName);
        float t = Time.time;
        float step = 2f / resolution;
        float v = .5f * step - 1f;
        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            if (resolution == x)
            {
                x = 0;
                z += 1;
                v = (z + .5f) * step - 1f;
            }
            float u = (x + .5f) * step - 1f;
            points[i].localPosition = func(u, v, t);
        }
    }

    void PickNextFunction()
    {
        functionName = transitionMode == TransitionMode.Cycle ?
                FunctionLibrary.GetNextFunctionName(functionName) :
                FunctionLibrary.GetRandomFunctionNameOtherThan(functionName);
    }
}
