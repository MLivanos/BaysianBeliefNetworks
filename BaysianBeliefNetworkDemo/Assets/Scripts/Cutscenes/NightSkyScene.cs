using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NightSkyScene : IntroCutscene
{
    [SerializeField] private ParticleSystem singleShotStar;
    [SerializeField] private GameObject[] shootingStars;
    [SerializeField] private float waitForStarTime;
    [SerializeField] private float preTextWaitTime;
    [SerializeField] private float firstLineWaitTime;
    [SerializeField] private Transform[] cameraEnds;
    [SerializeField] private float cameraMoveDuration;
    [SerializeField] private FadableImage whiteOutImage;
    [SerializeField] private float whiteOutTimer;

    protected override IEnumerator PlayScene()
    {
        yield return FadeInFromWhite();
        yield return StartShootingStars();
        yield return DisplayText();
        yield return SlideOutCamera();
    }

    public override void Interrupt()
    {
        cameraTransform.position = cameraEnds[1].position;
    }

    protected override IEnumerator ExitTransition()
    {
        textPanel.SetActive(false);
        whiteOutImage.gameObject.SetActive(true);
        float alpha = 0f;
        float timer = 0f;
        while (timer < whiteOutTimer)
        {
            whiteOutImage.SetAlpha(alpha);
            timer += Time.deltaTime;
            alpha = timer / whiteOutTimer;
            yield return null;
        }
    }

    private IEnumerator FadeInFromWhite()
    {
        whiteOutImage.gameObject.SetActive(true);
        yield return null;
        yield return whiteOutImage.Fade(2f, false);
    }

    private IEnumerator StartShootingStars()
    {
        yield return new WaitForSeconds(waitForStarTime);
        singleShotStar.Emit(1);
        yield return new WaitForSeconds(preTextWaitTime-waitForStarTime);
        foreach(GameObject shootingStarGenerator in shootingStars)
        {
            yield return new WaitForSeconds(0.35f);
            shootingStarGenerator.SetActive(true);
        }
    }

    private IEnumerator DisplayText()
    {
        yield return ViewPanel();
        AnimateText();
        yield return new WaitForSeconds(firstLineWaitTime);
    }

    private IEnumerator SlideOutCamera()
    {
        float timer = 0f;
        while(timer < cameraMoveDuration)
        {
            cameraTransform.position = Vector3.Lerp(cameraEnds[0].position, cameraEnds[1].position, timer/cameraMoveDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        cameraTransform.position = cameraEnds[1].position;
    }
}
