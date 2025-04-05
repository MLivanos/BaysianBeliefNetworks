using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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

    private void Start()
    {
        node.AddDisplay(this);
        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        SetValue(node.JointProbabilityDistribution()[index]);
    }

    public void SetValue(float newValue)
    {
        SetValue(newValue.ToString("0.###"));
    }

    public void SetValue(string newValue)
    {
        inputField.text = newValue;
    }

    public float GetValue()
    {
        string cleanText = new string(inputText.text
            .Where(c => char.IsDigit(c) || c == '.' || c == '-' || c == '+')
            .ToArray());
        return float.Parse(cleanText);
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
