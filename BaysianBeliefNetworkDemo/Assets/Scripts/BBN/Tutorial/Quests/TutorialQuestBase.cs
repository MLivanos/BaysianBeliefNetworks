using UnityEngine;

public abstract class TutorialQuestBase : MonoBehaviour
{
    [SerializeField] private string description;
    [SerializeField] protected GameObject objectToAttach;
    protected IQuestParent parent;

    public virtual void Initialize(TutorialStep step)
    {
        parent = step;
        OnInitialize();
    }

    public void SetSuperParent(SuperQuest superParent)
    {
        parent = superParent;
    }

    public abstract void OnInitialize();
    public abstract void HandleInteraction();
    public virtual void Complete()
    {
        parent.OnQuestComplete(this);
    }

    public bool CanComplete()
    {
        return parent.CanCompleteQuest(this);
    }

    public string GetDescription()
    {
        return description;
    }
}
