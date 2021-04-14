using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField]
    Transform hour, min, sec;
    [SerializeField]
    bool continuous;

    const float degreePreHour = 30;
    const float degreePreMinute = 10;
    const float degreePreSecond = 10;

    public void Update()
    {
        if (continuous)
        {
            UpdateContinuous();
        }
        else
        {
            UpdateDiscrete();
        }
    }

    void UpdateContinuous()
    {
        TimeSpan now = DateTime.Now.TimeOfDay;
        hour.transform.localRotation = Quaternion.Euler(0f, (float)now.TotalHours * degreePreHour, 0f);
        min.transform.localRotation = Quaternion.Euler(0f, (float)now.TotalMinutes * degreePreMinute, 0f);
        sec.transform.localRotation = Quaternion.Euler(0f, (float)now.TotalSeconds * degreePreSecond, 0f);
    }

    void UpdateDiscrete()
    {
        DateTime now = DateTime.Now;
        hour.transform.localRotation = Quaternion.Euler(0f, now.Hour * degreePreHour, 0f);
        min.transform.localRotation = Quaternion.Euler(0f, now.Minute * degreePreMinute, 0f);
        sec.transform.localRotation = Quaternion.Euler(0f, now.Second * degreePreSecond, 0f);
    }

}
