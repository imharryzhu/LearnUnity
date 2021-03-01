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
        public Vector3 direction;
        public Quaternion rotation;
        public Transform transform;
    }

    // 存储part的二维数组
    FractalPart[][] parts;

    void Awake()
    {
        parts = new FractalPart[depth][];
        for(int i = 0, len = 1; i < depth; ++i, len *= 5)
        {
            parts[i] = new FractalPart[len];
        }

        /*       长度
         *  深度0  1
         *  深度1  5
         *  深度2  25
         *  深度3  125
         */

        float scale = 1;
        // 创建根节点
        parts[0][0] = CreatePart(0, 0, scale);
        // 创建所有子节点
        for(int li = 1; li < depth; li++)
        {
            // 每下一层，缩放比例就减小一半
            scale *= 0.5f;
            // 拿到那层的数组
            var levelParts = parts[li];
            for (int fpi = 0; fpi < levelParts.Length; fpi += 5)
            {
                for(int ci = 0; ci < 5; ci++)
                {
                    levelParts[fpi + ci] = CreatePart(li, ci, scale);
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
        rootPart.transform.localRotation = rootPart.rotation;
        parts[0][0] = rootPart;

        for (int li = 1; li < depth; li++)
        {
            var parentParts = parts[li - 1];
            var levelParts = parts[li];
            for (int fpi = 0; fpi < levelParts.Length; fpi++)
            {
                var parentTransform = parentParts[fpi / 5].transform;
                var part = levelParts[fpi];
                part.rotation *= deltaRotation;
                part.transform.localPosition =
                    parentTransform.localPosition +
                    parentTransform.localRotation *
                        (1.5f * part.transform.localScale.x * part.direction);
                // 四元数的乘法顺序：parent-child
                part.transform.localRotation =
                    parentTransform.localRotation * part.rotation;
                levelParts[fpi] = part;
            }
        }
    }

    FractalPart CreatePart(int levelIndex, int childIndex, float scale)
    {
        var go = new GameObject("Fractal Part L" + levelIndex + " C" + childIndex);
        go.transform.SetParent(this.transform, false);
        go.transform.localScale = Vector3.one * scale;
        go.AddComponent<MeshFilter>().mesh = mesh;
        go.AddComponent<MeshRenderer>().material = material;

        return new FractalPart {
            direction = directions[childIndex],
            rotation = rotations[childIndex],
            transform = go.transform
        };
    }
}
