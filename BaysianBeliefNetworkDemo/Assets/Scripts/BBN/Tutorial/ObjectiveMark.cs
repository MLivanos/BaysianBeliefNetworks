using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveMark : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private Toggle checkmark;

    public void SetText(string description)
    {
        text.text = description;
    }

    public void Check()
    {
        checkmark.isOn = true;
    }
}
