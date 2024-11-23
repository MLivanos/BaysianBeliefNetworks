using UnityEngine;

public class CyclicalTimestepBehavior : TimestepBehavior
{
    [SerializeField] private int cycleLength;
    protected override void Step()
    {
        base.Step();
        step %= cycleLength;
    }
}