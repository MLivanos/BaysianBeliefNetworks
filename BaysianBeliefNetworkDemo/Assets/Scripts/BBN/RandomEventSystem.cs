using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEventSystem : MonoBehaviour
{
	[SerializeField] private List<RandomEvent> events;

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
		Debug.Log(randomEvent.name);
		Debug.Log(randomEvent.GetMessage());
		Debug.Log(randomEvent.GetDescription());
		randomEvent.ApplyOperations();
	}
}