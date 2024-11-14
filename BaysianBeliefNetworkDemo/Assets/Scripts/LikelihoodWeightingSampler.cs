using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class LikelihoodWeightingSampler : Sampler
{
    private float sumOfWeights;
    private float sumOfWeightsInQuery;

    public override float Sample()
    {
        GatherEvidence();
        List<Node> currentNodes;
        int[] positiveQueryOrder = GetNodeOrder(graph.GetPositiveQuery());
        int[] negativeQueryOrder = GetNodeOrder(graph.GetNegativeQuery());

        for (int i = 0; i < numberOfSamples; i++)
        {
            float weight = 1;
            int index = 0;
            bool[] truthValues = new bool[numberOfNodes];
            currentNodes = graph.GetRootNodes().ToList();
            HashSet<Node> processedNodes = new HashSet<Node>();
            while (currentNodes.Count > 0)
            {
                Node node = currentNodes[0];
                processedNodes.Add(node);
                currentNodes.RemoveAt(0);
                if (node.IsReadyToCalculateProbability())
                {
                    weight *= ProcessNodeEvidence(node);
                    truthValues[index] = node.IsTrue();
                    AddChildren(currentNodes, node.GetChildren(), processedNodes);
                    index++;
                }
            }

            if (FilterSample(truthValues, positiveQueryOrder, true) && FilterSample(truthValues, negativeQueryOrder, false))
            {
                sumOfWeightsInQuery += weight;
            }

            sumOfWeights += weight;
            samples.Add(truthValues);
        }

        sampleCount += numberOfSamples;
        return CalculateProbability();
    }

    private float ProcessNodeEvidence(Node node)
    {
        float weight = 1;
        if (IsInEvidence(node))
        {
            bool observedValue = evidence[node];
            node.IsTrue(observedValue);
            float newWeight = observedValue ? node.Query() : (1 - node.Query());
            weight *= newWeight;
        }
        else
        {
            node.IsTrue(node.Query(Random.value));
        }
        return weight;
    }

    public override float CalculateProbability()
    {
        return sumOfWeights > 0 ? sumOfWeightsInQuery / sumOfWeights : -1;
    }

    public override void Reset()
    {
        base.Reset();
        sumOfWeights = 0.0f;
        sumOfWeightsInQuery = 0.0f;
    }

    public override int GetNumberOfAcceptedSamples()
    {
        return samples.Count;
    }
}