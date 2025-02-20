using System.Collections;
using UnityEngine;

public abstract class FadableElement : MonoBehaviour
{
    private Coroutine currentFade;

    public abstract void SetAlpha(float alpha);

    public void FadeIn(float time, float maxAlpha=1f)
    {
        currentFade = StartCoroutine(Fade(time, true, maxAlpha));
    }

    public void FadeOut(float time, float maxAlpha=1f)
    {
        currentFade = StartCoroutine(Fade(time, false, maxAlpha));
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

    public void Interupt()
    {
        StopCoroutine(currentFade);
    }
}