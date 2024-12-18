using System.Collections;
using UnityEngine;

public class LeavingCutscene : CutsceneBehavior
{
	[SerializeField] private GameObject phoneLight;
	[SerializeField] private AudioClip textMessageAudio;
    [SerializeField] private AudioSource textMessageAudioSource;
    [SerializeField] private float timeBeforeText;
    [SerializeField] private float timeBetweenLightAndMessage;
    [SerializeField] private float hangTime;

	protected override IEnumerator PlayScene()
    {
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
    	phoneLight.SetActive(true);
        yield return new WaitForSeconds(timeBetweenLightAndMessage);
        textMessageAudioSource.PlayOneShot(textMessageAudio);
        yield return new WaitForSeconds(hangTime);
    }
}