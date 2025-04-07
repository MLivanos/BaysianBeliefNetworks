using System.Collections;
using UnityEngine;

[RequireComponent(typeof(FadableElement))]
public class PulsingElement : MonoBehaviour
{
    [SerializeField] private float pulseDuration = 1f;
    [SerializeField] private float minAlpha = 0.3f;
    [SerializeField] private float maxAlpha = 1f;
    [SerializeField] private bool pulseOnEnable = true;

    private FadableElement fadable;
    private Coroutine pulseRoutine;

    private void Awake()
    {
        fadable = GetComponent<FadableElement>();
    }

    private void OnEnable()
    {
        if (pulseOnEnable) StartPulse();
    }

    private void OnDisable()
    {
        StopPulse();
    }

    public void StartPulse()
    {
        if (pulseRoutine != null) StopCoroutine(pulseRoutine);
        pulseRoutine = StartCoroutine(PulseLoop());
    }

    public void StopPulse()
    {
        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
            pulseRoutine = null;
        }
        fadable.SetAlpha(maxAlpha);
    }

    private IEnumerator PulseLoop()
    {
        while (true)
        {
            yield return StartCoroutine(fadable.Fade(pulseDuration, false, maxAlpha));
            yield return StartCoroutine(fadable.Fade(pulseDuration, true, maxAlpha));
        }
    }
}