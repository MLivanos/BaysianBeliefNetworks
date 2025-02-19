using System.Collections.Generic;
using UnityEngine;

public class SuperQuest : TutorialQuestBase
{
	[SerializeField] private List<TutorialQuestBase> subQuests;
	private int questsCompleted = 0;

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

	public void OnQuestComplete()
	{
		if (++questsCompleted == subQuests.Count) Complete();
	}
}