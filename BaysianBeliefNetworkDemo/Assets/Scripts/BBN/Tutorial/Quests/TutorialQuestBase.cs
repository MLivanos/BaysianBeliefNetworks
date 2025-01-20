using UnityEngine;

public abstract class TutorialQuestBase : MonoBehaviour
{
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
