using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverPanelToggler : MonoBehaviour
{
    [SerializeField] private RectTransform panelTransform;
    [SerializeField] private RectTransform buttonTransform;
    [SerializeField] private float timeMargin = 0.2f;

    private Coroutine hoverCoroutine;

    public void HandlePointerExit()
    {
        // Start a coroutine to handle delayed deactivation
        if (hoverCoroutine != null)
        {
            StopCoroutine(hoverCoroutine);
        }
        hoverCoroutine = StartCoroutine(TimeToHover());
    }

    public void HandlePointerEnter()
    {
        // Cancel any ongoing coroutine and activate the panel immediately
        if (hoverCoroutine != null)
        {
            StopCoroutine(hoverCoroutine);
            hoverCoroutine = null;
        }
        panelTransform.gameObject.SetActive(true);
    }

    private IEnumerator TimeToHover()
    {
        yield return new WaitForSeconds(timeMargin);

        // Check if the mouse is still over the button or panel
        if (RectTransformUtility.RectangleContainsScreenPoint(panelTransform, Input.mousePosition) ||
            RectTransformUtility.RectangleContainsScreenPoint(buttonTransform, Input.mousePosition))
        {
            panelTransform.gameObject.SetActive(true);
        }
        else
        {
            panelTransform.gameObject.SetActive(false);
        }
    }
}
