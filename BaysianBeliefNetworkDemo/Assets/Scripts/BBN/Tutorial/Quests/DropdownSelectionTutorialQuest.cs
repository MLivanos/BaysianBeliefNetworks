using UnityEngine;
using UnityEngine.UI;

public class DropdownSelectionTutorialQuest : TutorialQuest<int>
{
    private Dropdown dropdown;

    [SerializeField] private string targetOption;

    public override void OnInitialize()
    {
        dropdown = objectToAttach.GetComponent<Dropdown>();
        if (dropdown == null)
        {
            Debug.LogError("No Dropdown attached to the object!");
            return;
        }

        interactionListener = HandleInteraction;
        dropdown.onValueChanged.AddListener((index) => interactionListener.Invoke(index));
    }

    public override void HandleInteraction()
    {
        Debug.LogWarning("This overload should not be called directly!");
    }

    public void HandleInteraction(int selectedIndex)
    {
        if (dropdown.options[selectedIndex].text == targetOption)
        {
            Complete();
        }
    }

    public override void Complete()
    {
        Debug.Log("Dropdown Selection Quest Complete!");
        dropdown.onValueChanged.RemoveAllListeners();
    }
}
