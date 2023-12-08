
using TMPro;
using UnityEngine;

public class FrameRateCounter: MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI display;

    [SerializeField, Range(0.1f, 2f)]
    float sampleDuration = 1.0f;

    int frames;
    float duration, bestDuration = float.MaxValue, worstDuration;

    void Update()
    {
        float frameDuration = Time.unscaledDeltaTime;
        frames += 1;
        duration += frameDuration;

        if (frameDuration < bestDuration)
        {
            bestDuration = frameDuration;
        }
        if (frameDuration > worstDuration)
        {
            worstDuration = frameDuration;
        }

        if (duration >= sampleDuration)
        {
            display.SetText("FPS\nAvg:    {0:0}\nBest:   {1:0}\nWorst: {2:0}", frames / duration, 1f / bestDuration, 1f / worstDuration);
            frames = 0;
            duration = 0f;
            bestDuration = float.MaxValue;
            worstDuration = 0f;
        }
    }

}

