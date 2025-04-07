using UnityEngine;

public class PlayOnHover : MonoBehaviour, IXRaycastTargetScript
{
	[SerializeField] private string soundEffect = "Hover";
	[SerializeField] private bool playOnEnter = true;
	[SerializeField] private bool playOnExit;
	private AudioManager audioManager;

	public void Start()
	{
		audioManager = AudioManager.instance;
	}

	public void OnDeepPointerEnter()
    {
        if (playOnEnter && audioManager != null) audioManager.PlayEffect(soundEffect);
    }

    public void OnDeepPointerExit()
    {
        if (playOnExit && audioManager != null) audioManager.PlayEffect(soundEffect);
    }
}