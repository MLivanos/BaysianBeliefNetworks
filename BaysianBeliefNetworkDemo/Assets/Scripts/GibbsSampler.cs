using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GibbsSampler : Sampler
{
    private float burnInPercentage=0.1f;
    private float randomInitializationPercentage=0.1f;
    private Dictionary<Node, bool> evidence;

    public override float Sample()
    {
        int reinitializationInterval = (int)(1 / Mathf.Max(0.00001f,randomInitializationPercentage));
        GatherEvidence();
        InitializeState(currentNodes);

        for (int i = 0; i < numberOfSamples; i++)
        {
            if (i%reinitializationInterval == 0)
            {
                InitializeState(currentNodes);
            }
            foreach (Node node in currentNodes)
            {
                if (evidence.ContainsKey(node))
                {
                    node.IsTrue(evidence[node]);
                }
                else
                {
                    float probTrue = CalculateConditionalProbability(node);
                    node.IsTrue(Random.value < probTrue);
                }
            }
            bool[] truthValues = currentNodes.Select(n => n.IsTrue()).ToArray();
            samples.Add(truthValues);
        }

        sampleCount += numberOfSamples;
        return CalculateProbability();
    }

    private void GatherEvidence()
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

    private void InitializeState(List<Node> nodes)
    {
        foreach (Node node in nodes)
        {
            if (!evidence.ContainsKey(node))
            {
                node.IsTrue(Random.value < node.Query());
            }
            else
            {
                node.IsTrue(evidence[node]);
            }
        }
    }

    private float CalculateConditionalProbability(Node node)
    {
        List<Node> parents = node.GetParents();
        List<Node> children = node.GetChildren();

        // Step 1: Get the probability of the node given its parents (from CPT)
        float probGivenParents = node.Query();

        // Step 2: Calculate the influence of each child, given the values of all its parents
        float childInfluenceTrue = 1.0f;
        float childInfluenceFalse = 1.0f;

        foreach (var child in children)
        {
            // Restore the node's original state
            bool originalState = node.IsTrue();

            // Set node to true temporarily to calculate influence
            node.IsTrue(true);
            float childProbIfTrue = child.Query();

            // Set node to false temporarily to calculate influence
            node.IsTrue(false);
            float childProbIfFalse = child.Query();

            node.IsTrue(originalState);

            // Adjust influence based on the child's current state
            if (child.IsTrue())
            {
                childInfluenceTrue *= childProbIfTrue;
                childInfluenceFalse *= childProbIfFalse;
            }
            else
            {
                childInfluenceTrue *= (1 - childProbIfTrue);
                childInfluenceFalse *= (1 - childProbIfFalse);
            }
        }

        // Step 3: Use Bayes' rule to normalize and find final probability
        float probTrueGivenMarkovBlanket = probGivenParents * childInfluenceTrue;
        float probFalseGivenMarkovBlanket = (1 - probGivenParents) * childInfluenceFalse;

        // Normalize so probabilities sum to 1
        return probTrueGivenMarkovBlanket / (probTrueGivenMarkovBlanket + probFalseGivenMarkovBlanket);
    }

    public override float CalculateProbability()
    {
        List<bool[]> filteredQueries = samples.GetRange((int)burnInPercentage*samples.Count, samples.Count);
        List<bool[]> filteredSamplesInQuery = FilterSamples(filteredQueries, graph.GetPositiveQuery(), graph.GetNegativeQuery());
        int numberOfAcceptedSamples = samples.Count;

        if (samples.Count == 0)
        {
            Debug.Log("N/A (no samples available)");
            return -1.0f;
        }

        return (float)filteredSamplesInQuery.Count / filteredQueries.Count;
    }

    public override int GetNumberOfAcceptedSamples()
    {
        return samples.Count - (int)(burnInPercentage*samples.Count);
    }

    public void SetBurnIn(float burnIn)
    {
        burnInPercentage = burnIn;
        Debug.Log(burnInPercentage);
    }

    public void SetRandomInitialization(float initializationPercentage)
    {
        randomInitializationPercentage = initializationPercentage;
    }

    public void SetRandomInitialization(string percentageText)
    {
        SetRandomInitialization(float.Parse(percentageText));
    }

    public void SetBurnIn(string percentageText)
    {
        SetBurnIn(float.Parse(percentageText));
    }
}
