using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class ButtonFontChangeEffect : MonoBehaviour
{
	[SerializeField] private TypewriterEffect humanTextEffect;
	[SerializeField] private TypewriterEffect alienTextEffect;
	private string buttonMessage;
	private bool clicked = false;
	private Coroutine typingCoroutine;

	private void Start()
	{
		if (!String.IsNullOrEmpty(humanTextEffect.Text())) buttonMessage = humanTextEffect.Text();
		else buttonMessage = alienTextEffect.Text();
	}

	public void ShowAlienText()
	{
		if (clicked) return;
		Interrupt();
		typingCoroutine = StartCoroutine(TypeNewText(humanTextEffect, alienTextEffect));
	}

	public void ShowHumanText()
	{
		if (clicked) return;
		Interrupt();
		typingCoroutine = StartCoroutine(TypeNewText(alienTextEffect, humanTextEffect));
	}

	private IEnumerator TypeNewText(TypewriterEffect oldTextEffect, TypewriterEffect newTextEffect)
	{
		oldTextEffect.TypewriterDelete();
		yield return new WaitForSeconds(2*oldTextEffect.GetTypingTime(buttonMessage, false));
		newTextEffect.UpdateText(buttonMessage);
		typingCoroutine = null;
	}

	public void Interrupt()
	{
		if (typingCoroutine == null) return;
		StopCoroutine(typingCoroutine);
		humanTextEffect.Interrupt();
		alienTextEffect.Interrupt();
	}

	public void Click()
	{
		clicked = true;
	}
}