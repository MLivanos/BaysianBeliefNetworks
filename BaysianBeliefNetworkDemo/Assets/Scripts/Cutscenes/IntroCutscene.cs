using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IntroCutscene : CutsceneBehavior
{
    [Header("Intro Cutscene Specific Fields")]
    [SerializeField] protected List<string> soundtrack;
    [SerializeField] protected float fadeSoundTime;
    [SerializeField] protected bool continueTrack;

    public List<string> GetMusic()
    {
        return soundtrack;
    }

    public bool ShouldConinueTrack()
    {
        return continueTrack;
    }

    public float GetFadeSoundTime()
    {
        return fadeSoundTime;
    }
}