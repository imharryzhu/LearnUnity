using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField]
    Transform pointPrefab;

    [SerializeField, Range(10, 200)]
    int resolution = 10;

    [SerializeField]
    FunctionLibrary.FunctionName function = default;

    Transform[] points;

    void Awake()
    {
        var step = 2f / resolution;
        var scale = Vector3.one * step;
        var position = Vector3.zero;
        points = new Transform[resolution * resolution];
        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            if(x == resolution)
            {
                x = 0;
                z++;
            }
            Transform point = Instantiate(pointPrefab);
            position.x = (x + .5f) * step - 1f;
            position.z = (z + .5f) * step - 1f;
            position.y = position.x * position.x;
            point.localPosition = position;
            point.localScale = scale;
            point.SetParent(this.transform);
            points[i] = point;
        }
    }

    void Update()
    {
        FunctionLibrary.Function func = FunctionLibrary.GetFunction(function);
        float t = Time.time;
        for (int i = 0; i < points.Length; i++)
        {
            Transform point = points[i];
            Vector3 position = point.localPosition;
            position = func(position.x, position.z, t);
            point.localPosition = position;
        }
    }
}
