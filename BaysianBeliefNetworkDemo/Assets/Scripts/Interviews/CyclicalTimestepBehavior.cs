using UnityEngine;

public class CyclicalTimestepBehavior : TimestepBehavior
{
    [SerializeField] private int cycleLength;
    protected int cycle = 0;

    public override void Step()
    {
        base.Step();
        if (step == cycleLength) Cycle();
    }

    public virtual void Cycle()
    {
        step = 0;
        cycle++;
    }
}