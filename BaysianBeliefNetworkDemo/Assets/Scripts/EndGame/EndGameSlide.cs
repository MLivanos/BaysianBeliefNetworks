using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class EndGameCutscene : CutsceneBehavior
{
	public int code;
}

public class EndGameSlide : MonoBehaviour
{
	[SerializeField] protected List<EndGameCutscene> scenes;
	protected AudioManager audioManager;
	
	public void Run(int sceneCode)
	{
		scenes[sceneCode].code = sceneCode;
		scenes[sceneCode].Play();
	}
}