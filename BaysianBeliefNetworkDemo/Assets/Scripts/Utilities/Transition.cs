using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Transition : MonoBehaviour
{
    [SerializeField] protected string sceneName;
    [SerializeField] protected bool loadsScene;
    protected SceneManagerScript sceneManager;
    protected AsyncOperation asyncLoad; 

    private void Start()
    {
        sceneManager = GetComponent<SceneManagerScript>();
    }

    public void PerformTransition()
    {
        //if (loadsScene) asyncLoad = sceneManager.PreloadScene(sceneName);
        StartCoroutine(TransitionToScene());
    }

    protected abstract IEnumerator TransitionToScene();
}
