using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProbabilityDisplay : MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] private Node node;
    [SerializeField] private bool isNode=true;
    private TMP_InputField inputField;
    private TMP_Text inputText;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>() as TMP_InputField;
        inputText = inputField.textComponent;
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

    public int GetIndex()
    {
        return index;
    }

    public Node GetNode()
    {
        return node;
    }

    public TMP_InputField GetInputField()
    {
        return inputField;
    }
}
