using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialStep : MonoBehaviour
{
	// Eventually let TM do this
	
	[SerializeField] private List<TutorialQuestBase> quests;
	[SerializeField] private List<GameObject> stepObjects;
	private TutorialManager tutorialManager;
	private ObjectiveSpawner objectiveSpawner;
    private DropdownList dropdownList;
	private int questsCompleted = 0;

	public void Initialize(TutorialManager manager)
	{
		tutorialManager = manager;
		objectiveSpawner = tutorialManager.GetObjectSpawner();
		dropdownList = tutorialManager.GetDropdownList();
		ChangeHighlight(true);
		foreach(TutorialQuestBase quest in quests)
		{
			objectiveSpawner.AddObjective(quest.GetDescription());
			quest.Initialize(this);
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
	}

	public void ClearObjectives()
	{
		objectiveSpawner.ClearObjectives();
		ChangeHighlight(false);
	}
}