using UnityEngine;
using UnityEngine.UI;

public class ButtonPressTutorialQuest : TutorialQuestBase
{
    private Button button;

    public override void OnInitialize()
    {
        button = objectToAttach.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("No Button attached to the object!");
            return;
        }

        button.onClick.AddListener(HandleInteraction);
    }

    public override void HandleInteraction()
    {
        Complete();
    }

    public override void Complete()
    {
        button.onClick.RemoveListener(HandleInteraction);
        base.Complete();
    }
}