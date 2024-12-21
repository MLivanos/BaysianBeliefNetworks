using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class EndGameSlide : MonoBehaviour
{
	[SerializeField] protected List<string> tracks;
	[SerializeField] protected List<GameObject> scenes;
	[SerializeField] protected List<string> texts;
	protected AudioManager audioManager;
	protected Transform cameraTransform;
	protected int code;

	protected abstract IEnumerator PlayScene();
    protected abstract IEnumerator ExitTransition();

	private void Start()
	{
		audioManager = AudioManager.instance;
	}

	public void Setup(int sceneCode)
	{
		code = sceneCode;
		audioManager.PlayMusic(tracks[code]);
		scenes[code].SetActive(true);
		cameraTransform.parent = scenes[code].transform.Find("CameraMark");
	}

	public IEnumerator Play()
    {
        yield return PlayScene();
    }

    public IEnumerator Exit()
    {
        yield return ExitTransition();
    }
}