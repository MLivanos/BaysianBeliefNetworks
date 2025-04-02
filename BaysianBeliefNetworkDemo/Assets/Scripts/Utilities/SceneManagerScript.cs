using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    [SerializeField] private LoadingScreenTextEffect loadingText;

    public void GoToDemo()
    {
        DisplayLoadingText("LOADING SIMULATION");
        SceneManager.LoadScene("AnimatedDemo");
    }

    public void GoToNetwork()
    {
        StartGame();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("BBN");
    }

    public void GoToCutscenes()
    {
        DisplayLoadingText("LOADING SCENE", true, 5f);
        SceneManager.LoadScene("Cutscenes", LoadSceneMode.Single);
    }

    public void GoToInterviews()
    {
        DisplayLoadingText("LOADING SCENE");
        SceneManager.LoadScene("Interviews", LoadSceneMode.Single);
    }

    public void GoToEndGame()
    {
        SceneManager.LoadScene("EndGame");
    }

    public AsyncOperation PreloadScene(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        operation.allowSceneActivation = false;
        return operation;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void GoToCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void GoToMainMenu()
    {
        FadeMusic();
        DisplayLoadingText("LOADING MENU");
        SceneManager.LoadScene("TitleScreen");
    }

    public void Exit()
    {
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void DisplayLoadingText(string text, bool fadeIn=true, float delay=0f)
    {
        if (loadingText == null)
        {
            Debug.LogWarning("No loading text set. Aborting.");
            return;
        }
        loadingText.gameObject.SetActive(true);
        loadingText.ChangeFadeIn(fadeIn);
        loadingText.ChangeMessage(text);
        loadingText.StartElipsesEffect(delay);
    }

    private void FadeMusic()
    {
        AudioManager.instance.FadeOutMusic(1.5f);
    }
}
