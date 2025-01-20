using UnityEngine;
using UnityEngine.Events;

public abstract class TutorialQuest<T> : TutorialQuestBase
{
    [SerializeField] protected GameObject objectToAttach;
    protected UnityAction<T> interactionListener;

    protected GameObject GetAttachedObject() => objectToAttach;
}