using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine;

[System.Serializable]
public class TutorialTooltipMessage
{
    public string tooltipID;
    public string tooltipText;
    public Vector2 boxSize = new Vector2(200, 100);
    public Vector3 worldPositionOverride;
}


public class TutorialTooltipTrigger : MonoBehaviour, IXRaycastTargetScript
{
    public string nameID;

    public void OnDeepPointerEnter()
    {
        // Turn self off if tutorial is no longer in progress
        this.enabled = TutorialManager.Instance.HandleTooltipHoverEnter(nameID);
    }

    public void OnDeepPointerExit()
    {
        TutorialManager.Instance.HandleTooltipHoverExit();
    }
}