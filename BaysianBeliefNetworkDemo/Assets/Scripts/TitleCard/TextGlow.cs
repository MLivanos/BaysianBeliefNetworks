using TMPro;
using UnityEngine;
using System.Collections;

public class TextGlow : MonoBehaviour
{
    private TextMeshProUGUI titleText;
    private Color textColor;
    [SerializeField] private float glowSpeed = 2f;
    [SerializeField] private float glowIntensity = 0.2f;
    [SerializeField] private float minAlpha = 0.8f;

    private void Start()
    {
        titleText = GetComponent<TextMeshProUGUI>();
        textColor = titleText.color;
    }

    public void StartGlow()
    {
        StartCoroutine(Glow());
    }

    private IEnumerator Glow()
    {
        while (true)
        {
            float t = Mathf.PerlinNoise(Time.time * glowSpeed, 0f);
            float glow = t * glowIntensity;
            titleText.alpha = Mathf.Clamp01(minAlpha + glow);
            yield return null;
        }
    }
}
