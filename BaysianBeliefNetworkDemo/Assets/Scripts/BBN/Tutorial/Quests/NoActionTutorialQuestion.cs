using UnityEngine;

public class NoActionTutorialQuestion : TutorialQuestBase
{
	public override void OnInitialize()
	{
		Complete();
	}

    public override void HandleInteraction()
    {
        Debug.LogWarning("This overload should not be called directly!");
    }
}