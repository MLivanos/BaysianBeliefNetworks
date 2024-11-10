using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Sampler : MonoBehaviour
{
    protected Graph graph;
    protected List<Node> currentNodes;
    protected int sampleCount;
    protected int numberOfNodes;
    protected int numberOfSamples = 10000;
    protected List<bool[]> samples = new List<bool[]>();
    protected string[] names = new string[10];
    protected Dictionary<Node, bool> evidence;

    private void Start()
    {
        graph = GetComponent<Graph>();
        currentNodes = graph.GetAllNodes();
        numberOfNodes = currentNodes.Count;
        GatherEvidence();
    }

    public virtual float Sample()
    {
        return -1.0f;
    }

    public virtual float CalculateProbability()
    {
        return -1.0f;
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

    protected int[] GetNodeOrder(List<Node> nodeList)
    {
        List<Node> currentNodes;
        int[] nodeOrder = new int[nodeList.Count];
        bool[] truthValues = new bool[10];
        currentNodes = graph.GetRootNodes().ToList();
        int orderIndex = 0;
        int index = 0;
        while(currentNodes.Count > 0)
        {
            Node node = currentNodes[0];
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
                AddChildren(currentNodes, node.GetChildren());
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
            if(FilerSample(sample, positiveEvidenceOrder, true) && FilerSample(sample, negativeEvidenceOrder, false))
            {
                samplesWithEvidence.Add(sample);
            }
        }
        return samplesWithEvidence;
    }

    protected bool FilerSample(bool[] sample, int[] indices, bool isTrue)
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

    protected void GatherEvidence()
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

    protected bool IsInEvidence(Node node)
    {
        return evidence.ContainsKey(node);
    }

    public virtual void Reset()
    {
        samples = new List<bool[]>();
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
}
