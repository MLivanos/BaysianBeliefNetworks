using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TutorialMessage
{
	[SerializeField] private string message;
	[SerializeField] private List<GameObject> messageObjects;

	public string Message=>message;

	public void ToggleObjects(bool toggleOn)
	{
		foreach(GameObject messageObject in messageObjects)
		{
			messageObject.SetActive(toggleOn);
		}
	}
}

public class TutorialStep : MonoBehaviour, IQuestParent
{
	[SerializeField] private List<TutorialQuestBase> quests;
	[SerializeField] private List<GameObject> stepObjects;
	[SerializeField] private List<GameObject> permanantStepObjects;
	[SerializeField] private List<TutorialMessage> tutorialMessages;
	[SerializeField] private List<List<GameObject>> messageObjects;
	[SerializeField] private List<TutorialTooltipMessage> tooltipMessages;
	[SerializeField] private bool isOrdered;
	private QuestOrderHandler questOrderHandler;
	private AudioManager audioManager;
	private TutorialManager tutorialManager;
	private ObjectiveSpawner objectiveSpawner;
    private DropdownList dropdownList;
    private TypewriterEffect typewriterEffect;
    private GameObject messagePanel;
    private Coroutine clickthroughText;
    private FadableTextMeshPro completionText;
    private Button advanceButton;
	private int questsCompleted = 0;
	private int messageID = 0;
	private Coroutine completionRoutine;
	private bool stepComplete = false;

	public QuestOrderHandler QuestHandler => questOrderHandler;

	private void Start()
	{
		questOrderHandler = new QuestOrderHandler(isOrdered, quests);
	}

	public void Initialize(TutorialManager manager)
	{
		audioManager = AudioManager.instance;
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
		ShowIncrementalObjects();
		tutorialManager.BlockInteractions(true);
	}

	private void ShowIncrementalObjects()
	{
		foreach(GameObject obj in permanantStepObjects)
		{
			obj.SetActive(true);
		}
	}

	private void InitializeMembers()
	{
		objectiveSpawner = tutorialManager.GetObjectSpawner();
		dropdownList = tutorialManager.GetDropdownList();
		typewriterEffect = tutorialManager.GetTypeWriterEffect();
		messagePanel = tutorialManager.GetMessagePanel();
		completionText = tutorialManager.GetCompletionText();
		advanceButton = tutorialManager.GetAdvanceButton();
		advanceButton.interactable = false;
	}

	private IEnumerator ClickThroughText()
	{
		if (tutorialMessages.Count == 0)
			yield break;

		messagePanel.SetActive(true);
		yield return null;

		messageID = 0;
		while (messageID <= tutorialMessages.Count)
		{
			if (messageID-1 >= 0) tutorialMessages[messageID-1].ToggleObjects(false);
			if (messageID < tutorialMessages.Count)
			{
				tutorialMessages[messageID].ToggleObjects(true);
				typewriterEffect.Clear();
				typewriterEffect.UpdateText(tutorialMessages[messageID].Message);
			}
			messageID++;
			yield return new WaitForSeconds(0.05f);
			yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
		}

		messagePanel.SetActive(false);
		tutorialManager.BlockInteractions(false);
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
		stepComplete = ++questsCompleted == quests.Count;
		if (completionRoutine != null) StopCoroutine(completionRoutine);
		completionRoutine = StartCoroutine(FadeInCompletionMessage(quest.GetDescription()));
		objectiveSpawner.CompleteQuest(quests.FindIndex(q => q == quest));
		if (stepComplete) StepComplete();
	}

	private void StepComplete()
	{
		messageID = 0;
		advanceButton.interactable = true;
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
		// Close out any active message objects
		for (int i=0; i<tutorialMessages.Count; i++) tutorialMessages[i].ToggleObjects(false);
		tutorialManager.BlockInteractions(false);
		messagePanel.SetActive(false);
	}

	public IEnumerator FadeInCompletionMessage(string questText)
	{
		if (stepComplete) audioManager.PlayEffect("Success2");
		else audioManager.PlayEffect("Success");
		completionText.SetText("Completed:\n" + questText);
		completionText.SetAlpha(0f);
		completionText.FadeIn(0.5f);
		yield return new WaitForSeconds(2f);
		completionText.FadeOut(0.5f);
		completionRoutine = null;
	}

	public void DestroyQuests()
	{
		foreach(TutorialQuestBase quest in quests)
		{
			quest.DestroyQuest();
		}
	}

	public TutorialTooltipMessage FindTooltip(string triggerName)
    {
        foreach (var tooltip in tooltipMessages)
        {
            if (tooltip.tooltipID == triggerName)
                return tooltip;
        }
        return null;
    }
}