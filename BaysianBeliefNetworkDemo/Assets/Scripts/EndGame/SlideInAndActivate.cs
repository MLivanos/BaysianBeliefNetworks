using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideInAndActivate : EndGameCutscene
{
	[SerializeField] private List<SlideInBehavior> objectsToSlide;
	[SerializeField] private List<GameObject> objectsToActivate;
	[SerializeField] private float activationPauseTime;
	[SerializeField] private bool slideFirst;

	protected override IEnumerator PlayScene()
	{
		yield return null;
		yield return slideFirst ? SlideIn() : ActivateObjects();
		yield return slideFirst ? ActivateObjects() : SlideIn();
	}

	protected override IEnumerator ExitTransition()
	{
		yield return null;
	}

	private IEnumerator SlideIn()
	{
		foreach(SlideInBehavior slide in objectsToSlide)
		{
			slide.BeginSlideIn();
			yield return new WaitForSeconds(slide.GetDuration());
		}
	}

	private IEnumerator ActivateObjects()
	{
		foreach(GameObject objectToActivate in objectsToActivate)
		{
			objectToActivate.SetActive(true);
			yield return new WaitForSeconds(activationPauseTime);
		}
	}

	public override void Interrupt()
	{
		foreach(SlideInBehavior slide in objectsToSlide)
        {
            slide.SetAtTerminalPoint(false);
        }
        foreach(GameObject objectToActivate in objectsToActivate)
		{
			objectToActivate.SetActive(true);
		}
	}
}