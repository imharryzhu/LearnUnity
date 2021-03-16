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

    [SerializeField, Range(0, 100), Tooltip("最大捕捉速度")]
    float maxSnapSpeed = 100f;

    [SerializeField, Min(0f), Tooltip("判定可捕捉的最大高度")]
    float probeDistane = 1f;

    [SerializeField]
    LayerMask probeMask = -1;

    // 实际速度
    Vector3 velocity, desiredVelocity;

    // 刚体
    Rigidbody rigidBody;

    bool desiredJump;

    // 与地面接触点的数量
    int groundContactCount;

    // 是否在地面上，无法在update中判断球是否落地，所以用碰撞检测来判断
    bool isOnGround => groundContactCount > 0;

    // 记录当前有几次跳跃
    int jumpCount;

    float minGroundDotProduct;

    // 由于原来跳都是向上跳跃（简单的给vec3.y赋值)。现在根据碰撞点的法线数据跳跃
    Vector3 concatNormal;

    // 记录球被动离开地面的物理帧次数
    int stepsSinceLastGrounded;

    // 记录主动跳跃在空中的物理帧次数
    int stepsSinceLastJump;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        OnValidate();

        Time.timeScale = 0.5f;
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

        GetComponent<Renderer>().material.color = isOnGround ? Color.black : Color.white;
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
        ClearState();
    }

    void UpdateState()
    {
        stepsSinceLastGrounded++;
        stepsSinceLastJump++;
        velocity = rigidBody.velocity;
        if (isOnGround || SnapToGround()) // 尝试拉回到地面
        {
            stepsSinceLastGrounded = 0;
            jumpCount = 0;
            // 当接触点不止一个时，这个法线值是所有法线之和，所以必须归一化
            if (groundContactCount > 1)
            {
                concatNormal.Normalize();
            }
        }
        else
        {
            // 当不在地面上时，采用垂直跳跃
            concatNormal = Vector3.up;
        }
    }

    void ClearState()
    {
        groundContactCount = 0;
        // 重置法线
        concatNormal = Vector3.zero;
    }

    // 尝试拉回到地面
    bool SnapToGround()
    {
        // 当被动离开超过一帧，或者主动跳跃后超过2帧，则判断为无法拉回
        if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2)
        {
            return false;
        }

        // 实际速度大于最大可捕捉速度，就判定为不可拉回
        float speed = velocity.magnitude;
        if (speed > maxSnapSpeed)
        {
            return false;
        }

        RaycastHit hit;
        // 先物体正下方发射线，如果没有地面，则说明不要强行拉回
        if(!Physics.Raycast(rigidBody.position, Vector3.down, out hit, probeDistane, probeMask))
        {
            return false;
        }

        // 如果射线检测到了地面
        if (hit.normal.y < minGroundDotProduct)
        {
            return false;
        }

        groundContactCount = 1;
        concatNormal = hit.normal;

        float dot = Vector3.Dot(velocity, hit.normal);
        if (dot > 0f)
        {
            velocity = (velocity - hit.normal * dot).normalized * speed;
        }
        return true;
    }

    void Jump()
    {
        if (isOnGround || jumpCount < maxAirJumps)
        {
            stepsSinceLastJump = 0;
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
                groundContactCount++;
                concatNormal += normal;
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
