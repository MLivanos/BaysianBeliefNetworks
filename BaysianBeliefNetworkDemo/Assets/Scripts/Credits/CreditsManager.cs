using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsManager : MonoBehaviour
{
    [SerializeField] private FadableImage fadeToBlack;
    [SerializeField] private Transform scrollingObject;
    [SerializeField] private float scrollSpeed;
    [SerializeField] private float stopHeight;

    private void Start()
    {
        StartCoroutine(RollCredits());
    }

    private IEnumerator RollCredits()
    {
        yield return FadeToBlack();
        yield return Scroll();
    }

    private IEnumerator FadeToBlack()
    {
        fadeToBlack.SetAlpha(1f);
        yield return fadeToBlack.Fade(2f, false);
    }

    private IEnumerator Scroll()
    {
        while(scrollingObject.position.y < stopHeight)
        {
            scrollingObject.Translate(Vector3.up * scrollSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
