using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NodeHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject[] incomingArrowObjects;
    [SerializeField] private GameObject[] outgoingArrowObjects;
    [SerializeField] private Color incomingPulseColor = Color.white;
    [SerializeField] private Color outgoingPulseColor = Color.white;
    [SerializeField] private float pulseSpeed = 1.5f;

    private Color baseArrowColor;
    private Color baseLineColor;
    private List<RawImage> incomingArrows = new List<RawImage>();
    private List<RawImage> outgoingArrows = new List<RawImage>();
    private Coroutine[] pulseCoroutines = new Coroutine[2];
    private bool isHighlighted = false;

    private void Start()
    {
        AddChildrenToList(incomingArrowObjects, incomingArrows);
        AddChildrenToList(outgoingArrowObjects, outgoingArrows);
        baseArrowColor = new Color(0.012f, 0.894f, 1f, 1f);
        baseLineColor = new Color(0.012f, 0.894f, 1f, 0.784f);
    }

    private void AddChildrenToList(GameObject[] arrowObjects, List<RawImage> arrowList)
    {
        foreach(GameObject arrow in arrowObjects){
            foreach(Transform component in arrow.transform)
            {
                arrowList.Add(component.gameObject.GetComponent<RawImage>());
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHighlighted = true;
        pulseCoroutines[0] = StartCoroutine(PulseArrows(incomingPulseColor, incomingArrows));
        pulseCoroutines[1] = StartCoroutine(PulseArrows(outgoingPulseColor, outgoingArrows));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHighlighted = false;
        StopCoroutine(pulseCoroutines[0]);
        StopCoroutine(pulseCoroutines[1]);
        ResetArrowColors(incomingArrows);
        ResetArrowColors(outgoingArrows);
    }

    private void ResetArrowColors(List<RawImage> arrows)
    {
        foreach (RawImage element in arrows)
        {
            if(element.gameObject.name == "Line"){
                element.color = baseLineColor;
            }
            else{
                element.color = baseArrowColor;
            }
        }
    }

    private IEnumerator PulseArrows(Color pulseColor, List<RawImage> arrows)
    {
        while (isHighlighted)
        {
            float elapsedTime = 0f;
            while (elapsedTime < pulseSpeed)
            {
                float t = Mathf.PingPong(elapsedTime, pulseSpeed/2);
                Color lerpedColor = Color.Lerp(baseArrowColor, pulseColor, t);
                foreach (RawImage arrow in arrows)
                {
                    arrow.color = lerpedColor;
                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}
