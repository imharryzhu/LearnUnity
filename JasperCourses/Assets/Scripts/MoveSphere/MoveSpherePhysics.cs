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
    float maxAcceleration = 10f;

    [SerializeField, Range(0f, 100f), Tooltip("在空中的最大加速度")]
    float maxAirAcceleration = 1f;

    [SerializeField, Range(0f, 10f), Tooltip("跳跃高度")]
    float jumpHeight = 2f;

    [SerializeField, Range(0, 10), Tooltip("在空中跳跃的最大次数")]
    int maxAirJumps = 2;

    [SerializeField, Range(0, 90), Tooltip("判断是否水平与地面的容错角度")]
    float maxGroundAngle = 25f;

    // 实际速度
    Vector3 velocity, desiredVelocity;

    // 刚体
    Rigidbody rigidBody;

    bool desiredJump;

    // 是否在地面上，无法在update中判断球是否落地，所以用碰撞检测来判断
    bool isOnGround;

    // 记录当前有几次跳跃
    int jumpCount;

    float minGroundDotProduct;

    // 由于原来跳都是向上跳跃（简单的给vec3.y赋值)。现在根据碰撞点的法线数据跳跃
    Vector3 concatNormal;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        OnValidate();
    }

    // 调用条件： 脚本加载时、Inspector界面数值修改时
    void OnValidate()
    {
        // 将允许误差角度转换为法线的y值（0-1）
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
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
        AdjustVelocity();

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
        {
            jumpCount = 0;
        }
        else
        {
            // 当不在地面上时，采用垂直跳跃
            concatNormal = Vector3.up;
        }
    }

    void Jump()
    {
        if (isOnGround || jumpCount < maxAirJumps)
        {
            jumpCount++;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
            float alignedSpeed = Vector3.Dot(velocity, concatNormal);
            if (alignedSpeed > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0);
            }

            // 简单粗暴的跳跃只会一直向上
            // velocity.y += jumpSpeed;

            velocity += concatNormal * jumpSpeed;
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
            Vector3 b = transform.TransformPoint(contactPoint.normal * 2);
            Debug.DrawLine(a, b, Color.green);

            // 接近与垂直，代表与地面接触
            if (normal.y >= minGroundDotProduct)
            {
                isOnGround = true;
                concatNormal = normal;
            }
        }
    }

    Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - concatNormal * Vector3.Dot(vector, concatNormal);
    }

    void AdjustVelocity()
    {
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

        float currentX = Vector3.Dot(velocity, xAxis);
        float currentZ = Vector3.Dot(velocity, zAxis);

        float acceleration = isOnGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;

        float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

        velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
    }
}
