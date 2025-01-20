using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextFieldTutorialQuest : TutorialQuest<string>
{
    private TMP_InputField inputField;

    [SerializeField] private string targetText;

    public override void OnInitialize()
    {
        inputField = objectToAttach.GetComponent<TMP_InputField>();
        if (inputField == null)
        {
            Debug.LogError("No InputField attached to the object!");
            return;
        }

        interactionListener = HandleInteraction;
        inputField.onEndEdit.AddListener((text) => interactionListener.Invoke(text));
    }

    public override void HandleInteraction()
    {
        Debug.LogWarning("This overload should not be called directly!");
    }

    public void HandleInteraction(string inputText)
    {
        if (inputText.Trim() == targetText)
        {
            Complete();
        }
    }

    public override void Complete()
    {
        Debug.Log("Text Field Quest Complete!");
        inputField.onEndEdit.RemoveAllListeners();
    }
}
