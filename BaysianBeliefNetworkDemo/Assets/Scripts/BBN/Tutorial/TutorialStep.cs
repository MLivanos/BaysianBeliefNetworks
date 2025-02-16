using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialStep : MonoBehaviour
{
	[SerializeField] private List<TutorialQuestBase> quests;
	[SerializeField] private List<GameObject> stepObjects;
	[SerializeField] private List<string> messages;
	private TutorialManager tutorialManager;
	private ObjectiveSpawner objectiveSpawner;
    private DropdownList dropdownList;
    private TypewriterEffect typewriterEffect;
    private GameObject messagePanel;
    private Coroutine clickthroughText;
	private int questsCompleted = 0;
	private int messageID = 0;

	public void Initialize(TutorialManager manager)
	{
		tutorialManager = manager;
		InitializeMembers();
		ChangeHighlight(true);
		foreach(TutorialQuestBase quest in quests)
		{
			objectiveSpawner.AddObjective(quest.GetDescription());
			quest.Initialize(this);
		}
		dropdownList.Peep();
		clickthroughText = StartCoroutine(ClickThroughText());
		tutorialManager.BlockInteractions(true);
	}

	private void InitializeMembers()
	{
		objectiveSpawner = tutorialManager.GetObjectSpawner();
		dropdownList = tutorialManager.GetDropdownList();
		typewriterEffect = tutorialManager.GetTypeWriterEffect();
		messagePanel = tutorialManager.GetMessagePanel();
	}

	private IEnumerator ClickThroughText()
	{
		if (messages.Count >= 1)
		{
			messagePanel.SetActive(true);
			yield return null;
			typewriterEffect.UpdateText(messages[messageID++]);
		}
		while(messageID <= messages.Count)
		{
			if(Input.GetMouseButtonDown(0))
			{
				if(messageID == messages.Count) break;
				typewriterEffect.Clear();
				typewriterEffect.UpdateText(messages[messageID++]);
			}
			yield return null;
		}
		tutorialManager.BlockInteractions(false);
		messagePanel.SetActive(false);
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
		messageID = 0;
		dropdownList.Peep();
	}

	public void ClearObjectives()
	{
		objectiveSpawner.ClearObjectives();
		ChangeHighlight(false);
	}

	public void CloseMessages()
	{
		StopCoroutine(clickthroughText);
		messagePanel.SetActive(false);
	}
}