using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FrameRateCounter : MonoBehaviour
{
    [SerializeField, Range(0.1f, 2f)]
    float sampleDuration = 1f;

    [SerializeField]
    DisplayMode displayMode = DisplayMode.FPS;

    TextMeshProUGUI display;

    public enum DisplayMode { FPS, MS }

    // 持续时间
    float duration = 0;
    // 最长持续时间
    float worstDuration = 0;
    // 最短持续时间
    float bestDuration = float.MaxValue;
    // 持续帧数
    float frame = 0;

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

        if (frameDuration < bestDuration)
            bestDuration = frameDuration;
        if (frameDuration > worstDuration)
            worstDuration = frameDuration;

        if (duration >= sampleDuration)
        {
            if (displayMode == DisplayMode.FPS)
            {
                display.SetText("FPS\n{0:0}\n{1:0}\n{2:0}",
                    frame / duration,
                    1 / bestDuration,
                    1 / worstDuration);
            }
            else
            {
                display.SetText("MS\n{0:1}\n{1:1}\n{2:1}",
                    duration / frame * 1000f,
                    bestDuration * 1000f,
                    worstDuration * 1000f);
            }
            
            frame = 0;
            duration = 0;
            bestDuration = float.MaxValue;
            worstDuration = 0;
        }
    }
}
