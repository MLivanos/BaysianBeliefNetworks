using UnityEngine;

public class TimestepManager : MonoBehaviour
{
    private InterviewManager interviewManager;
    private TimestepBehavior[] timesteps;

    public void Initialize(InterviewManager manager)
    {
        interviewManager = manager;
    }

    private void Start()
    {
        timesteps = GetComponents<TimestepBehavior>();
    }

    public void Step()
    {
        foreach (TimestepBehavior timestep in timesteps)
        {
            timestep.Step();
        }
        interviewManager.Advance();
    }
}