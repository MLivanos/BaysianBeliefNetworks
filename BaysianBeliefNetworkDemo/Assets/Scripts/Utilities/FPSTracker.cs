using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class FPSTracker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsCounterText;
    [SerializeField] private int tickEpoch = 10;
    [SerializeField] private int longEpoch = 200;
    private float[] fpsHistory;
    private float medianFPS=0f;
    private float lowFPS=0f;
    private float currentFPS;
    private int historySize;

    private void Start()
    {
        historySize = (int)longEpoch/tickEpoch;
        fpsHistory = new float[historySize];
    }

    public void StartTracking()
    {
        StartCoroutine(TrackFPS());
    }

    private IEnumerator TrackFPS()
    {
        float timer = 0f;
        int frameCounter = 0;
        int epochCounter = 0;

        while (true)
        {
            timer += Time.unscaledDeltaTime;
            frameCounter++;

            if (frameCounter % tickEpoch == 0)
            {
                currentFPS = frameCounter / timer;

                fpsHistory[epochCounter++ % historySize] = currentFPS;

                GetMedianAndLowFPS();
                fpsCounterText.text = $"FPS: {Mathf.Round(currentFPS)}\nLow: {Mathf.Round(lowFPS)}\nMed: {Mathf.Round(medianFPS)}";

                frameCounter = 0;
                timer = 0f;
            }

            yield return null;
        }
    }

    private void GetMedianAndLowFPS()
    {
        float[] sorted = fpsHistory.OrderBy(fps => fps).ToArray();
        medianFPS = sorted[sorted.Length / 2];
        lowFPS = sorted[0];
    }
}