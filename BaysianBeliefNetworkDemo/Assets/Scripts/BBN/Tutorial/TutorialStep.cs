using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialStep : MonoBehaviour, IQuestParent
{
	[SerializeField] private List<TutorialQuestBase> quests;
	[SerializeField] private List<GameObject> stepObjects;
	[SerializeField] private List<string> messages;
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
    private Image advanceButton;
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
		tutorialManager.BlockInteractions(true);
	}

	private void InitializeMembers()
	{
		objectiveSpawner = tutorialManager.GetObjectSpawner();
		dropdownList = tutorialManager.GetDropdownList();
		typewriterEffect = tutorialManager.GetTypeWriterEffect();
		messagePanel = tutorialManager.GetMessagePanel();
		completionText = tutorialManager.GetCompletionText();
		advanceButton = tutorialManager.GetAdvanceButton();
		advanceButton.color = new Color32(0xA5, 0xA5, 0xA5, 160);
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
		stepComplete = ++questsCompleted == quests.Count;
		if (completionRoutine != null) StopCoroutine(completionRoutine);
		completionRoutine = StartCoroutine(FadeInCompletionMessage(quest.GetDescription()));
		objectiveSpawner.CompleteQuest(quests.FindIndex(q => q == quest));
		if (stepComplete) StepComplete();
	}

	private void StepComplete()
	{
		messageID = 0;
		advanceButton.color = new Color32(0xFF, 0xFF, 0xFF, 255);
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
}