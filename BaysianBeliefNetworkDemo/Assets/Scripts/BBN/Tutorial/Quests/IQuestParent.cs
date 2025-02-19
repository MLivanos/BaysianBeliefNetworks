public interface IQuestParent
{
    QuestOrderHandler QuestHandler { get; }
    void OnQuestComplete(TutorialQuestBase quest);
    bool CanCompleteQuest(TutorialQuestBase quest) => QuestHandler.CanCompleteQuest(quest);
}