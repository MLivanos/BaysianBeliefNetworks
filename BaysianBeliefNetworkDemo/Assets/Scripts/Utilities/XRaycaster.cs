using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class XRaycaster : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private List<GraphicRaycaster> raycasters = new();

    private readonly List<RaycastResult> combinedResults = new();

    void Update()
    {
        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        combinedResults.Clear();

        foreach (var raycaster in raycasters)
        {
            List<RaycastResult> results = new();
            raycaster.Raycast(pointerData, results);
            combinedResults.AddRange(results);
        }

        combinedResults.Sort((a, b) => a.distance.CompareTo(b.distance));

        foreach (var result in combinedResults)
        {
            if (result.gameObject.GetComponent<XRaycastStopper>() != null) break;

            var target = result.gameObject.GetComponent<XRaycastTarget>();
            if (target != null) target.Hit();
        }
    }
}