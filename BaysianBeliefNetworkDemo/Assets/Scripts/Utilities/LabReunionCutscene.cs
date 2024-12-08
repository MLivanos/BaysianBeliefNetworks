using System.Collections;
using UnityEngine;

public class LabReunionCutscene : CutsceneBehavior
{
	[SerializeField] private AudioClip hugAudio;
    [SerializeField] private AudioSource hugAudioSource;
    [SerializeField] private float timeBeforeSound;
    [SerializeField] private float timeBeforeText;
    [SerializeField] private SlideInBehavior cameraSlide;

	protected override IEnumerator PlayScene()
    {
        yield return new WaitForSeconds(timeBeforeSound);
        hugAudioSource.PlayOneShot(hugAudio);
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