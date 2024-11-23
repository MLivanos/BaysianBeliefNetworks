using UnityEngine;

public class TimestepBehavior : MonoBehaviour
{
    protected int step;
    protected virtual void Step()
    {
        step++;
    }
}