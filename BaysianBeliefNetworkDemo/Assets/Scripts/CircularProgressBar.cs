using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ColorRange
{
    public Color startColor;
    public Color endColor;
    public float start;
    public float end;

    public bool InRange(float value)
    {
        return value >= start && value <=end;
    }

    public Color GetColor(float value)
    {
        return Color.Lerp(startColor, endColor, value/end);
    }
}

public class CircularProgressBar : MonoBehaviour
{
    [SerializeField] private float maxValue=1f;
    [SerializeField] private float minValue=0f;
    [SerializeField] private Image progressDisplay;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private int decimalPrecision;
    [SerializeField] private int significantFigures;
    [SerializeField] private List<ColorRange> colorGradients;
    private float progress;

    private void Start()
    {
        progress = progressDisplay.fillAmount*(maxValue-minValue) + minValue;
    }

    private void Update()
    {
        UpdateProgress(progress - Time.deltaTime);
    }

    public void UpdateProgress(float newProgress)
    {
        progress = Mathf.Clamp(newProgress, minValue, maxValue);
        progressDisplay.fillAmount = (progress - minValue)/(maxValue-minValue);
        UpdateColor();
        string displayText = significantFigures > 0 ? RoundToSignificantFigures(newProgress, significantFigures).ToString() : newProgress.ToString("#"+ new string('0', decimalPrecision));
        progressText.text = displayText;
    }

    private float RoundToSignificantFigures(float num, int digits)
    {
        if(num == 0) return 0;

        float orderOfMagnitude = Mathf.Floor(Mathf.Log10(Mathf.Abs(num))) + 1;
        float scale = Mathf.Pow(10, Mathf.Max(orderOfMagnitude - digits, 1));
        return scale * Mathf.Round(num / scale);
    }

    private void UpdateColor()
    {
        foreach (ColorRange colorRange in colorGradients)
        {
            if (colorRange.InRange(progress))
            {
                progressDisplay.color = colorRange.GetColor(progress);
                return;
            }
        }
    }
}