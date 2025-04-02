using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingScreenTextEffect : MonoBehaviour
{
	[SerializeField] private TMP_Text loadingText;
	[SerializeField] private float timeBetweenElipses = 0.25f;
	[SerializeField] private string message = "LOADING";
	[SerializeField] private bool fadeIn;

	public void ChangeMessage(string newMessage)
	{
		message = newMessage;
	}

	public void ChangeFadeIn(bool newFadeIn)
	{
		fadeIn = newFadeIn;
	}

	public void StartElipsesEffect()
	{
		StartCoroutine(PlayElipses());
	}

	private IEnumerator PlayElipses()
	{
		FadableTextMeshPro fadeEffect = GetComponent<FadableTextMeshPro>();
		if (fadeIn && fadeEffect != null) yield return fadeEffect.Fade(timeBetweenElipses, true);
		int numberOfElipses = 0;
		while(true)
		{
			loadingText.text = message + new string('.', numberOfElipses);
			yield return new WaitForSeconds(timeBetweenElipses);
			numberOfElipses++;
			numberOfElipses %= 4;
		}
	}
}