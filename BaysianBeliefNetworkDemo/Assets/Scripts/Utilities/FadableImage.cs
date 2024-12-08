using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadableImage : MonoBehaviour, IFadable
{
    private Image _image;
    private RawImage _rawImage;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _rawImage = GetComponent<RawImage>();
    }

    public void SetAlpha(float alpha)
    {
        if (_image != null)
        {
            var color = _image.color;
            color.a = alpha;
            _image.color = color;
        }
        else if (_rawImage != null)
        {
            var color = _rawImage.color;
            color.a = alpha;
            _rawImage.color = color;
        }
    }

    public void FadeIn(float time, float maxAlpha=1f)
    {
        StartCoroutine(Fade(time, true, maxAlpha));
    }

    public void FadeOut(float time, float maxAlpha=1f)
    {
        StartCoroutine(Fade(time, false, maxAlpha));
    }

    public IEnumerator Fade(float time, bool fadeIn, float maxAlpha=1f)
    {
        float timer = 0f;
        float alpha = fadeIn ? 0f : maxAlpha;
        while (timer < time)
        {
            SetAlpha(alpha);       
            timer += Time.deltaTime;
            alpha = maxAlpha * (fadeIn ? timer/time : 1 - timer/time);
            yield return null;
        }
        SetAlpha(fadeIn ? maxAlpha : 0f);
    }
}