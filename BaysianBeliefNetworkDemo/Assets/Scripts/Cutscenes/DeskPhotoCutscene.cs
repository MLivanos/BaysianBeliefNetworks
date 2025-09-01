using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskPhotoCutscene : IntroCutscene
{
    [SerializeField] private float sceneWaitTime;
    [SerializeField] private SlideInBehavior[] photoSlides;
    [SerializeField] private SlideInBehavior[] photoSlideOuts;
    [SerializeField] private SlideInBehavior transitionPicture;
    [SerializeField] private SlideInBehavior transitionCameraSlide;
    private bool isInterrupted = false;

    protected override IEnumerator PlayScene()
    {
        yield return new WaitForSeconds(sceneWaitTime);
        int index = 1;
        foreach(SlideInBehavior photo in photoSlides)
        {
            audioManager.PlayEffect("PhotoSlide" + index++.ToString());
            if (!isInterrupted) photo.BeginSlideIn();
            yield return new WaitForSeconds(photo.GetDuration());
        }
        if (isInterrupted) yield break;
        yield return ViewPanel();
        AnimateText();
    }

    public override void Interrupt()
    {
        isInterrupted = true;
        foreach(SlideInBehavior photo in photoSlides)
        {
            photo.Interupt();
            photo.SetAtTerminalPoint(false);
        }
    }

    protected override IEnumerator ExitTransition()
    {
        int index = 3;
        foreach(SlideInBehavior photo in photoSlideOuts)
        {
            audioManager.PlayEffect("PhotoSlide" + ((index++%6)+1).ToString());
            photo.BeginSlideIn();
            yield return new WaitForSeconds(photo.GetDuration());
        }
        audioManager.PlayEffect("PhotoSlide1");
        transitionPicture.BeginSlideIn();
        yield return new WaitForSeconds(transitionPicture.GetDuration());
        transitionCameraSlide.BeginSlideIn();
        yield return new WaitForSeconds(transitionCameraSlide.GetDuration());
    }
}
