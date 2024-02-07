using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RejectionSampler : Sampler
{
    public override void Sample()
    {
        List<Node> currentNodes;
        for(int i=0; i<numberOfSamples; i++)
        {
            int index = 0;
            bool[] truthValues = new bool[10];
            currentNodes = graph.GetRootNodes().ToList();
            while(currentNodes.Count > 0)
            {
                Node node = currentNodes[0];
                currentNodes.RemoveAt(0);
                if (node.IsReadyToCalculateProbability())
                {
                    node.IsTrue(node.Query(Random.value));
                    names[index] = node.GetName();
                    truthValues[index] = node.IsTrue();
                    AddChildren(currentNodes, node.GetChildren());
                    index++;
                }
            }
            samples.Add(truthValues);
        }
        sampleCount += numberOfSamples;
        CalculateProbability();
    }

    public void CalculateProbability()
    {
        List<bool[]> filteredSamples = FilterSamples(samples, graph.GetPositiveEvidence(), graph.GetNegativeEvidence());
        List<bool[]> filteredSamplesInQuery = FilterSamples(filteredSamples, graph.GetPositiveQuery(), graph.GetNegativeQuery());
        if (filteredSamples.Count == 0)
        {
            Debug.Log("N/A (evidence never occured)");
            return;
        }
        Debug.Log((float)filteredSamplesInQuery.Count / filteredSamples.Count);
    }

    private List<bool[]> FilterSamples(List<bool[]> group, List<Node> positiveNodes, List<Node> negativeNodes)
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

    private bool FilerSample(bool[] sample, int[] indices, bool isTrue)
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
}
