using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractal : MonoBehaviour
{
    [SerializeField]
    Mesh mesh = default;

    [SerializeField]
    Material material = default;

    // 分形的最大深度
    [SerializeField, Range(1, 8)]
    int depth = 4;

    // 存储方向信息
    static Vector3[] directions =
    {
        Vector3.up, Vector3.right, Vector3.left, Vector3.forward, Vector3.back
    };

    // 存储旋转角度信息
    static Quaternion[] rotations =
    {
        Quaternion.identity,
        Quaternion.Euler(0, 0, -90),
        Quaternion.Euler(0, 0, 90),
        Quaternion.Euler(90, 0, 0),
        Quaternion.Euler(-90, 0, 0)
    };

    // 分型的基本信息
    struct FractalPart
    {
        public Vector3 direction, worldPosition;
        public Quaternion rotation, worldRotation;
        // 为了避免用四元数计算，保存欧拉角用于后续计算
        public float angle;
    }

    // 存储part的二维数组
    FractalPart[][] parts;

    // 用于渲染的变换矩阵数组
    Matrix4x4[][] matrices;

    // shader属性参数
    static readonly int matricesId = Shader.PropertyToID("_Matrices");

    ComputeBuffer[] matricesBuffers;

    // 将Awake改为OnEnable，为了在OnDisable时释放缓冲区
    void OnEnable()
    {
        parts = new FractalPart[depth][];
        matrices = new Matrix4x4[depth][];
        matricesBuffers = new ComputeBuffer[depth];
        // 4x4的矩阵有16个字节
        int stride = 16 * 4;
        for (int i = 0, len = 1; i < depth; ++i, len *= 5)
        {
            parts[i] = new FractalPart[len];
            matrices[i] = new Matrix4x4[len];
            matricesBuffers[i] = new ComputeBuffer(len, stride);
        }

        /*       长度
         *  深度0  1
         *  深度1  5
         *  深度2  25
         *  深度3  125
         */

        // 创建根节点
        parts[0][0] = CreatePart(0);

        // 创建所有子节点
        for (int li = 1; li < depth; li++)
        {
            // 拿到那层的数组
            var levelParts = parts[li];
            for (int fpi = 0; fpi < levelParts.Length; fpi += 5)
            {
                for(int ci = 0; ci < 5; ci++)
                {
                    levelParts[fpi + ci] = CreatePart(ci);
                }
            }
        }
    }

    // 需要Disable时释放缓冲区
    void OnDisable()
    {
        foreach (var buf in matricesBuffers)
        {
            buf.Release();
        }
        parts = null;
        matrices = null;
        matricesBuffers = null;
    }

    // 调用条件： 脚本加载时、Inspector界面数值修改时
    void OnValidate()
    {
        if (parts != null && enabled)
        {
            OnDisable();
            OnEnable();
        }
    }

    void Update()
    {
        // 不对四元数进行递增操作，float值的微量不精确累加会导致四元数不合法！
        // Quaternion deltaRotation = Quaternion.Euler(0, 22.5f * Time.deltaTime, 0);
        float angleDelta = 22.5f * Time.deltaTime;
        // 旋转根节点
        var rootPart = parts[0][0];
        // rootPart.rotation *= deltaRotation;
        rootPart.angle += angleDelta;
        rootPart.worldRotation = rootPart.rotation;
        parts[0][0] = rootPart;
        // 根节点的变换矩阵
        // 平移-旋转-缩放 Translation-Rotation-Scale
        var mat4 = Matrix4x4.TRS(
           rootPart.worldPosition,
           rootPart.worldRotation,
           Vector3.one
        );
        matrices[0][0] = mat4;

        float scale = 1;
        for (int li = 1; li < depth; li++)
        {
            // 每下一层，缩放比例就减小一半
            scale *= .5f;
            var parentParts = parts[li - 1];
            var levelParts = parts[li];
            var levelMat4 = matrices[li];
            Matrix4x4[] levelMatrices = matrices[li];
            for (int fpi = 0; fpi < levelParts.Length; fpi++)
            {
                var parentPart = parentParts[fpi / 5];
                var part = levelParts[fpi];
                part.rotation *= Quaternion.Euler(0, part.angle, 0); ;
                part.worldRotation = parentPart.worldRotation * part.rotation;
                part.worldPosition = parentPart.worldPosition +
                    parentPart.worldRotation *
                    (1.5f * scale * part.direction);
                levelParts[fpi] = part;

                levelMatrices[fpi] = Matrix4x4.TRS(
                    part.worldPosition,
                    part.worldRotation,
                    scale * Vector3.one
                );
            }
        }

        var bounds = new Bounds(Vector3.zero, 3 * Vector3.one);
        for (int i = 0; i < matricesBuffers.Length; i++)
        {
            var buffer = matricesBuffers[i];
            buffer.SetData(matrices[i]);
            material.SetBuffer(matricesId, buffer);
            Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, buffer.count);
        }
    }

    FractalPart CreatePart(int childIndex)
    {
        return new FractalPart {
            direction = directions[childIndex],
            rotation = rotations[childIndex]
        };
    }
}
