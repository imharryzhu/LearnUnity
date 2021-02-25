using UnityEngine;
// 相当于Import了Mathf中的所有常量和静态成员
using static UnityEngine.Mathf;

public static class FunctionLibrary
{
    public delegate Vector3 Function(float u, float v, float t);

    static Function[] funcs = { Wave, MutilWave, Ripple };

    public enum FunctionName { Wave, MutilWave, Ripple };

    public static Function GetFunction(FunctionName name)
    {
        return funcs[(int)name];
    }

    public static Vector3 Wave(float u, float v, float t)
    {
        Vector3 vec;
        vec.x = u;
        vec.y = Sin(PI * (u + v + t));
        vec.z = v;
        return vec;
    }

    public static Vector3 MutilWave(float u, float v, float t)
    {
        Vector3 vec;
        vec.x = u;
        vec.z = v;
        var y = Sin(PI * (u + .5f * t));
        y += Sin(2f * PI * (v + t)) * .5f;
        y += Sin(PI * (u + v + .25f * t));
        y *= (1f / 2.5f);
        vec.y = y;
        return vec;
    }

    public static Vector3 Ripple(float u, float v, float t)
    {
        float d = Sqrt(u * u + v * v);
        float y = Sin(PI * (4f * d - t));
        Vector3 vec;
        vec.x = u;
        vec.z = v;
        vec.y = y / (1f + 10 * d);
        return vec;
    }
}
