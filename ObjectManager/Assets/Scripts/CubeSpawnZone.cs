using UnityEngine;
using System.Collections;

public class CubeSpawnZone : SpawnZone
{
    public override Vector3 SpawnPoint
    {
        get
        {
            Vector3 p;
            p.x = Random.Range(-.5f, .5f);
            p.y = Random.Range(-.5f, .5f);
            p.z = Random.Range(-.5f, .5f);
            if (surfaceOnly)
            {
                // 随机xyz轴
                int axis = Random.Range(0, 3);
                p[axis] = p[axis] < 0f ? -.5f : .5f;
            }
            return transform.TransformPoint(p);
        }
    }

    /// <summary>
    /// OnDrawGizmos是特殊方法，仅在编辑器中生效
    /// 每次绘制Scene窗口时，都会调用该方法
    /// 显示隐藏由Scene窗口中的Gizmos按钮决定
    /// </summary>
    public void OnDrawGizmos()
    {
        // Gizmos的这些属性，是自动重置的。

        Gizmos.color = Color.cyan;
        // 应用物体的转换矩阵
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}
