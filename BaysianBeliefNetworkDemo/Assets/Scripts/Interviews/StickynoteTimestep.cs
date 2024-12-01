using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StickynoteBatch
{
    [SerializeField] public List<GameObject> batch;
}

public class StickynoteTimestep : TimestepBehavior
{
    [SerializeField] private List<StickynoteBatch> stickynotes;

    public override void Step()
    {
        base.Step();
        if (step >= stickynotes.Count) return;
        foreach (GameObject note in stickynotes[step].batch)
        {
            note.SetActive(true);
        }
    }

}