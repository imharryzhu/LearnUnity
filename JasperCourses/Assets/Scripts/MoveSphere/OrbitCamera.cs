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

    [SerializeField, Min(0f), Tooltip("物体移动超过该值时相机才会调整")]
    float focusRadius = 1f;

    [SerializeField, Range(1f, 360f), Tooltip("相机旋转速度")]
    float rotationSpeed = 90f;

    [SerializeField, Range(-89f, 89f), Tooltip("相机垂直旋转的角度限制")]
    float minVerticalAngle = -30f, maxVerticalAngle = 60f;

    // 相机跟随物体移动的位置插值计算变量
    float focusCentering = 0.1f;

    // 当前相机焦点位置
    Vector3 focusPoint;

    // 相机的角度，x定义垂直方向，y定义水平方向
    Vector2 orbitAngles = new Vector2(45f, 0f);

    void Awake()
    {
        focusPoint = focus.position;
        transform.localRotation = Quaternion.Euler(orbitAngles);
    }

    void OnValidate()
    {
        if (maxVerticalAngle < minVerticalAngle)
        {
            maxVerticalAngle = minVerticalAngle;
        }
    }

    /*
     * 更新相机位置一般都放在LateUpdate中
     * 
     * LateUpdate在所有Update执行完在执行
     * 避免Update任务过重也避免闪帧的问题
     */
    void LateUpdate()
    {
        // 目标世界坐标
        UpdateFocusPoint();

        // 更新相机旋转角度
        Quaternion loockRotation = transform.localRotation;
        if (ManualRotation())
        {
            ConstrainAngles();
            loockRotation = Quaternion.Euler(orbitAngles);
        }

        // 相机看向的方向
        Vector3 lookDirection = loockRotation * Vector3.forward;

        // 将相机放在正确的位置
        transform.position = focusPoint - lookDirection * distance;
        transform.rotation = loockRotation;
    }

    void UpdateFocusPoint()
    {
        // 物体的实际位置
        Vector3 targetPoint = focus.position;
        if (focusRadius > 0)
        {
            float distance = Vector3.Distance(targetPoint, focusPoint);
            float t = 1;
            if(distance > 0.01f && focusCentering > 0f)
            {
                t = Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime);
            }
            if (distance > focusRadius)
            {
                t = Mathf.Min(t, focusRadius / distance);
                
            }
            // 线性插值，平滑移动相机到焦点位置
            focusPoint = Vector3.Lerp(
                targetPoint, focusPoint, t);

            // 不明白为什么不直接
            // Vector3.Lerp(focusPoint, targetPoint, Time.unscaledDeltaTime);
        }
        else
        {
            // 将物体位置直接赋值给相机焦点位置
            focusPoint = targetPoint;
        }
    }

    bool ManualRotation()
    {
        // 获取自定义的按键输入
        Vector2 input = new Vector2(
            Input.GetAxis("Vertical Camera"),
            Input.GetAxis("Horizontal Camera")
        );

        const float e = 0.001f;
        if (Mathf.Abs(input.x) > e || Mathf.Abs(input.y) > e)
        {
            orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input;
            return true;
        }
        return false;
    }

    void ConstrainAngles()
    {
        orbitAngles.x =
            Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);

        if (orbitAngles.y < 0)
        {
            orbitAngles.y += 360;
        }
        else if (orbitAngles.y >= 360)
        {
            orbitAngles.y -= 360;
        }
    }
}
