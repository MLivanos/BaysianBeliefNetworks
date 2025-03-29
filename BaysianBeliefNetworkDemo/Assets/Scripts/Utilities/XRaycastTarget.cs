using System.Collections;
using UnityEngine;

public class XRaycastTarget : MonoBehaviour
{
    [SerializeField] private float hitDelay = 0.1f;
    private bool isHit = false;
    private Coroutine hitCoroutine = null;

    /// <summary>
    /// Called by the centralized DeepRaycaster when this object is hit.
    /// </summary>
    public void Hit()
    {
        if (!isHit)
        {
            isHit = true;
            TriggerPointerEnter();
        }

        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
        }
        hitCoroutine = StartCoroutine(HandleHitTimer());
    }

    /// <summary>
    /// Waits for a short duration; if no additional Hit() occurs, then the pointer is considered to have exited.
    /// </summary>
    private IEnumerator HandleHitTimer()
    {
        yield return new WaitForSeconds(hitDelay);
        isHit = false;
        TriggerPointerExit();
        hitCoroutine = null;
    }

    private void TriggerPointerEnter()
    {
        foreach (var target in GetComponents<IXRaycastTargetScript>())
        {
            target.OnDeepPointerEnter();
        }
    }

    private void TriggerPointerExit()
    {
        foreach (var target in GetComponents<IXRaycastTargetScript>())
        {
            target.OnDeepPointerExit();
        }
    }
}