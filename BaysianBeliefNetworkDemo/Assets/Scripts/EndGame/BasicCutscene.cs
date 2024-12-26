using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCutscene : EndGameCutscene
{
	[SerializeField] private bool hasText;
	[SerializeField] private float timeBeforeText;
	protected override IEnumerator PlayScene()
	{
		yield return new WaitForSeconds(timeBeforeText);
		if (hasText) yield return ViewPanel();
	}

	protected override IEnumerator ExitTransition()
	{
		yield return null;
	}

	public override void Interrupt()
	{
		
	}
}