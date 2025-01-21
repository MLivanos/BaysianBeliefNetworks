using UnityEngine;

public abstract class TutorialQuestBase : MonoBehaviour
{
    [SerializeField] private string description;
    [SerializeField] protected GameObject objectToAttach;
    protected TutorialStep parentStep;

    public void Initialize(TutorialStep step)
    {
        parentStep = step;
        OnInitialize();
    }

    public abstract void OnInitialize();
    public abstract void HandleInteraction();
    public virtual void Complete()
    {
        parentStep.OnQuestComplete(this);
    }

    public string GetDescription()
    {
        return description;
    }
}
