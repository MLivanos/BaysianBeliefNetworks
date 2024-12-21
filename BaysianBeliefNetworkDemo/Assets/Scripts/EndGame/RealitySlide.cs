using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RealitySlide : EndGameSlide
{
	[SerializeField] private FadableLight blinker;
	[SerializeField] private float blinkerFrequency;

	private void Start()
	{
		code = 1;
		StartCoroutine(PlayScene());
	}

	protected override IEnumerator PlayScene()
	{
		scenes[code].SetActive(true);
		switch(code)
		{
			case 0:
				break;
			case 1:
				StartCoroutine(PlayAggressiveAliens());
				break;
			case 2:
				break;
			default:
				break;
		}
		yield return null;
	}

	protected override IEnumerator ExitTransition()
	{
		yield return null;
	}

	private IEnumerator PlayAggressiveAliens()
	{
		StartCoroutine(blinker.BlinkForever(blinkerFrequency, true));
		yield return null;
	}
}