using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskPhotoCutscene : CutsceneBehavior
{
    [SerializeField] private float sceneWaitTime;
    [SerializeField] private SlideInBehavior[] photoSlides;
    [SerializeField] private SlideInBehavior[] photoSlideOuts;
    [SerializeField] private SlideInBehavior transitionPicture;
    [SerializeField] private SlideInBehavior transitionCameraSlide;

    protected override IEnumerator PlayScene()
    {
        yield return new WaitForSeconds(sceneWaitTime);
        foreach(SlideInBehavior photo in photoSlides)
        {
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
        foreach(SlideInBehavior photo in photoSlideOuts)
        {
            photo.BeginSlideIn();
            yield return new WaitForSeconds(photo.GetDuration());
        }
        transitionPicture.BeginSlideIn();
        yield return new WaitForSeconds(transitionPicture.GetDuration());
        transitionCameraSlide.BeginSlideIn();
        yield return new WaitForSeconds(transitionCameraSlide.GetDuration());
    }
}
