using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NightSkyScene : CutsceneBehavior
{
    [SerializeField] private float preTextWaitTime;
    [SerializeField] private float firstLineWaitTime;
    [SerializeField] private Transform[] cameraEnds;
    [SerializeField] private float cameraMoveDuration;
    [SerializeField] private string nightSkyText;
    [SerializeField] private FadableImage whiteOutImage;
    [SerializeField] private float whiteOutTimer;

    protected override IEnumerator PlayScene()
    {
        yield return new WaitForSeconds(preTextWaitTime);
        textPanel.SetActive(true);
        typewriterEffect.UpdateText(nightSkyText);
        yield return new WaitForSeconds(firstLineWaitTime);
        float timer = 0f;
        while(timer < cameraMoveDuration)
        {
            cameraTransform.position = Vector3.Lerp(cameraEnds[0].position, cameraEnds[1].position, timer/cameraMoveDuration);
            timer += Time.deltaTime;
            yield return null;
        }
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
}
