using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropagationHighlighter : MonoBehaviour
{
    [System.Serializable]
    public class Step
    {
        // All these fade in together during this step
        public List<FadableImage> items = new List<FadableImage>();
    }

    [Header("Propagation sequence (parents first, then children, etc.)")]
    [Tooltip("Each Step is one 'wave' of glow. All items in a Step fade in together. Next Step waits until these are done.")]
    [SerializeField] private List<Step> steps = new List<Step>();

    [Header("Timings")]
    [SerializeField, Tooltip("Seconds each item takes to fade in. Uses your FadableImage.FadeIn(time).")]
    private float fadeInTime = 0.35f;

    [SerializeField, Tooltip("Pause between steps, after a step has fully faded in.")]
    private float interStepDelay = 0.15f;

    [SerializeField, Tooltip("Hold time after the final step, before reset.")]
    private float finalHoldTime = 0.75f;

    [SerializeField, Tooltip("Seconds to fade everything out on reset. If your FadableImage has a FadeOut, we'll use it; else we snap off.")]
    private float fadeOutTime = 0.25f;

    [Header("Behavior")]
    [SerializeField, Tooltip("If true, we auto-reset all glows to fully hidden on Awake.")]
    private bool resetOnAwake = true;

    private bool _running;
    private int _runVersion;

    private void Awake()
    {
        if (resetOnAwake)
            SnapAllOff();
        Play();
    }


    public void Play()
    {
        StartCoroutine(LoopSequnece());
    }

    public IEnumerator LoopSequnece()
    {
        yield return RunSequence(_runVersion);
        while (true)
        {
            _runVersion++;
            yield return RestartSoon();
        }
    }

    private IEnumerator RestartSoon()
    {
        yield return null;
        _runVersion++;
        yield return RunSequence(_runVersion);
    }

    private IEnumerator RunSequence(int version)
    {
        _running = true;

        // --- Fade in per step ---
        for (int s = 0; s < steps.Count; s++)
        {
            if (version != _runVersion) { _running = false; yield break; }

            List<FadableImage> wave = steps[s].items;

            // Kick off all fades in parallel
            foreach (var img in wave)
            {
                if (img == null) continue;
                img.FadeIn(fadeInTime);
            }

            yield return new WaitForSeconds(fadeInTime);

            if (interStepDelay > 0f)
                yield return new WaitForSeconds(interStepDelay);
        }

        // --- Final hold ---
        if (finalHoldTime > 0f)
            yield return new WaitForSeconds(finalHoldTime);

        if (version != _runVersion) { _running = false; yield break; }
        yield return FadeAllOff(fadeOutTime);

        _running = false;
    }

    private void SnapAllOff()
    {
        foreach (Step step in steps)
        {
            foreach (FadableImage img in step.items)
            {
                if (img == null) continue;
                img.SetAlpha(0f);
            }
        }
    }

    private IEnumerator FadeAllOff(float time)
    {
        foreach (Step step in steps)
        {
            foreach (FadableImage img in step.items)
            {
                if (img == null) continue;
                img.FadeOut(fadeOutTime);
            }
        }
        yield return new WaitForSeconds(time);
    }
}
