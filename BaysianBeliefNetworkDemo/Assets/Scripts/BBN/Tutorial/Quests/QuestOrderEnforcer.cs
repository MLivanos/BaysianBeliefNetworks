using System.Collections.Generic;
using UnityEngine;

public class QuestOrderHandler
{
    private bool isOrdered;
    private List<TutorialQuestBase> quests;
    private bool[] questsCompleted;

    public QuestOrderHandler(bool ordered, List<TutorialQuestBase> questList)
    {
        isOrdered = ordered;
        quests = questList;
        questsCompleted = new bool[quests.Count];
    }

    public bool CanCompleteQuest(TutorialQuestBase quest)
    {
    	if (!isOrdered) return true;

        int questIndex = quests.IndexOf(quest);
        if (questIndex == -1)
        {
            Debug.LogWarning("Quest not found in list!");
            return false;
        }

        Debug.Log(questIndex);
        for (int i = 0; i < questIndex; i++)
        {
        	Debug.Log(questsCompleted[i]);
            if (!questsCompleted[i]) return false;
        }

        questsCompleted[questIndex] = true;
        return true;
    }
}