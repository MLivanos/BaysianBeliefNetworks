using TMPro;
using UnityEngine;
using System.Linq;

public class SampleInputHandler : MonoBehaviour
{
    [SerializeField] private Graph graph;
    [SerializeField] private TMP_InputField inputField;

    private readonly string truePassword = "i am beautiful";
    private readonly string fakePassword = "you are beautiful";

    private void Start()
    {
        if (inputField == null)
            inputField = GetComponent<TMP_InputField>();
    }

    public void OnEditEnd()
    {
        string userInput = inputField.text.Trim().ToLower();

        if (truePassword == userInput)
        {
            inputField.text = "Access Code Accepted";
            return;
        }

        if (userInput == fakePassword)
        {
            inputField.text = "Error: See message";
            return;
        }

        string digitsOnly = new string(userInput.Where(char.IsDigit).ToArray());

        if (int.TryParse(digitsOnly, out int parsed))
        {
            graph.SetNumberOfSamples(parsed);
            inputField.text = FormatWithCommas(parsed);
        }
        else
        {
            inputField.text = "Please enter a number!";
        }
    }

    private string FormatWithCommas(int number)
    {
        return number.ToString("N0");
    }
}