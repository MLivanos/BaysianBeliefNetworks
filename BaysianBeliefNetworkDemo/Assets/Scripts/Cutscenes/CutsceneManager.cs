using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private CutsceneBehavior[] cutscenes;
    [SerializeField] private TypewriterEffect typewriterEffect;
    [SerializeField] private GameObject textPanel;
    [SerializeField] private Transform mainCamera;
    private SceneManagerScript sceneManager;
    private CutsceneBehavior currentCutScene;
    private Coroutine currentCoroutine;
    private int cutsceneIndex = 0;
    private bool exiting;

    private void Start()
    {
        sceneManager = GetComponent<SceneManagerScript>();
        textPanel.SetActive(false);
        currentCoroutine = StartCoroutine(PlayNextScene());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            sceneManager.StartGame();
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (typewriterEffect.IsTyping()) typewriterEffect.Interrupt();
            else EndScene();
        }
    }

    private IEnumerator PlayNextScene()
    {
        if (cutsceneIndex < cutscenes.Length)
        {
            currentCutScene = cutscenes[cutsceneIndex];
            currentCutScene.SetupObjects(mainCamera, textPanel, typewriterEffect);
            yield return currentCutScene.Play();  
        }
        else
        {
            sceneManager.StartGame();
        }
    }

    private IEnumerator ExitScene()
    {
        if (!exiting)
        {
            if (cutsceneIndex + 1 < cutscenes.Length && cutscenes[cutsceneIndex + 1].NeedsPrewarm()) cutscenes[cutsceneIndex + 1].Prewarm(); 
            currentCutScene.Interrupt();
            exiting = true;
            yield return currentCutScene.Exit();
            exiting = false;
            cutsceneIndex ++;
            yield return PlayNextScene();
        }
    }

    private void EndScene()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
        currentCoroutine = StartCoroutine(ExitScene());
    }
}