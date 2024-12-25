using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveAlienScene : CutsceneBehavior
{
	[SerializeField] private FadableLight blinker;
	[SerializeField] private float blinkerFrequency;

	protected override IEnumerator PlayScene()
	{
		StartCoroutine(blinker.BlinkForever(blinkerFrequency, true));
		yield return null;
	}

	protected override IEnumerator ExitTransition()
	{
		yield return null;
	}

	public override void Interrupt()
	{
		
	}
}