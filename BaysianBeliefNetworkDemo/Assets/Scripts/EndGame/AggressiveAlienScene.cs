using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveAlienScene : EndGameCutscene
{
	[SerializeField] private FadableLight blinker;
	[SerializeField] private float blinkerFrequency;
	[SerializeField] private float timeBeforeText;

	protected override IEnumerator PlayScene()
	{
		yield return new WaitForSeconds(timeBeforeText);
		yield return ViewPanel();
		AnimateText();
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