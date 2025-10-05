using UnityEngine;
using System.Collections;
using TMPro;

public class CallToActionSequence : MonoBehaviour
{
    public TypewriterEffect titleEffect;
    public TypewriterEffect subtitleEffect;
    public FadableTextMeshPro callToActionText;
    public FadableImage backgroundFadableImage;
    public FadableImage blackBackground;
    public float waitTimeBetweenText;
    public float fadeInTime;
    public TMP_Text alienTitleEffect;
    public TMP_Text alienSubtitleEffect;
    public TMP_Text alienCallToAction;
    public float glitchInterval;
    public TMPGlitchEffect textGlitch;

    private void Start()
    {
        titleEffect.Clear();
        subtitleEffect.Clear();
        callToActionText.SetAlpha(0f);
        backgroundFadableImage.SetAlpha(0f);
        textGlitch.TurnOff();
        StartCoroutine(StartBehavior());
    }

    private IEnumerator StartBehavior()
    {
        yield return FadeOutElement(blackBackground);
        yield return new WaitForSeconds(waitTimeBetweenText);
        yield return FadeInElement(backgroundFadableImage);
        yield return TypeInText(titleEffect);
        yield return TypeInText(subtitleEffect);
        yield return OffsetGlitch();
        yield return FadeInElement(callToActionText);
        yield return new WaitForSeconds(waitTimeBetweenText);
        yield return AlienTextGlitch();
        yield return new WaitForSeconds(waitTimeBetweenText*3);
        yield return FadeInElement(blackBackground);
    }

    private IEnumerator TypeInText(TypewriterEffect effect)
    {
        effect.UpdateText();
        yield return new WaitForSeconds(effect.GetTypingTime()+waitTimeBetweenText);
    }

    private IEnumerator FadeInElement(FadableElement element)
    {
        element.FadeIn(fadeInTime);
        yield return new WaitForSeconds(fadeInTime);
    }

    private IEnumerator FadeOutElement(FadableElement element)
    {
        element.FadeOut(fadeInTime);
        yield return new WaitForSeconds(fadeInTime);
    }

    private IEnumerator GlitchOut()
    {
        ToggleActiveText();
        yield return new WaitForSeconds(glitchInterval);
        ToggleActiveText();
    }

    private void ToggleActiveText()
    {
        bool isActive = titleEffect.gameObject.activeSelf;
        titleEffect.gameObject.SetActive(isActive);
        subtitleEffect.gameObject.SetActive(isActive);
        callToActionText.gameObject.SetActive(isActive);
        alienTitleEffect.gameObject.SetActive(!isActive);
        alienSubtitleEffect.gameObject.SetActive(!isActive);
        alienCallToAction.gameObject.SetActive(!isActive);
    }

    private IEnumerator AlienTextGlitch()
    {
        AudioManager.instance.PlayEffect("MiniGlitch");
        textGlitch.ToggleAlienTextVisibility(true);
        callToActionText.gameObject.SetActive(false);
        alienCallToAction.gameObject.SetActive(true);
        yield return new WaitForSeconds(glitchInterval);
        textGlitch.ToggleAlienTextVisibility(false);
        callToActionText.gameObject.SetActive(true);
        alienCallToAction.gameObject.SetActive(false);
    }

    private IEnumerator OffsetGlitch()
    {
        AudioManager.instance.PlayEffect("MiniGlitch");
        textGlitch.OffsetText();
        yield return new WaitForSeconds(glitchInterval);
        ResetTexts();
        yield return new WaitForSeconds(waitTimeBetweenText);
    }

    // Triangle mesh vertice issue - good hack
    private void ResetTexts()
    {
        titleEffect.gameObject.SetActive(false);
        subtitleEffect.gameObject.SetActive(false);
        callToActionText.gameObject.SetActive(false);
        titleEffect.gameObject.SetActive(true);
        subtitleEffect.gameObject.SetActive(true);
        callToActionText.gameObject.SetActive(true);
    }
}