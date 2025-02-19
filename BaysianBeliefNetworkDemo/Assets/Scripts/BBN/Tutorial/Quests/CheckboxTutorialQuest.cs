using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckboxTutorialQuest : TutorialQuest<bool>
{
    private Toggle checkbox;

    private enum Mode { Positive, Negative, Either, ToggleOff }
    [SerializeField] private Mode toggleMode;

    public override void OnInitialize()
    {
        checkbox = objectToAttach.GetComponent<Toggle>();
        if (checkbox == null)
        {
            Debug.LogError($"Attached object {objectToAttach.name} is not a Toggle!");
            return;
        }

        interactionListener = (isOn) =>
        {
            if (toggleMode == Mode.ToggleOff && !isOn) HandleInteraction(isOn);
            else if (toggleMode != Mode.ToggleOff && isOn) HandleInteraction(isOn);
        };

        checkbox.onValueChanged.AddListener(interactionListener);
    }

    public override void HandleInteraction()
    {
        Debug.LogWarning("This overload should not be called directly!");
    }

    private void HandleInteraction(bool isOn)
    {
        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        switch (toggleMode)
        {
            case Mode.Positive:
                if (!isShiftPressed) Complete();
                break;

            case Mode.Negative:
                if (isShiftPressed) Complete();
                break;

            case Mode.Either:
                Complete();
                break;

            case Mode.ToggleOff:
                Complete();
                break;
        }
    }

    public override void Complete()
    {
        if (!CanComplete()) return;
        if (checkbox != null)
        {
            checkbox.onValueChanged.RemoveListener(interactionListener);
        }
        base.Complete();
    }
}