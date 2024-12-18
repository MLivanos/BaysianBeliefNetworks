using System.Collections;
using UnityEngine;

public class LabReunionCutscene : CutsceneBehavior
{
	[SerializeField] private string hugAudio;
    [SerializeField] private float timeBeforeSound;
    [SerializeField] private float timeBeforeText;
    [SerializeField] private SlideInBehavior cameraSlide;

	protected override IEnumerator PlayScene()
    {
        yield return new WaitForSeconds(timeBeforeSound);
        audioManager.PlayEffect(hugAudio);
    	yield return new WaitForSeconds(timeBeforeText);
    	yield return ViewPanel();
        AnimateText();
    }

    public override void Interrupt()
    {
    	return;
    }

    protected override IEnumerator ExitTransition()
    {
        cameraSlide.BeginSlideIn();
        yield return new WaitForSeconds(cameraSlide.GetDuration());
    }
}