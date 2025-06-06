using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public abstract class Sampler : MonoBehaviour
{
    [SerializeField] protected bool parallelizable;
    [SerializeField] protected bool reinstantiable;
    protected Graph graph;
    protected List<Node> currentNodes;
    protected int sampleCount;
    protected int numberOfNodes;
    protected int numberOfSamples = 10000;
    protected List<bool[]> samples = new List<bool[]>();
    protected Dictionary<Node, bool> evidence = new Dictionary<Node, bool>();
    protected float timeElapsed;
    private bool busy;
    private int currentIteration;

    protected void Start()
    {
        graph = GetComponent<Graph>();
        currentNodes = graph.GetAllNodes();
        numberOfNodes = currentNodes.Count;
    }

    public IEnumerator RunSamples()
    {
        busy = true;
        GatherEvidence();
        float timer = Time.realtimeSinceStartup;
        for (currentIteration=0; currentIteration<numberOfSamples; currentIteration++)
        {
            if(Time.realtimeSinceStartup - timer > 0.1f)
            {
                if (!busy) break;
                yield return null;
                timer = Time.realtimeSinceStartup;
            }
            Sample();
            sampleCount++;
        }
        busy = false;
    }

    public abstract void Sample();

    public virtual float CalculateProbability()
    {
        return -1.0f;
    }

    protected void AddChildren(List<Node> currentNodes, List<Node> children, HashSet<Node> processedNodes)
    {
        foreach(Node child in children)
        {
            if (!currentNodes.Contains(child) && !processedNodes.Contains(child))
            {
                currentNodes.Add(child);
            }
        }
    }

    protected int[] GetNodeOrder(List<Node> nodeList)
    {
        List<Node> currentNodes;
        int[] nodeOrder = new int[nodeList.Count];
        currentNodes = graph.GetRootNodes().ToList();
        int orderIndex = 0;
        int index = 0;
        HashSet<Node> processedNodes = new HashSet<Node>();
        while(currentNodes.Count > 0)
        {
            Node node = currentNodes[0];
            processedNodes.Add(node);
            currentNodes.RemoveAt(0);
            if (node.IsReadyToCalculateProbability())
            {
                node.IsTrue(node.Query(Random.value));
                if (nodeList.Any(n => n == node))
                {
                    nodeOrder[orderIndex] = index;
                    orderIndex ++;
                }
                index ++;
                AddChildren(currentNodes, node.GetChildren(), processedNodes);
            }
        }
        return nodeOrder;
    }

    public void SetNumberOfSamples(int nSamples)
    {
        numberOfSamples = nSamples;
    }

    protected List<bool[]> FilterSamples(List<bool[]> group, List<Node> positiveNodes, List<Node> negativeNodes)
    {
        List<bool[]> samplesWithEvidence = new List<bool[]>();
        int[] positiveEvidenceOrder = GetNodeOrder(positiveNodes);
        int[] negativeEvidenceOrder = GetNodeOrder(negativeNodes);
        foreach(bool[] sample in group)
        {
            if(FilterSample(sample, positiveEvidenceOrder, true) && FilterSample(sample, negativeEvidenceOrder, false))
            {
                samplesWithEvidence.Add(sample);
            }
        }
        return samplesWithEvidence;
    }

    protected bool FilterSample(bool[] sample, int[] indices, bool isTrue)
    {
        foreach(int index in indices)
        {
            if(sample[index] != isTrue)
            {
                return false;
            }
        }
        return true;
    }

    public void GatherEvidence()
    {
        evidence = new Dictionary<Node, bool>();
        List<Node> positiveEvidence = graph.GetPositiveEvidence();
        List<Node> negativeEvidence = graph.GetNegativeEvidence();
        foreach (Node node in positiveEvidence)
        {
            evidence.Add(node, true);
        }
        foreach (Node node in negativeEvidence)
        {
            evidence.Add(node, false);
        }
    }

    public void AddToEvidence(Node node, bool positive)
    {
        evidence.Add(node, positive);
    }

    public void ClearEvidence()
    {
        evidence = new Dictionary<Node, bool>();
    }

    protected bool IsInEvidence(Node node)
    {
        return evidence.ContainsKey(node);
    }

    public virtual void Reset()
    {
        samples = new List<bool[]>();
        timeElapsed = 0f;
        sampleCount = 0;
    }

    public int GetNumberOfSamples()
    {
        return sampleCount;
    }

    public virtual int GetNumberOfAcceptedSamples()
    {
        return -1;
    }

    public bool[] GetLastSample()
    {
        return samples[samples.Count - 1];
    }

    public float GetTimeElapsed()
    {
        return timeElapsed;
    }

    public void AddTime(float time)
    {
        timeElapsed += time;
    }

    public bool Busy()
    {
        return busy;
    }

    public float GetProgress()
    {
        return (float)currentIteration / numberOfSamples;
    }

    public void Interupt()
    {
        busy = false;
    }
}
