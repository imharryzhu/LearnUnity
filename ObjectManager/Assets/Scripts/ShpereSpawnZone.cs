using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// 圆形生成区域
/// </summary>
public class ShpereSpawnZone : SpawnZone
{
    public override Vector3 SpawnPoint
    {
        get
        {
            return transform.TransformPoint(surfaceOnly ?
                Random.onUnitSphere :
                Random.insideUnitSphere);
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
        Gizmos.DrawWireSphere(Vector3.zero, 1f);
    }
}
