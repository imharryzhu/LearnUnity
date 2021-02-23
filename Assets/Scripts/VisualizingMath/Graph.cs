using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField]
    Transform pointPrefab;

    [SerializeField, Range(10, 100)]
    int resolution;

    Transform[] points;

    void Awake()
    {
        var step = 2f / resolution;
        var scale = Vector3.one * step;
        var position = Vector3.zero;
        points = new Transform[resolution];
        for (int i = 0; i < resolution; i++)
        {
            Transform point = Instantiate(pointPrefab);
            position.x = (i + .5f) * step - 1f;
            position.y = position.x * position.x;
            point.localPosition = position;
            point.localScale = scale;
            point.SetParent(this.transform);
            points[i] = point;
        }
    }

    void Update()
    {
        float t = Time.time;
        for (int i = 0; i < points.Length; i++)
        {
            Transform point = points[i];
            Vector3 position = point.localPosition;
            position.y = Mathf.Sin(Mathf.PI * (position.x + t));
            point.localPosition = position;
        }
    }
}
