using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RandomEventSystem : MonoBehaviour
{
	[SerializeField] private GameObject notificationPanel;
	[SerializeField] private List<FadableElement> fadableElements;
	[SerializeField] private float fadeTime;
	[SerializeField] private float stayTime;
	[SerializeField] private List<RandomEvent> events;
	[SerializeField] private TMP_Text messageText;
	[SerializeField] private TMP_Text eventText;
	[SerializeField] private bool runningTest;
	[SerializeField] private int testIndex;
	private Coroutine notificationCoroutine;

	private void Start()
	{
		if (runningTest)
		{
			StartCoroutine(NotifyUser("You are running the RandomEventSystem in test mode",
				"Make sure you don't see this message in a build"));
		}
		foreach(RandomEvent randomEvent in events)
		{
			randomEvent.Initialize();
		}
	}

	public void DrawEvent()
	{
		int index = runningTest ? testIndex++ % events.Count : (int)(Random.Range(0, events.Count));
		RandomEvent randomEvent = events[index];
		if (notificationCoroutine != null) ClearNotification();
		notificationCoroutine = StartCoroutine(NotifyUser(randomEvent.GetMessage(), randomEvent.GetDescription()));
		randomEvent.ApplyOperations();
		FindObjectOfType<SaveSystem>().SaveGame();
	}

	public IEnumerator NotifyUser(string message, string description)
	{
		notificationPanel.SetActive(true);
		messageText.text = message;
		eventText.text = description;
		foreach(FadableElement element in fadableElements)
		{
			element.FadeIn(fadeTime);
		}
		yield return new WaitForSeconds(fadeTime+stayTime);
		foreach(FadableElement element in fadableElements)
		{
			element.FadeOut(fadeTime);
		}
		yield return new WaitForSeconds(fadeTime);
		notificationPanel.SetActive(false);
		notificationCoroutine = null;
	}

	public void ClearNotification()
	{
		StopCoroutine(notificationCoroutine);
		foreach(FadableElement element in fadableElements)
		{
			element.SetAlpha(0f);
		}
	}
}