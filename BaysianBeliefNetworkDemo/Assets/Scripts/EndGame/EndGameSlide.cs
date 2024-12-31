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
	protected int code;
	protected Dictionary<int, int> sceneLookup = new Dictionary<int, int>();
	protected AudioManager audioManager;

	private void Awake()
	{
		if (codesToScene.Count < scenes.Count) Debug.LogError("The slide {gameObject.name} has {codesToScene.Count} codes but {scenes.Count} scenes");
		for(int i=0; i<codesToScene.Count; i++)
		{
			sceneLookup.Add(i, codesToScene[i]);
		}
	}
	
	public IEnumerator Run(int sceneCode, Transform mainCamera, GameObject textPanel, TypewriterEffect typewriterEffect)
	{
		code = sceneCode;
		scenes[sceneLookup[code]].code = sceneCode;
		scenes[sceneLookup[code]].SetupObjects(mainCamera, textPanel, typewriterEffect);
		yield return scenes[sceneLookup[sceneCode]].Play();
	}

	public IEnumerator Exit()
	{
		yield return scenes[sceneLookup[code]].Exit();
	}
}