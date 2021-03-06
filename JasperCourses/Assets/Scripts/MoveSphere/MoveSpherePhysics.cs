using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * PhysX解决速度过快导致碰撞穿透的问题：
 *      修改Rigidbody的Collision Detection来防止
 *      
 * Rigidbody.Constraints，可以锁定物体的位置和角度
 * 
 */

public class MoveSpherePhysics : MonoBehaviour
{
    [SerializeField, Range(1f, 100f), Tooltip("每秒最大速度")]
    float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f), Tooltip("每秒最大加速度")]
    float maxAcceleration = 10f;

    // 实际速度
    Vector3 velocity;

    // 刚体
    Rigidbody rigidBody;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");

        // 将向量模长限制在 1
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        velocity = rigidBody.velocity;

        // 理论速度：计算得出玩家摇杆对应的速度
        Vector3 desiredVelocity = maxSpeed * new Vector3(playerInput.x, 0f, playerInput.y);
        // 这帧最大的加速度
        float maxSpeedChange = maxAcceleration * Time.deltaTime;

        // 将值 current 向 target 靠近
        velocity.x = Mathf.MoveTowards(velocity.x, velocity.x + desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, velocity.z + desiredVelocity.z, maxSpeedChange);

        rigidBody.velocity = velocity;
    }
}
