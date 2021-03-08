using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 
 * PhysX解决速度过快导致碰撞穿透的问题：
 *      修改Rigidbody的Collision Detection来防止
 *      
 * Rigidbody.Constraints，可以锁定物体的位置和角度
 * 
 * 当FixedUpdate被调用时，Time.deltaTime等于Time.fixedDeltaTime
 * 
 */

public class MoveSpherePhysics : MonoBehaviour
{
    [SerializeField, Range(1f, 100f), Tooltip("每秒最大速度")]
    float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f), Tooltip("每秒最大加速度")]
    float maxAcceleration = 10f, maxAirAcceleration = 1f;

    [SerializeField, Range(0f, 10f), Tooltip("跳跃高度")]
    float jumpHeight = 2f;

    [SerializeField, Range(0, 10), Tooltip("在空中跳跃的最大次数")]
    int maxAirJumps = 2;

    // 实际速度
    Vector3 velocity, desiredVelocity;

    // 刚体
    Rigidbody rigidBody;

    bool desiredJump;

    // 是否在地面上，无法在update中判断球是否落地，所以用碰撞检测来判断
    bool isOnGround;

    // 记录当前有几次跳跃
    int jumpCount;

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

        // 理论速度：计算得出玩家摇杆对应的速度
        desiredVelocity = maxSpeed * new Vector3(playerInput.x, 0f, playerInput.y);

        // 玩家是否按下了跳跃键
        desiredJump |= Input.GetButtonDown("Jump");
    }

    void FixedUpdate()
    {
        UpdateState();
        // 这帧最大的加速度
        float acceleration = isOnGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;

        // 将值 current 向 target 靠近
        velocity.x = Mathf.MoveTowards(velocity.x, velocity.x + desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, velocity.z + desiredVelocity.z, maxSpeedChange);

        if (desiredJump)
        {
            desiredJump = false;
            Jump();
        }

        rigidBody.velocity = velocity;

        // 先调用FixedUpdate再会处理碰撞，所以先置为false
        isOnGround = false;
    }

    void UpdateState()
    {
        velocity = rigidBody.velocity;
        if (isOnGround)
            jumpCount = 0;
    }

    void Jump()
    {
        if (isOnGround || jumpCount < maxAirJumps)
        {
            jumpCount++;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);

            // 减去之前的y分量，防止连续跳跃导致高度不一致的问题
            if (velocity.y > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0);
            }

            velocity.y += jumpSpeed;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        EvaluteCollision(collision);
    }

    // 只要碰撞一直存在
    void OnCollisionStay(Collision collision)
    {
        EvaluteCollision(collision);
    }

    void EvaluteCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint contactPoint = collision.GetContact(i);
            // 获取接触点的法线
            Vector3 normal = contactPoint.normal;
            
            Vector3 a = transform.position;
            Vector3 b = transform.TransformPoint(contactPoint.normal);
            Debug.DrawLine(a, b, Color.green);

            // 接近与垂直，代表与地面接触
            isOnGround |= normal.y >= 0.9f;
        }
    }
}
