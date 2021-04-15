using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Transform prefab;
    public KeyCode createKey = KeyCode.C;


    void Update()
    {
        if (Input.GetKeyUp(createKey))
        {
            CreateObject();
        }
    }

    void CreateObject()
    {
        Transform t = Instantiate(prefab);

        // 在一个球体的空间内的随机一个点
        t.localPosition = Random.insideUnitSphere * 5f;
        t.localRotation = Random.rotation;
        t.localScale = Random.Range(0.1f, 1f) * Vector3.one;
    }
}
