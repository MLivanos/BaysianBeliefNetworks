using System;
using UnityEngine;

[System.Serializable]
public class ClockTime
{
    public int hour;
    public int minute;

    public (float, float) GetClockhandAngles()
    {
        return ((float)((hour%12) + (float)minute/60)*30 + (float)minute/60, minute*6);
    }
}

public class ClockTimestep : CyclicalTimestepBehavior
{
    [SerializeField] private ClockTime[] times;
    [SerializeField] private Transform hourHand;
    [SerializeField] private Transform minuteHand;
    [SerializeField] private float hourOffset;
    [SerializeField] private float minuteOffset;
    [SerializeField] private float minuteRandomness;

    private void Start()
    {
        SetTime();
        ModifyTime();
    }

    public override void Step()
    {
        base.Step();
        SetTime();
        ModifyTime();
    }

    private void SetTime()
    {
        (float, float) clockhandAngles = times[step].GetClockhandAngles();
        
        Vector3 hourHandAngle = hourHand.eulerAngles;
        Vector3 minuteHandAngle = minuteHand.eulerAngles;

        hourHandAngle.z = clockhandAngles.Item1 + hourOffset;
        minuteHandAngle.z = clockhandAngles.Item2 + minuteOffset;

        hourHand.eulerAngles = hourHandAngle;
        minuteHand.eulerAngles = minuteHandAngle;
    }

    private void ModifyTime()
    {
        int newMinute = times[step].minute + (int)UnityEngine.Random.Range(-minuteRandomness,minuteRandomness);
        int overflow = newMinute >= 60 ? 1 : newMinute < 0 ? -1 : 0;
        newMinute %= 60;
        times[step].minute = newMinute;
        times[step].hour = times[step].hour + overflow;
    }
}