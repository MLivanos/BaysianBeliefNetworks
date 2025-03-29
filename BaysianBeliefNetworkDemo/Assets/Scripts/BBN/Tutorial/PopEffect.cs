using UnityEngine;
using System;
using System.Collections;

public class PopEffect : MonoBehaviour
{
    [SerializeField] private float animationDuration = 0.2f;
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private string popInSound;
    [SerializeField] private string popOutSound;
    
    private Coroutine animationCoroutine;
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = AudioManager.instance;
    }

    public void PlayPopIn()
    {
        gameObject.SetActive(true);
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
        if (!String.IsNullOrEmpty(popInSound)) audioManager.PlayEffect(popInSound);
        animationCoroutine = StartCoroutine(PopRoutine(popIn: true));
    }

    public void PlayPopDown()
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
        if (!String.IsNullOrEmpty(popOutSound)) audioManager.PlayEffect(popOutSound);
        animationCoroutine = StartCoroutine(PopRoutine(popIn: false));
    }

    private IEnumerator PopRoutine(bool popIn)
    {
        float timer = 0f;
        Vector3 originalScale = transform.localScale;
        float startY = originalScale.y;
        float endY = popIn ? 1f : 0f;

        while (timer < animationDuration)
        {
            float t = timer / animationDuration;
            float curveT = scaleCurve.Evaluate(t);
            float yScale = Mathf.Lerp(startY, endY, curveT);
            transform.localScale = new Vector3(originalScale.x, yScale, originalScale.z);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.localScale = new Vector3(originalScale.x, endY, originalScale.z);

        if (!popIn) gameObject.SetActive(false);

        animationCoroutine = null;
    }
}