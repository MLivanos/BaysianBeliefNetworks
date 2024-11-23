using UnityEngine;

public class CyclicalTimestepBehavior : TimestepBehavior
{
    [SerializeField] private int cycleLength;
    public override void Step()
    {
        base.Step();
        step %= cycleLength;
    }
}