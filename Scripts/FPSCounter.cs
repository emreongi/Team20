using UnityEngine;
using System.Collections.Generic;

public class FPSCounter : MonoBehaviour
{
    /*
    private int frameCount = 0;
    private float nextUpdate = 0.0f;
    private float fps = 0.0f;
    private float updateRate = 4.0f;
    private List<float> fpsValues = new List<float>();

    void Awake()
    {
        GameAnalytics.Initialize();
    }

    void Start()
    {
        GameAnalytics.NewDesignEvent("test:unity_editor");
    }

    void Update()
    {
        frameCount++;
        if (Time.time > nextUpdate)
        {
            nextUpdate += 1.0f / updateRate;
            fps = frameCount * updateRate;
            frameCount = 0;
            fpsValues.Add(fps);

            if (fps < 20.0f)
            {
                GameAnalytics.NewDesignEvent("criticalFPS", fps);
            }
        }
    }

    void OnDestroy()
    {
        float meanFPS = 0.0f;
        if (fpsValues.Count > 0)
        {
            meanFPS = CalculateMeanFPS();
        }

        GameAnalytics.NewDesignEvent("averageFPS", meanFPS);
    }

    private float CalculateMeanFPS()
    {
        float sum = 0.0f;
        foreach (float value in fpsValues)
        {
            sum += value;
        }
        return sum / fpsValues.Count;
    }
    */
}