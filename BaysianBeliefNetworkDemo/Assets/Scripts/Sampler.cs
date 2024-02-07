using System.Linq;
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

    public virtual void CalculateProbability()
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

    public virtual void Reset()
    {
        samples = new List<bool[]>();
        sampleCount = 0;
    }
}
