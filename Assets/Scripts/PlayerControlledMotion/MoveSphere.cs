using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSphere : MonoBehaviour
{
    void Update()
    {
        Vector2 playerInput = new Vector2(0, 0);
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        transform.localPosition = new Vector3(playerInput.x, 0, playerInput.y);
    }
}
