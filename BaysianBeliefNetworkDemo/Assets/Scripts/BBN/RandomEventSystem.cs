using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RandomEventSystem : MonoBehaviour
{
	[SerializeField] private List<RandomEvent> events;
	[SerializeField] private bool runningTest;
	[SerializeField] private int testIndex;
	private NotificationManager notificationManager;

	private void Start()
	{
		notificationManager = FindObjectOfType<NotificationManager>();
		if (runningTest)
		{
			NotifyUser("You are running the RandomEventSystem in test mode",
				"Make sure you don't see this message in a build");
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
		NotifyUser(randomEvent.GetMessage(), randomEvent.GetDescription());
		randomEvent.ApplyOperations();
		FindObjectOfType<SaveSystem>().SaveGame();
	}

	public void NotifyUser(string message, string description)
	{
		notificationManager.ShowNotification(message, description);
	}
}