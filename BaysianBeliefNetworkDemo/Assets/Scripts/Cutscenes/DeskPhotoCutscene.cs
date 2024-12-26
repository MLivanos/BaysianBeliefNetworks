using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskPhotoCutscene : EndGameCutscene
{
    [SerializeField] private float sceneWaitTime;
    [SerializeField] private SlideInBehavior[] photoSlides;
    [SerializeField] private SlideInBehavior[] photoSlideOuts;
    [SerializeField] private SlideInBehavior transitionPicture;
    [SerializeField] private SlideInBehavior transitionCameraSlide;

    protected override IEnumerator PlayScene()
    {
        yield return new WaitForSeconds(sceneWaitTime);
        int index = 1;
        foreach(SlideInBehavior photo in photoSlides)
        {
            audioManager.PlayEffect("PhotoSlide" + index++.ToString());
            photo.BeginSlideIn();
            yield return new WaitForSeconds(photo.GetDuration());
        }
        yield return ViewPanel();
        AnimateText();
    }

    public override void Interrupt()
    {
        foreach(SlideInBehavior photo in photoSlides)
        {
            photo.SetAtTerminalPoint(false);
        }
    }

    protected override IEnumerator ExitTransition()
    {
        int index = 4;
        foreach(SlideInBehavior photo in photoSlideOuts)
        {
            audioManager.PlayEffect("PhotoSlide" + index++.ToString());
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
