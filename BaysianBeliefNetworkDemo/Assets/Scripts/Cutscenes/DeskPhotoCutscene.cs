using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskPhotoCutscene : CutsceneBehavior
{
    [SerializeField] private SlideInBehavior[] photoSlides;
    [SerializeField] private string deskPhotoText;

    protected override IEnumerator PlayScene()
    {
        // Skip first frame to give Start method a chance to be called
        yield return null;
        foreach(SlideInBehavior photo in photoSlides)
        {
            photo.BeginSlideIn();
            yield return new WaitForSeconds(photo.GetDuration());
        }
        textPanel.SetActive(true);
        yield return ViewPanel();
        typewriterEffect.UpdateText(deskPhotoText);
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
        yield return null;
    }
}
