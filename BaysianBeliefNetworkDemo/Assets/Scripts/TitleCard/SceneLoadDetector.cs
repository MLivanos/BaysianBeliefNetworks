using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISceneDetectorTarget
{
    public string HomeSceneName { get; }
    public void OnSceneChange(bool isHomeScene);
}

public class SceneLoadDetector : MonoBehaviour
{
	[SerializeField] private string targetName;
	private ISceneDetectorTarget target;

	private void Awake()
	{
		target = GameObject.Find(targetName).GetComponent<ISceneDetectorTarget>();
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		target.OnSceneChange(scene.name == target.HomeSceneName);
	}

	private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}