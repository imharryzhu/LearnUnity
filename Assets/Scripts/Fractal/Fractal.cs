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

    struct FractalPart
    {
        public Vector3 direction, worldPosition;
        public Quaternion rotation, worldRotation;
    }

    // 存储part的二维数组
    FractalPart[][] parts;

    // 用于渲染的变换矩阵数组
    Matrix4x4[][] matrices;

    void Awake()
    {
        parts = new FractalPart[depth][];
        matrices = new Matrix4x4[depth][];
        for (int i = 0, len = 1; i < depth; ++i, len *= 5)
        {
            parts[i] = new FractalPart[len];
            matrices[i] = new Matrix4x4[len];
        }

        /*       长度
         *  深度0  1
         *  深度1  5
         *  深度2  25
         *  深度3  125
         */

        // 创建根节点
        var rootPart = CreatePart(0);
        parts[0][0] = rootPart;
        // 根节点的变换矩阵
        // 平移-旋转-缩放 Translation-Rotation-Scale
        var mat4 = Matrix4x4.TRS(
           rootPart.worldPosition,
           rootPart.worldRotation,
           Vector3.one
        );
        matrices[0][0] = mat4;

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

    void Update()
    {
        Quaternion deltaRotation = Quaternion.Euler(0, 22.5f * Time.deltaTime, 0);

        // 旋转根节点
        var rootPart = parts[0][0];
        rootPart.rotation *= deltaRotation;
        rootPart.worldRotation = rootPart.rotation;
        parts[0][0] = rootPart;

        float scale = 1;
        for (int li = 1; li < depth; li++)
        {
            // 每下一层，缩放比例就减小一半
            scale *= .5f;
            var parentParts = parts[li - 1];
            var levelParts = parts[li];
            var levelMat4 = matrices[li];
            for (int fpi = 0; fpi < levelParts.Length; fpi++)
            {
                var parentPart = parentParts[fpi / 5];
                var part = levelParts[fpi];
                part.rotation *= deltaRotation;
                part.worldRotation = parentPart.worldRotation * part.rotation;
                part.worldPosition = parentPart.worldPosition +
                    parentPart.worldRotation *
                    (1.5f * scale * part.direction);
                levelParts[fpi] = part;
            }
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
