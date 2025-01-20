using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialStep : MonoBehaviour
{
	[SerializeField] private List<TutorialQuestBase> quests;
	private int questsCompleted = 0;

	// This will eventually be a public Initialize method, but for testing purposes we will do this
	private void Start()
	{
		Initialize();
	}

	public void Initialize()
	{
		foreach(TutorialQuestBase quest in quests)
		{
			quest.Initialize(this);
		}
	}

	public void OnQuestComplete(TutorialQuestBase quest)
	{
		// Eventually, provide feedback as to the specific quest
		questsCompleted++;
		if (questsCompleted == quests.Count) StepComplete();
	}

	private void StepComplete()
	{
		Debug.Log("This step is complete");
	}
}