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

    [SerializeField, Tooltip("调整相机位置后多少秒，相机自动回正")]
    float alignDelay = 3f;

    [SerializeField, Range(0f, 90f)]
    float alignSmoothRange = 45f;

    // 相机跟随物体移动的位置插值计算变量
    float focusCentering = 0.1f;

    // 当前相机焦点位置
    Vector3 focusPoint;

    // 记录上一次的相机焦点位置
    Vector3 previousFocusPoint;

    // 相机的角度，x定义垂直方向，y定义水平方向
    Vector2 orbitAngles = new Vector2(45f, 0f);

    // 最后一次手动旋转发生的时间
    float lastManualRotaitonTime;

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
        if (ManualRotation() || AutomaticRotation())
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
        previousFocusPoint = focusPoint;

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
            lastManualRotaitonTime = Time.unscaledTime;
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
    
    // 判断是否可以将相机拉回正常角度
    bool AutomaticRotation()
    {
        if(Time.unscaledTime - lastManualRotaitonTime < alignDelay)
        {
            return false;
        }

        Vector2 movement = new Vector2(
            focusPoint.x - previousFocusPoint.x,
            focusPoint.z - previousFocusPoint.z);

        // 仅仅是获取最小值，所以不用magnitude了，浪费性能
        float movementSqrtMagnitude = movement.sqrMagnitude;
        if (movementSqrtMagnitude < 0.000001f)
        {
            return false;
        }

        float headingAngle = GetAngle(movement / Mathf.Sqrt(movementSqrtMagnitude));
        float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(orbitAngles.y, headingAngle));
        float rotationChange = rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementSqrtMagnitude);
        if (deltaAbs < alignSmoothRange)
        {
            rotationChange *= deltaAbs / alignSmoothRange;
        } else if(180f - deltaAbs < alignSmoothRange)
        {
            rotationChange *= (180f - deltaAbs) / alignSmoothRange;
        }
        orbitAngles.y = Mathf.MoveTowardsAngle(orbitAngles.y, headingAngle, rotationChange);
        return true;
    }

    static float GetAngle(Vector2 dir)
    {
        float angle = Mathf.Acos(dir.y) * Mathf.Rad2Deg;
        return dir.x < 0f ? 360f - angle : angle;
    }
}
