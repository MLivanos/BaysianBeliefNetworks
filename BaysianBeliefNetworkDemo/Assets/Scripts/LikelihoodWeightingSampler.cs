using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LikelihoodWeightingSampler : Sampler
{
    private float sumOfWeights;
    private float sumOfWeightsInQuery;

    public override void Sample()
    {
        List<Node> currentNodes;
        List<Node> positiveEvidence = graph.GetPositiveEvidence();
        List<Node> negativeEvidence = graph.GetNegativeEvidence();
        int[] positiveQueryOrder = GetNodeOrder(graph.GetPositiveQuery());
        int[] negativeQueryOrder = GetNodeOrder(graph.GetNegativeQuery());
        for(int i=0; i<numberOfSamples; i++)
        {
            float weight = 1;
            int index = 0;
            bool[] truthValues = new bool[10];
            currentNodes = graph.GetRootNodes().ToList();
            while(currentNodes.Count > 0)
            {
                Node node = currentNodes[0];
                currentNodes.RemoveAt(0);
                if (node.IsReadyToCalculateProbability())
                {
                    if (positiveEvidence.Any(n => n == node))
                    {
                        node.IsTrue(true);
                        weight *= node.Query();
                    }
                    else if (negativeEvidence.Any(n => n == node))
                    {
                        node.IsTrue(false);
                        weight *= (1 - node.Query());
                    }
                    else
                    {
                        node.IsTrue(node.Query(Random.value));
                    }
                    names[index] = node.GetName();
                    truthValues[index] = node.IsTrue();
                    AddChildren(currentNodes, node.GetChildren());
                    index++;
                }
            }
            if(FilerSample(truthValues, positiveQueryOrder, true) && FilerSample(truthValues, negativeQueryOrder, false))
            {
                sumOfWeightsInQuery += weight;
            }
            sumOfWeights += weight;
            samples.Add(truthValues);
        }
        sampleCount += numberOfSamples;
        CalculateProbability();
    }

    public override void CalculateProbability()
    {  
        Debug.Log(sumOfWeightsInQuery/sumOfWeights);
        sumOfWeights = 0.0f;
        sumOfWeightsInQuery = 0.0f;
    }

}
