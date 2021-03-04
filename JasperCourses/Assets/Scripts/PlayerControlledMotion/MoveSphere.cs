using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSphere : MonoBehaviour
{
    [SerializeField, Range(1f, 100f), Tooltip("每秒最大速度")]
    float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f), Tooltip("每秒最大加速度")]
    float maxAcceleration = 10f;

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
        // 实际速度小于理论速度，就需要加速度
        if (velocity.x < desiredVelocity.x)
        {
            velocity.x = Mathf.Min(velocity.x + maxSpeedChange, desiredVelocity.x);
        }
        // 实际速度大于理论速度，就只能采用理论速度了
        else if (velocity.x > desiredVelocity.x)
        {
            velocity.x = Mathf.Max(velocity.x - maxSpeedChange, desiredVelocity.x);
        }

        if (velocity.z < desiredVelocity.z)
        {
            velocity.z = Mathf.Min(velocity.z + maxSpeedChange, desiredVelocity.z);
        }
        else if (velocity.y > desiredVelocity.z)
        {
            velocity.z = Mathf.Max(velocity.z - maxSpeedChange, desiredVelocity.z);
        }
        transform.localPosition += velocity * Time.deltaTime; ;
    }
}
