using TMPro;
using UnityEngine;
using System.Linq;

public class SampleInputHandler : MonoBehaviour
{
    [SerializeField] private Graph graph;
    [SerializeField] private TMP_InputField inputField;
    private NotificationManager notificationManager;

    private readonly string truePassword = "i am beautiful";
    private readonly string fakePassword = "you are beautiful";

    private void Start()
    {
        notificationManager = FindObjectOfType<NotificationManager>();
        if (inputField == null)
            inputField = GetComponent<TMP_InputField>();
    }

    public void OnEditEnd()
    {
        string userInput = inputField.text.Trim().ToLower();

        if (truePassword == userInput)
        {
            notificationManager.ShowNotification("Password Accepted", "Compute Time Override Engaged");
            inputField.text = "Override Successful";
            GameManager.instance.OverrideTime();
            return;
        }

        if (userInput == fakePassword)
        {
            notificationManager.ShowNotification("thank you", "klt qrok fq fkql x pbic xccfojxqflk", true);
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