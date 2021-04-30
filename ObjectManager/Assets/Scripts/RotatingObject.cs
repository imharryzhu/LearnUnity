using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObject : PersistableObject
{
    [SerializeField, Tooltip("旋转速度")]
    Vector3 angularVelocity;

    void Update()
    {
        transform.Rotate(angularVelocity * Time.deltaTime);
    }
}
