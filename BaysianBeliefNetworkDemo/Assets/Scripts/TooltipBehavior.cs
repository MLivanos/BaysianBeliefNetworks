using UnityEngine;

public class TooltipBehavior : MonoBehaviour
{
    [SerializeField] private RectTransform tooltipPanel;
    [SerializeField] private Vector2 offset;

    public void ShowTooltip()
    {
        tooltipPanel.gameObject.SetActive(true);
        //PositionTooltip();
    }

    public void HideTooltip()
    {
        tooltipPanel.gameObject.SetActive(false);
    }

    public void PositionTooltip(Vector2 nodePosition, Vector2 mousePosition)
    {
        // Get the RectTransform of the tooltip's parent canvas
        RectTransform canvasRect = tooltipPanel.parent.parent as RectTransform;

        // Get the canvas size (assuming the canvas is properly scaled)
        Vector2 canvasSize = canvasRect.sizeDelta;

        // Get the size of the tooltip panel
        Vector2 panelSize = tooltipPanel.sizeDelta;

        // Start by positioning the tooltip on the right of the node
        Vector2 tooltipPosition = new Vector2(
            nodePosition.x + canvasSize.x,
            mousePosition.y + canvasSize.y
        );

        // Adjust for offscreen conditions
        if (tooltipPosition.x + panelSize.x > canvasSize.x) // Going off the right edge
        {
            tooltipPosition.x = nodePosition.x - panelSize.x - offset.x; // Move to the left
        }
        else if (tooltipPosition.x < 0) // Going off the left edge
        {
            tooltipPosition.x = offset.x; // Adjust to stay within screen bounds
        }

        if (tooltipPosition.y + panelSize.y > canvasSize.y) // Going off the top edge
        {
            tooltipPosition.y = canvasSize.y - panelSize.y - offset.y;
        }
        else if (tooltipPosition.y < 0) // Going off the bottom edge
        {
            tooltipPosition.y = offset.y;
        }

        // Apply the calculated position
        tooltipPanel.anchoredPosition = tooltipPosition;
}

}