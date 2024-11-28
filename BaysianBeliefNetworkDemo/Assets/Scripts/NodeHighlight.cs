using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class EdgeHighlightSettings
{
    public List<RawImage> incoming;
    public List<RawImage> outgoing;
    public int generation = 1;
}

public class NodeHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private EdgeHighlightSettings[] edgeHighlightSettings = new EdgeHighlightSettings[1];
    [SerializeField] private Color incomingPulseColor = Color.white;
    [SerializeField] private Color outgoingPulseColor = Color.white;
    [SerializeField] private float pulseSpeed = 1.5f;

    private Color baseArrowColor;
    private Color baseLineColor;
    private GameObject highlight;
    private List<Coroutine> pulseCoroutines = new List<Coroutine>();
    private bool isHighlighted = false;

    private void Start()
    {
        highlight = transform.Find("Highlight").gameObject;
        highlight.SetActive(false);
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
        highlight.SetActive(true);
        foreach (EdgeHighlightSettings settings in edgeHighlightSettings)
        {
            pulseCoroutines.Add(StartCoroutine(PulseArrows(incomingPulseColor, settings.incoming, settings.generation)));
            pulseCoroutines.Add(StartCoroutine(PulseArrows(outgoingPulseColor, settings.outgoing, settings.generation)));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHighlighted = false;
        highlight.SetActive(false);
        foreach (Coroutine routine in pulseCoroutines)
        {
            StopCoroutine(routine);
        }
        ResetArrowColors();
        ResetArrowColors();
    }

    private void ResetArrowColors()
    {
        foreach (EdgeHighlightSettings settings in edgeHighlightSettings)
        {
            foreach (RawImage element in (settings.incoming).Concat(settings.outgoing).ToList())
            {
                if(element.gameObject.name == "Line") element.color = baseLineColor;
                else element.color = baseArrowColor;
            }
        }
    }

    private IEnumerator PulseArrows(Color pulseColor, List<RawImage> arrows, int generation)
    {
        while (isHighlighted)
        {
            float elapsedTime = 0f;
            while (elapsedTime < pulseSpeed)
            {
                float t = Mathf.PingPong(elapsedTime, pulseSpeed/2);
                Color lerpedColor = Color.Lerp(baseArrowColor, pulseColor, t/generation);
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
