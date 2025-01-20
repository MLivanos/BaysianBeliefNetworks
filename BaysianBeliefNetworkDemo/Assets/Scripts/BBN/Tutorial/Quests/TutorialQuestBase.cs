using UnityEngine;

public abstract class TutorialQuestBase : MonoBehaviour
{
    [SerializeField] protected GameObject objectToAttach;
    protected TutorialStep parentStep;

    public void Initialize(TutorialStep step)
    {
        parentStep = step;
        OnInitialize();
    }

    public abstract void OnInitialize();
    public abstract void HandleInteraction();
    public abstract void Complete();
}
