using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProbabilityValue : MonoBehaviour
{
    [SerializeField] private List<ProbabilityDisplay> displays;
    [SerializeField] private List<Slider> sliders;
    private Dictionary<Slider, int> sliderToIndex = new Dictionary<Slider, int>();
    private Dictionary<ProbabilityDisplay, int> displayToIndex = new Dictionary<ProbabilityDisplay, int>();
    private Graph graph;
    private Node node;
    private List<float> originalValues = new List<float>();
    private List<float> currentValues = new List<float>();
    private bool modifying;
    
    private void Start()
    {
        graph = Graph.instance;
        node = displays[0].GetNode();
        for(int i=0; i<displays.Count; i++)
        {
            ProbabilityDisplay display = displays[i];
            Slider slider = sliders[i];
            sliderToIndex.Add(sliders[i], i);
            displayToIndex.Add(displays[i], i);
            originalValues.Add(sliders[i].value);
            currentValues.Add(sliders[i].value);
            sliders[i].onValueChanged.AddListener(value => ChangeDisplay(slider, value));
            displays[i].GetInputField().onEndEdit.AddListener(value => ChangeSlider(display, value));
        }
        StartCoroutine(ResetSliders());
    }

    private IEnumerator ResetSliders()
    {
        yield return null;
        for(int i=0; i<displays.Count; i++)
        {
            ChangeSlider(displays[i], displays[i].GetValue().ToString("0.000"));
        }
    }

    private void RunWithModificationLock(System.Action action)
    {
        if (modifying) return;
        modifying = true;
        action();
        modifying = false;
        graph.ClearSamples();
    }

    public void UpdateProbability(int index, float value)
    {
        ChangeDisplay(sliders[index], value);
        ChangeSlider(displays[index], value.ToString());
    }

    public float GetProbability(int index)
    {
        return currentValues[index];
    }

    public void ReturnToOriginalValue(int index)
    {
        UpdateProbability(index, originalValues[index]);
    }

    public float GetOriginalValue(int index)
    {
        return originalValues[index];
    }

    private void ChangeDisplay(Slider slider, float value)
    {
        RunWithModificationLock(() =>
        {
            int index = sliderToIndex[slider];
            displays[index].SetValue(value);
            UpdateNodeProbability(value, displays[index].GetIndex());
        });
    }

    private void ChangeSlider(ProbabilityDisplay display, string value)
    {
        RunWithModificationLock(() =>
        {
            int index = displayToIndex[display];
            if (float.TryParse(value, out float parsedValue))
            {
                sliders[index].value = parsedValue;
                UpdateNodeProbability(parsedValue, display.GetIndex());
            }
        });
    }

    private void UpdateNodeProbability(float value, int index)
    {
        node.SetProbability(value, index);
    }
}
