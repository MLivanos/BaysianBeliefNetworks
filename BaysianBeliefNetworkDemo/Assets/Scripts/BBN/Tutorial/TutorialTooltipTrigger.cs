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


public class TutorialTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string nameID;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Turn self off if tutorial is no longer in progress
        this.enabled = TutorialManager.Instance.HandleTooltipHoverEnter(nameID);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TutorialManager.Instance.HandleTooltipHoverExit();
    }
}