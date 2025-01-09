using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsManager : MonoBehaviour
{
    [SerializeField] private FadableImage fadeToBlack;
    [SerializeField] private Transform scrollingObject;
    [SerializeField] private FadableElement[] buttons;
    [SerializeField] private float scrollSpeed;
    [SerializeField] private float scrollSpeedMultiplier;
    [SerializeField] private float stopHeight;
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = AudioManager.instance;
        audioManager.PlayMusic("SynthpopFirstFlight");
        StartCoroutine(RollCredits());
    }

    private IEnumerator RollCredits()
    {
        HideButtons();
        yield return FadeToBlack();
        yield return Scroll();
        FadeInButtons();
    }

    private IEnumerator FadeToBlack()
    {
        fadeToBlack.SetAlpha(1f);
        yield return fadeToBlack.Fade(2f, false);
    }

    private IEnumerator Scroll()
    {
        while(scrollingObject.localPosition.y < stopHeight)
        {
            if (Input.GetMouseButtonDown(0)) scrollSpeed *= scrollSpeedMultiplier;
            else if (Input.GetMouseButtonUp(0)) scrollSpeed /= scrollSpeedMultiplier;
            scrollingObject.Translate(Vector3.up * scrollSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void FadeInButtons()
    {
        foreach(FadableElement element in buttons)
        {
            element.gameObject.SetActive(true);
            element.FadeIn(2f);
        }
    }

    private void HideButtons()
    {
        foreach(FadableElement element in buttons)
        {
            element.SetAlpha(0f);
            element.gameObject.SetActive(false);
        }
    }
}
