using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProbabilityDisplay : MonoBehaviour
{
    private TMP_InputField inputField;
    private TMP_Text inputText;

    private void Start()
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
}
