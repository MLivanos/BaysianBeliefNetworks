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
    [SerializeField] private List<Image> markovBlanket;
    [SerializeField] private GameObject legend;
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
        ToggleObjects(true);
        
        pulseCoroutines.Add(StartCoroutine(Pulse(new Color(1f,1f,1f,0.69f), new Color(1f,1f,1f,0.1f), markovBlanket, 1)));
        foreach (EdgeHighlightSettings settings in edgeHighlightSettings)
        {
            pulseCoroutines.Add(StartCoroutine(Pulse(baseArrowColor, incomingPulseColor, settings.incoming, settings.generation)));
            pulseCoroutines.Add(StartCoroutine(Pulse(baseArrowColor, outgoingPulseColor, settings.outgoing, settings.generation)));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToggleObjects(false);

        foreach (Coroutine routine in pulseCoroutines)
        {
            StopCoroutine(routine);
        }
        ResetArrowColors();
        ResetArrowColors();
        foreach (Image markovHighlight in markovBlanket)
        {
            markovHighlight.color = Color.clear;
        }
    }

    private void ToggleObjects(bool toggleOn)
    {
        isHighlighted = toggleOn;
        highlight.SetActive(toggleOn);
        legend.SetActive(toggleOn);
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

    private IEnumerator Pulse(Color baseColor, Color pulseColor, List<Image> imageList, int generation)
    {
        List<MaskableGraphic> elements = new List<MaskableGraphic>();
        elements.AddRange(imageList);
        yield return Pulse(baseColor, pulseColor, elements, generation);
    }

    private IEnumerator Pulse(Color baseColor, Color pulseColor, List<RawImage> imageList, int generation)
    {
        List<MaskableGraphic> elements = new List<MaskableGraphic>();
        elements.AddRange(imageList);
        yield return Pulse(baseColor, pulseColor, elements, generation);
    }

    private IEnumerator Pulse(Color baseColor, Color pulseColor, List<MaskableGraphic> elements, int generation)
    {
        while (isHighlighted)
        {
            float elapsedTime = 0f;
            while (elapsedTime < pulseSpeed)
            {
                float t = Mathf.PingPong(elapsedTime, pulseSpeed/2);
                Color lerpedColor = Color.Lerp(baseColor, pulseColor, t/generation);
                foreach (MaskableGraphic element in elements)
                {
                    element.color = lerpedColor;
                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}