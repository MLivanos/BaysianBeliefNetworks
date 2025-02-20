using UnityEngine;
using UnityEngine.Events;

public abstract class TutorialQuest<T> : TutorialQuestBase
{
    protected UnityAction<T> interactionListener;
    protected GameObject GetAttachedObject() => objectToAttach;
}