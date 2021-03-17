using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 轨道摄像机
/// </summary>
public class OrbitCamera : MonoBehaviour
{
    [SerializeField, Tooltip("需要围观的物体")]
    Transform focus = default;

    [SerializeField, Range(1f, 20f), Tooltip("距离物体的围观距离")]
    float distance = 5f;

    /*
     * LateUpdate在所有Update执行完在执行
     * 避免Update任务过重也避免闪帧的问题
     * 
     */
    void LateUpdate()
    {
        // 目标世界坐标
        Vector3 focusPoint = focus.position;

        // 相机看向的方向
        Vector3 lookDir = transform.forward;

        // 将相机放在正确的位置
        transform.position = focusPoint - lookDir * distance;
    }
}
