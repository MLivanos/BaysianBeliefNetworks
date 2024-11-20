using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public class ParallelizedRejectionSampler : ParallelizableSampler
{
    int numberOfAcceptedSamples;

    public override JobHandle CreateJob()
    {
        InitializeNativeArrays();
        Unity.Mathematics.Random random = new Unity.Mathematics.Random((uint)System.DateTime.Now.Ticks);
        var job = new RejectionSamplingJob
        {
            parentIndices = parentIndices,
            parentCount = parentCount,
            sampledParentValues = sampledParentValues,
            conditionalProbabilities = conditionalProbabilities,
            cptOffsets = cptOffsets,
            sampledNodeValues = sampledNodeValues,
            randomGenerator = random
        };

        return job.Schedule();
    }

    public override float CalculateProbability()
    {
        List<bool[]> filteredSamples = FilterSamples(samples, graph.GetPositiveEvidence(), graph.GetNegativeEvidence());
        List<bool[]> filteredSamplesInQuery = FilterSamples(filteredSamples, graph.GetPositiveQuery(), graph.GetNegativeQuery());
        numberOfAcceptedSamples = filteredSamples.Count;
        if (filteredSamples.Count == 0)
        {
            Debug.Log("N/A (evidence never occured)");
            return -1.0f;
        }
        return (float)filteredSamplesInQuery.Count / (float)filteredSamples.Count;
    }

    public override int GetNumberOfAcceptedSamples()
    {
        return numberOfAcceptedSamples;
    }
}

public struct RejectionSamplingJob : IJob
{
    [ReadOnly] public NativeArray<int> parentIndices; // Indices of parent nodes
    [ReadOnly] public NativeArray<int> parentCount;
    [ReadOnly] public NativeArray<bool> sampledParentValues; // Intermediate truth values for parents
    [ReadOnly] public NativeArray<float> conditionalProbabilities; // Flattened CPTs
    [ReadOnly] public NativeArray<int> cptOffsets; // Start indices of each node's CPT in the array
    public NativeArray<bool> sampledNodeValues; // Output truth values for each node
    public Unity.Mathematics.Random randomGenerator;

    public void Execute()
    {
        for (int i = 0; i < sampledNodeValues.Length; i++)
        {
            // Determine the index in the CPT for this node based on its parents' truth values
            int offset = cptOffsets[i]; // Starting index in parentIndices for this node
            int nodesParentCount = parentCount[i]; // Number of parents for this node (rename parentIndices[i] for clarity)
            int cptIndex = 0;

            for (int p = 0; p < nodesParentCount; p++)
            {
                int parentIndex = parentIndices[offset + p]; // Index of parent in sampledParentValues
                bool parentValue = sampledParentValues[parentIndex];
                cptIndex |= (parentValue ? 1 : 0) << p; // Construct index bitwise
            }

            // Sample based on the conditional probability
            float nodeProbability = conditionalProbabilities[offset + cptIndex];
            float probability = randomGenerator.NextFloat();

            sampledNodeValues[i] = probability < nodeProbability;
        }
    }
}
