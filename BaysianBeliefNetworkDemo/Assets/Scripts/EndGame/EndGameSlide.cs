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
	[SerializeField] protected List<int> codesToScene;
	protected Dictionary<int, int> sceneLookup = new Dictionary<int, int>();
	protected AudioManager audioManager;

	private void Start()
	{
		if (codesToScene.Count < scenes.Count) Debug.LogError("The slide {gameObject.name} has {codesToScene.Count} codes but {scenes.Count} scenes");
		for(int i=0; i<codesToScene.Count; i++)
		{
			sceneLookup.Add(i, codesToScene[i]);
		}
	}
	
	public void Run(int sceneCode)
	{
		scenes[sceneLookup[sceneCode]].code = sceneCode;
		StartCoroutine(scenes[sceneLookup[sceneCode]].Play());
	}
}