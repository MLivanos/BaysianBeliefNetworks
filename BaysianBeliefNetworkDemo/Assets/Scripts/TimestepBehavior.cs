using UnityEngine;

public class TimestepBehavior : MonoBehaviour
{
    protected int step;
    public virtual void Step()
    {
        step++;
    }
}