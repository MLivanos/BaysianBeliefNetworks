using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperQuest : TutorialQuestBase, IQuestParent
{
	[SerializeField] private List<TutorialQuestBase> subQuests;
	[SerializeField] private bool isOrdered;
	private QuestOrderHandler questOrderHandler;
	private int questsCompleted = 0;

	public QuestOrderHandler QuestHandler => questOrderHandler;

	private void Start()
	{
		questOrderHandler = new QuestOrderHandler(isOrdered, subQuests);
	}

	public override void OnInitialize()
    {
        foreach(TutorialQuestBase quest in subQuests)
        {
        	quest.OnInitialize();
        	quest.SetSuperParent(this);
        }
    }

    public override void HandleInteraction()
    {
    	return;
    }

	public void OnQuestComplete(TutorialQuestBase quest)
	{
		if (++questsCompleted == subQuests.Count) StartCoroutine(WaitTillCanComplete());
	}

	private IEnumerator WaitTillCanComplete()
	{
		while(!CanComplete())
		{
			yield return new WaitForSeconds(0.1f);
		}
		Complete();
	}

	public override void DestroyQuest()
	{
		foreach(TutorialQuestBase quest in subQuests)
		{
			quest.DestroyQuest();
		}
		base.DestroyQuest();
	}
}