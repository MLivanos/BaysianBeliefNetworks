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
    private List<float> originalValues;
    private List<float> currentValues;
    private bool modifying;
    
    private void Start()
    {
        graph = GameObject.Find("Graph").GetComponent<Graph>();
        node = displays[0].GetNode();
        for(int i=0; i<displays.Count; i++)
        {
            ProbabilityDisplay display = displays[i];
            Slider slider = sliders[i];
            sliderToIndex.Add(sliders[i], i);
            displayToIndex.Add(displays[i], i);
            sliders[i].onValueChanged.AddListener(value => ChangeDisplay(slider, value));
            displays[i].GetInputField().onEndEdit.AddListener(value => ChangeSlider(display, value));
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


    private void ChangeDisplay(Slider slider, float value)
    {
        RunWithModificationLock(() =>
        {
            int index = sliderToIndex[slider];
            displays[index].SetValue(value);
            ChangeProbability(value, displays[index].GetIndex());
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
                ChangeProbability(parsedValue, display.GetIndex());
            }
        });
    }

    private void ChangeProbability(float value, int index)
    {
        node.SetProbability(value, index);
    }
}
