using UnityEngine;
// 相当于Import了Mathf中的所有常量和静态成员
using static UnityEngine.Mathf;

public static class FunctionLibrary
{
    public delegate float Function(float x, float z, float y);

    static Function[] funcs = { Wave, MutilWave, Ripple };

    public enum FunctionName { Wave, MutilWave, Ripple };

    public static Function GetFunction(FunctionName name)
    {
        return funcs[(int)name];
    }

    public static float Wave(float x, float z, float t)
    {
        return Sin(PI * (x + z + t));
    }

    public static float MutilWave(float x, float z, float t)
    {
        var y = Sin(PI * (x + .5f * t));
        // y += Sin(2f * PI * (x * t)) / 2f;
        // return y / 1.5f;
        // 由于除法运算比乘法运算消耗更多计算资源，所以最好改成乘法运算
        y += Sin(2f * PI * (z + t)) * .5f;
        y += Sin(PI * (x + z + .25f * t));
        return y * (1f / 2.5f);
    }

    public static float Ripple(float x, float z, float t)
    {
        float d = Sqrt(x * x + z * z);
        float y = Sin(PI * (4f * d - t));
        return y / (1f + 10 * d);
    }
}
