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
        Graph graph = GameObject.Find("Graph").GetComponent<Graph>();
        graph.UnsaveGraph();
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

    public AsyncOperation PreloadScene(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        operation.allowSceneActivation = false;
        return operation;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void GoToCredits()
    {
        
    }
}
