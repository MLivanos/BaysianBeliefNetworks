using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProbabilityDisplay : MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] private Node node;
    private TMP_InputField inputField;
    private TMP_Text inputText;

    private void Start()
    {
        inputField = GetComponent<TMP_InputField>() as TMP_InputField;
        inputText = inputField.textComponent;
        inputField.onValueChanged.AddListener(delegate {ChangeProbability(); });
    }

    private void ChangeProbability()
    {
        node.SetProbability(float.Parse(inputField.text), index);
    }

    public void SetValue(float newValue)
    {
        SetValue(newValue.ToString("0.000"));
    }

    public void SetValue(string newValue)
    {
        inputField.text = newValue;
    }

    public float GetValue()
    {
        return float.Parse(inputText.text);
    }
}
