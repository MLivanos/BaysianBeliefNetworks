using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    public void GoToDemo()
    {
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
        SceneManager.LoadScene("Cutscenes", LoadSceneMode.Single);
    }

    public void GoToInterviews()
    {
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
        SceneManager.LoadScene("TitleScreen");
    }

    public void Exit()
    {
        Debug.Log("Exit called (will not exit if in editor)");
        Application.Quit();
    }
}
