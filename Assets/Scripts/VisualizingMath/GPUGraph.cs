using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUGraph : MonoBehaviour
{
    const int maxResolution = 1000;

    [SerializeField]
    ComputeShader computerShader = default;

    [SerializeField]
    Material material = default;
    
    [SerializeField]
    Mesh mesh = default;

    [SerializeField, Range(10, maxResolution)]
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

    /// <summary>
    /// 函数切换的过渡动画时间
    /// </summary>
    [SerializeField, Min(0.3f)]
    float transitionDuration = 1f;

    public enum TransitionMode { Cycle, Random }

    // 当前持续时间
    float duration;

    // 标识当前是否是过渡动画阶段
    bool transitioning;

    // 定义ComputeShader的属性
    static readonly int
        positionsId = Shader.PropertyToID("_Positions"),
        resolutionId = Shader.PropertyToID("_Resolution"),
        stepId = Shader.PropertyToID("_Step"),
        timeId = Shader.PropertyToID("_Time"),
        scaleId = Shader.PropertyToID("_Scale"),
        transitionProgressId = Shader.PropertyToID("_TransitionProgress");

    FunctionLibrary.FunctionName transitionFromFunc;

    ComputeBuffer positionsBuffer;

    void OnEnable()
    {
        positionsBuffer = new ComputeBuffer(maxResolution * maxResolution, 3 * 4);
    }

    void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
    }

    void Update()
    {
        duration += Time.deltaTime;
        if (transitioning)
        {
            if (duration >= functionDuration)
            {
                duration -= functionDuration;
                transitioning = false;
            }
        }
        else if (duration >= functionDuration)
        {
            duration -= functionDuration;
            transitioning = true;
            transitionFromFunc = functionName;
            PickNextFunction();
        }

        UpdateFunctionOnGPU();
    }

    void PickNextFunction()
    {
        functionName = transitionMode == TransitionMode.Cycle ?
                FunctionLibrary.GetNextFunctionName(functionName) :
                FunctionLibrary.GetRandomFunctionNameOtherThan(functionName);
    }

    void UpdateFunctionOnGPU() {
        float step = 2f / resolution;
        computerShader.SetInt(resolutionId, resolution);
        computerShader.SetFloat(stepId, step);
        computerShader.SetFloat(timeId, Time.time);

        if (transitioning)
        {
            computerShader.SetFloat(
                transitionProgressId,
                Mathf.SmoothStep(0f, 1f, duration / transitionDuration)
            );
        }

        var kernelIndex = (int)functionName + (int)(transitioning ? transitionFromFunc : functionName) * FunctionLibrary.FunctionsCount;
        computerShader.SetBuffer(kernelIndex, positionsId, positionsBuffer);
        int groups = Mathf.CeilToInt(resolution / 8f);
        computerShader.Dispatch(kernelIndex, groups, groups, 1);

        material.SetBuffer(positionsId, positionsBuffer);
        material.SetVector(scaleId, new Vector4(step, 1f / step));
        Bounds bounds = new Bounds(Vector3.zero, new Vector3(2f + 2f / resolution, 2f));
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, positionsBuffer.count);

    }
}
