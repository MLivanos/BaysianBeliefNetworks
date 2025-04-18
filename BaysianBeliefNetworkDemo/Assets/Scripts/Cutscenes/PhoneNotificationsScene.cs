using System.Collections;
using UnityEngine;

public class PhoneNotificationsScene : IntroCutscene
{
    [SerializeField] private string phoneChime;
	[SerializeField] private SlideInBehavior[] textMessages;
    [SerializeField] private float timeBetweenTexts;
    [SerializeField] private FadableImage phoneToSleep;
    [SerializeField] private SlideInBehavior cameraSlide;

	protected override IEnumerator PlayScene()
    {
    	foreach(SlideInBehavior textMessage in textMessages)
        {
            textMessage.gameObject.SetActive(true);
            yield return null; // Wait one frame to allow slide in behaviors to initialize
            audioManager.PlayEffect(phoneChime);
            textMessage.BeginSlideIn();
            yield return new WaitForSeconds(timeBetweenTexts);
        }
    }

    public override void Interrupt()
    {
    	return;
    }

    protected override IEnumerator ExitTransition()
    {
    	yield return phoneToSleep.Fade(0.5f, true);
        cameraSlide.BeginSlideIn();
        yield return new WaitForSeconds(cameraSlide.GetDuration());
    }
}