using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FrameRateCounter : MonoBehaviour
{
    [SerializeField, Range(0.1f, 2f)]
    float sampleDuration = 1f;

    TextMeshProUGUI display;

    float duration = 0, frame = 0;

    void Awake()
    {
        display = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // Time.deltaTime 是前一帧到当前帧的时间，它受时间停止、慢放影响
        // 所以应该使用Time.unscaledDeltaTime获取实际帧间隔时间
        float frameDuration = Time.unscaledDeltaTime;
        duration += Time.unscaledDeltaTime;
        frame++;
        if (duration >= sampleDuration)
        {
            display.SetText("FPS\n{0:0}\n000\n000", frame / duration);
            frame = 0;
            duration = 0;
        }
        
    }

}
