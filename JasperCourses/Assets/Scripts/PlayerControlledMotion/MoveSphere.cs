using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSphere : MonoBehaviour
{
    [SerializeField, Range(1f, 100f), Tooltip("最大移动速度")]
    float maxSpeed = 10f;

    void Update()
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");

        // 将向量模长限制在 1
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);
        Vector3 displacement = maxSpeed * Time.deltaTime * new Vector3(playerInput.x, 0f, playerInput.y);
        transform.localPosition += displacement;
    }
}
