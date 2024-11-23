using UnityEngine;

public class TimestepManager : MonoBehaviour
{
    private TimestepBehavior[] timesteps;

    private void Start()
    {
        timesteps = GetComponents<TimestepBehavior>();
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Step();
        }
    }

    private void Step()
    {
        foreach (TimestepBehavior timestep in timesteps)
        {
            timestep.Step();
        }
    }
}