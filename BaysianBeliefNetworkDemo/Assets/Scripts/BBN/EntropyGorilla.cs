using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntropyGorilla : MonoBehaviour
{
	[SerializeField] private float minTimeBetweenEvents;
	[SerializeField] private float maxTimeBetweenEvents;
	[SerializeField] private float timeBetweenInitialEvents;
	[SerializeField] private int initialEvents;
	private RandomEventSystem randomEventSystem;

	private void Start()
	{
		randomEventSystem = GetComponent<RandomEventSystem>();
	}

	public void PokeGorilla()
	{
		StartCoroutine(GorillaCrashout());
	}

	public void TauntGorilla()
	{
		StartCoroutine(GorillaOutburst());
	}

	private IEnumerator GorillaCrashout()
	{
		Debug.Log("Oh god the gorilla is crashing out! Oh god oh no!");
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(minTimeBetweenEvents, maxTimeBetweenEvents));
			randomEventSystem.DrawEvent();
		}
	}

	private IEnumerator GorillaOutburst()
	{
		for(int i=0; i<initialEvents; i++)
		{
			randomEventSystem.DrawEvent();
			yield return new WaitForSeconds(timeBetweenInitialEvents);
		}
	}
}