using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Transition : MonoBehaviour
{
    //[SerializeField] protected Slider progressBar;
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
        if (loadsScene) StartCoroutine(PreloadAndTransition());
        else StartCoroutine(TransitionToScene());
    }

    private IEnumerator PreloadAndTransition()
    {
        //if (progressBar) progressBar.gameObject.SetActive(true);
        AsyncOperation asyncLoad = sceneManager.PreloadScene(sceneName);
        asyncLoad.allowSceneActivation = false;
        while (asyncLoad.progress < 0.9f)
        {
            //if (progressBar) progressBar.value = asyncLoad.progress / 0.9f;
            yield return null;
        }
        //if (progressBar) progressBar.gameObject.SetActive(false);
        yield return TransitionToScene();
        asyncLoad.allowSceneActivation = true;
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    protected abstract IEnumerator TransitionToScene();
}
