using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEventSystem : MonoBehaviour
{
	[SerializeField] private List<RandomEvent> events;
	private List<RandomEvent> spentEvents = new List<RandomEvent>();
	private List<RandomEvent> inverseEvents = new List<RandomEvent>();

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
		int index = (int)(Random.Range(0, events.Count+inverseEvents.Count-1));
		bool isInverse = index >= events.Count;
		List<RandomEvent> relevantList = isInverse ? inverseEvents : events;
		index = isInverse ? index - events.Count : index;
		RandomEvent randomEvent = relevantList[index];
		Debug.Log(randomEvent.name);
		Debug.Log(randomEvent.GetMessage());
		Debug.Log(randomEvent.GetDescription());
		randomEvent.ApplyOperations();
		if (!isInverse)
		{
			RandomEvent inverseEvent = randomEvent.GetInverse();
			inverseEvents.Add(inverseEvent);
			spentEvents.Add(randomEvent);
		}
		else
		{
			events.Add(spentEvents[index]);
			spentEvents.RemoveAt(index);
		}
		relevantList.RemoveAt(index);
	}
}