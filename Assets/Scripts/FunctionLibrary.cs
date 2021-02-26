using UnityEngine;
// 相当于Import了Mathf中的所有常量和静态成员
using static UnityEngine.Mathf;

public static class FunctionLibrary
{
    public delegate Vector3 Function(float u, float v, float t);

    static Function[] funcs = { Wave, MutilWave, Ripple, Sphere, Torus };

    public enum FunctionName { Wave, MutilWave, Ripple, Sphere, Torus };

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

    public static Vector3 Sphere(float u, float v, float t)
    {
        float r = .9f + .1f * Sin(PI * (6f * u + 4f * v + t));
        float s = r * Cos(.5f * PI * v);
        Vector3 vec;
        vec.x = s * Sin(PI * u);
        vec.y = r * Sin(PI * .5f * v);
        vec.z = s * Cos(PI * u);
        return vec;
    }

    public static Vector3 Torus(float u, float v, float t)
    {
        float r1 = .7f + .1f * Sin(PI * (6f * u  + .5f * t));
        float r2 = .15f + .05f * Sin(PI * (8f * u + 4f * v + 2f * t));
        float s = r1 + r2 * Cos(PI * v);
        Vector3 vec;
        vec.x = s * Sin(PI * u);
        vec.y = r2 * Sin(PI * v);
        vec.z = s * Cos(PI * u);
        return vec;
    }
}
