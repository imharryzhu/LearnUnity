using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ColorRangeHSV
{
    [FloatRangeSlider(0f, 1f)]
    public FloatRange hue, stauration, value;

    public Color RandomInRange
    {
        get
        {
            return Random.ColorHSV(
                hue.min, hue.max,
                stauration.min, stauration.max,
                value.min, value.max,
                1f, 1f);
        }
    }
}

public class FloatRangeSliderAttribute : PropertyAttribute
{
    public float Min { get; private set; }
    public float Max { get; private set; }

    public FloatRangeSliderAttribute(float min, float max)
    {
        if (max < min)
        {
            max = min;
        }
        Min = min;
        Max = max;
    }
}
