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
        SceneManager.LoadScene("BBN");
    }
}
