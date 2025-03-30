using System.Collections;
using UnityEngine;

public class XRaycastTarget : MonoBehaviour
{
    private static XRaycastTarget currentTarget = null;

    [SerializeField] private float hitDelay = 0.1f;
    private bool isHit = false;
    private Coroutine hitCoroutine = null;

    /// <summary>
    /// Called by the centralized raycaster when this object is hit.
    /// </summary>
    public void Hit()
    {
        if (currentTarget != this)
        {
            if (currentTarget != null)
            {
                currentTarget.ForceExit();
            }
            currentTarget = this;
        }

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

        // Only trigger exit if we're still the active target.
        if (currentTarget == this)
        {
            isHit = false;
            TriggerPointerExit();
            currentTarget = null;
        }
        hitCoroutine = null;
    }

    /// <summary>
    /// Immediately force an exit (cancel any timer and trigger exit event).
    /// </summary>
    public void ForceExit()
    {
        if (hitCoroutine != null)
        {
            StopCoroutine(hitCoroutine);
            hitCoroutine = null;
        }
        if (isHit)
        {
            isHit = false;
            TriggerPointerExit();
        }
        if (currentTarget == this)
        {
            currentTarget = null;
        }
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