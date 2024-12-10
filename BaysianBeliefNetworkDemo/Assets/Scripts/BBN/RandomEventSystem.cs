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
	private Coroutine notificationCoroutine;

	private void Start()
	{
		foreach(RandomEvent randomEvent in events)
		{
			randomEvent.Initialize();
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown("space"))
        {
            DrawEvent();
        }
	}

	public void DrawEvent()
	{
		int index = (int)(Random.Range(0, events.Count));
		RandomEvent randomEvent = events[index];
		if (notificationCoroutine != null) ClearNotification();
		notificationCoroutine = StartCoroutine(NotifyUser(randomEvent));
		randomEvent.ApplyOperations();
	}

	public IEnumerator NotifyUser(RandomEvent randomEvent)
	{
		notificationPanel.SetActive(true);
		messageText.text = randomEvent.GetMessage();
		eventText.text = randomEvent.GetDescription();
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