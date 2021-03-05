using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSphere : MonoBehaviour
{
    [SerializeField, Range(1f, 100f), Tooltip("每秒最大速度")]
    float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f), Tooltip("每秒最大加速度")]
    float maxAcceleration = 10f;

    [SerializeField, Tooltip("可移动区域")]
    Rect allowedArea = new Rect(-4.5f, -.45f, 9f, 9f);

    [SerializeField, Range(0f, 1f), Tooltip("球的弹性")]
    float bounciness = 0.5f;

    // 实际速度
    Vector3 velocity;

    void Update()
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");

        // 将向量模长限制在 1
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        // 理论速度：计算得出玩家摇杆对应的速度
        Vector3 desiredVelocity = maxSpeed * new Vector3(playerInput.x, 0f, playerInput.y);
        // 这帧最大的加速度
        float maxSpeedChange = maxAcceleration * Time.deltaTime;

        // 将值 current 向 target 靠近
        velocity.x = Mathf.MoveTowards(velocity.x, velocity.x + desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, velocity.z + desiredVelocity.z, maxSpeedChange);

        Vector3 newPosition = transform.localPosition + velocity * Time.deltaTime;
        // 增加限制区域判断，仅仅判断边缘是不够的，还需要将碰壁之后的速度清掉
        //if (!allowedArea.Contains(new Vector2(newPosition.x, newPosition.z)))
        //{
        //    newPosition.x = Mathf.Clamp(newPosition.x, allowedArea.xMin, allowedArea.xMax);
        //    newPosition.z = Mathf.Clamp(newPosition.z, allowedArea.yMin, allowedArea.yMax);
        //}

        if (newPosition.x < allowedArea.xMin)
        {
            newPosition.x = allowedArea.xMin;
            velocity.x = -velocity.x * bounciness;
        }
        else if (newPosition.x > allowedArea.xMax)
        {
            newPosition.x = allowedArea.xMax;
            velocity.x = -velocity.x * bounciness;
        }

        if (newPosition.z < allowedArea.yMin)
        {
            newPosition.z = allowedArea.yMin;
            velocity.z = -velocity.z * bounciness;
        }
        else if (newPosition.z > allowedArea.yMax)
        {
            newPosition.z = allowedArea.yMax;
            velocity.z = -velocity.z * bounciness;
        }

        transform.localPosition = newPosition;
    }
}
