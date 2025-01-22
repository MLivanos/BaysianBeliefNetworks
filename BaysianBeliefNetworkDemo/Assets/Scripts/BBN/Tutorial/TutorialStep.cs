using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialStep : MonoBehaviour
{
	// Eventually let TM do this
	[SerializeField] private ObjectiveSpawner objectiveSpawner;
	[SerializeField] private DropdownList dropdownList;
	[SerializeField] private List<TutorialQuestBase> quests;
	[SerializeField] private List<GameObject> stepObjects;
	private int questsCompleted = 0;

	// This will eventually be a public Initialize method, but for testing purposes we will do this
	private void Start()
	{
		Initialize();
	}

	public void Initialize()
	{
		ChangeHighlight(true);
		foreach(TutorialQuestBase quest in quests)
		{
			quest.Initialize(this);
			objectiveSpawner.AddObjective(quest.GetDescription());
		}
		dropdownList.Peep();
	}

	private void ChangeHighlight(bool highlightOn)
	{
		foreach(GameObject highlight in stepObjects)
		{
			highlight.SetActive(highlightOn);
		}
	}

	public void OnQuestComplete(TutorialQuestBase quest)
	{
		// Eventually, provide feedback as to the specific quest
		objectiveSpawner.CompleteQuest(quests.FindIndex(q => q == quest));
		questsCompleted++;
		if (questsCompleted == quests.Count) StepComplete();
	}

	private void StepComplete()
	{
		dropdownList.Peep();
		StartCoroutine(ClearObjectives());
	}

	private IEnumerator ClearObjectives()
	{
		yield return new WaitForSeconds(3f);
		objectiveSpawner.ClearObjectives();
		ChangeHighlight(false);
	}
}