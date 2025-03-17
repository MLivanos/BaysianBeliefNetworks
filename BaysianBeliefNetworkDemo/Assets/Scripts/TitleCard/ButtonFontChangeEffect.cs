using System.Collections;
using UnityEngine;
using TMPro;

public class ButtonFontChangeEffect : MonoBehaviour
{
	[SerializeField] private TypewriterEffect humanTextEffect;
	[SerializeField] private TypewriterEffect alienTextEffect;
	private bool clicked = false;
	private Coroutine typingCoroutine;

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
		yield return new WaitForSeconds(2*oldTextEffect.GetTypingTime("PLAY", false));
		newTextEffect.UpdateText("PLAY");
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