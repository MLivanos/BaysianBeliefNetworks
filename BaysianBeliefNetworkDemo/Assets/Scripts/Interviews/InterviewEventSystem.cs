using UnityEngine;

public abstract class InterviewEventSystem : MonoBehaviour
{
    protected InterviewManager interviewManager;

    public void Initialize(InterviewManager manager)
    {
        interviewManager = manager;
    }
}