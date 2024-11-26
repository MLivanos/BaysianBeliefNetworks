using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterviewCalculator : MonoBehaviour
{
    private List<Node> rootNodes;
    private Dictionary<string, int> eventIndices;
    private LikelihoodWeightingSampler sampler;

    public void Initialize(List<Node> root, Dictionary<string, int> indices, LikelihoodWeightingSampler samplingAlgorithm)
    {
        rootNodes = root;
        eventIndices = indices;
        sampler = samplingAlgorithm;
    }

    public void AddToEvidence(Node node, bool eventOccurs)
    {
        sampler.AddToEvidence(node, eventOccurs);
    }

    public void Reset()
    {
        sampler.ClearEvidence();
        sampler.Reset();
    }

    private float CalculateProbability(int numberOfSamples=10000)
    {
        int[] positiveQuery = new int[1] {eventIndices["Alien"]};
        int[] negativeQuery = new int[0];
        for (int i = 0; i < numberOfSamples; i++)
        {
            sampler.Sample(positiveQuery, negativeQuery, rootNodes.ToList());
        }
        return sampler.CalculateProbability();
    }

    public float CalculateProbability(float precision, int maxIterations, int windowSize, float z=1.96f)
    {
        List<float> slidingWindow = new List<float>();
        for (int i = 0; i < windowSize - 1; i++)
        {
            slidingWindow.Add(CalculateProbability());
        }
        slidingWindow.Add(0f);
        float ciWidth = 0f;
        for(int i = windowSize - 1; i < maxIterations; i++)
        {
            slidingWindow[i % windowSize] = CalculateProbability();
            float mean = slidingWindow.Average();
            float std = Mathf.Sqrt(slidingWindow.Select(p => (p - mean) * (p - mean)).Average());

            float standardError = std / Mathf.Sqrt(windowSize);
            ciWidth = z * standardError;

            if (ciWidth < slidingWindow[i % windowSize] * (1 - precision))
            {
                return slidingWindow[i % windowSize];
            }
        }
        return slidingWindow[(maxIterations-1)%windowSize];
    }
}