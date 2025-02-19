using UnityEngine;

public abstract class TutorialQuestBase : MonoBehaviour
{
    [SerializeField] private string description;
    [SerializeField] protected GameObject objectToAttach;
    protected SuperQuest superQuestParent;
    protected TutorialStep parentStep;

    public virtual void Initialize(TutorialStep step)
    {
        parentStep = step;
        OnInitialize();
    }

    public abstract void OnInitialize();
    public abstract void HandleInteraction();
    public virtual void Complete()
    {
        if (parentStep != null) parentStep.OnQuestComplete(this);
        if (superQuestParent != null) superQuestParent.OnQuestComplete();
    }

    public string GetDescription()
    {
        return description;
    }

    public void SetSuperParent(SuperQuest superParent)
    {
        superQuestParent = superParent;
    }
}
