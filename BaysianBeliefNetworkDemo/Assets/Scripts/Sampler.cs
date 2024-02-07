using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Sampler : MonoBehaviour
{
    protected Graph graph;
    protected int sampleCount;
    protected int numberOfSamples = 10000;
    protected List<bool[]> samples = new List<bool[]>();
    protected string[] names = new string[10];

    private void Start()
    {
        graph = GetComponent<Graph>();
    }

    public virtual void Sample()
    {

    }

    protected void AddChildren(List<Node> currentNodes, List<Node> children)
    {
        foreach(Node child in children)
        {
            if (!currentNodes.Contains(child))
            {
                currentNodes.Add(child);
            }
        }
    }
}
