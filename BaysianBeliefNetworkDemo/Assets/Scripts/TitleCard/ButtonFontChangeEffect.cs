using System.Collections;
using UnityEngine;
using TMPro;

public class ButtonFontChangeEffect : MonoBehaviour
{
	[SerializeField] private TypewriterEffect humanTextEffect;
	[SerializeField] private TypewriterEffect alienTextEffect;
	private Coroutine typingCoroutine;

	public void ShowAlienText()
	{
		Interrupt();
		typingCoroutine = StartCoroutine(TypeNewText(humanTextEffect, alienTextEffect));
	}

	public void ShowHumanText()
	{
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
}