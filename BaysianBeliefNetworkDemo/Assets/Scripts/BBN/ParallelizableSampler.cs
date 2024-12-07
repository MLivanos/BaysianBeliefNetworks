using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;

public abstract class ParallelizableSampler : Sampler
{
    NativeList<JobHandle> jobList;
    protected NativeArray<int> parentIndices;
    protected NativeArray<int> parentCount;
    protected NativeArray<bool> sampledParentValues;
    protected NativeArray<float> conditionalProbabilities;
    protected NativeArray<int> cptOffsets;
    protected NativeArray<bool> sampledNodeValues;
    protected System.Random randomGenerator = new System.Random();
    private int numberOfThreads = 16;

    ~ParallelizableSampler()
    {
        if (jobList.IsCreated)
        {
            jobList.Dispose();
        }
    }

    private new void Start()
    {
        base.Start();
        InitializeNativeArrays();
        jobList = new NativeList<JobHandle>(Allocator.Persistent);
    }

    public override void Sample()
    {
        if (sampleCount % numberOfThreads == 0 && sampleCount > 0)
        {
            JobHandle.CompleteAll(jobList);
            jobList.Dispose();
            jobList.Clear();
        }
        jobList.Add(CreateJob());
    }

    public abstract JobHandle CreateJob();

    protected void InitializeNativeArrays()
    {
        var nodes = graph.GetAllNodes();

        parentIndices = new NativeArray<int>(CalculateParentIndices(nodes), Allocator.TempJob);
        parentCount = new NativeArray<int>(CalculateParentIndices(nodes), Allocator.TempJob);
        sampledParentValues = new NativeArray<bool>(nodes.Count, Allocator.TempJob);
        conditionalProbabilities = new NativeArray<float>(FlattenCPTs(nodes), Allocator.TempJob);
        cptOffsets = new NativeArray<int>(nodes.Count, Allocator.TempJob);
        sampledNodeValues = new NativeArray<bool>(nodes.Count, Allocator.TempJob);

        int offset = 0;
        List<int> parentCountList = new List<int>();
        for (int i = 0; i < nodes.Count; i++)
        {
            parentCountList.Add(nodes[i].GetParents().Count);
            cptOffsets[i] = offset;
            offset += nodes[i].GetCPT().Length;
        }
        parentCount = new NativeArray<int>(parentCountList.ToArray(), Allocator.TempJob);
    }

    public void Dispose()
    {
        if (parentIndices.IsCreated) parentIndices.Dispose();
        if (parentCount.IsCreated) parentCount.Dispose();
        if (sampledParentValues.IsCreated) sampledParentValues.Dispose();
        if (conditionalProbabilities.IsCreated) conditionalProbabilities.Dispose();
        if (cptOffsets.IsCreated) cptOffsets.Dispose();
        if (sampledNodeValues.IsCreated) sampledNodeValues.Dispose();
    }

    private int[] CalculateParentIndices(List<Node> nodes)
    {
        List<int> indices = new List<int>();
        foreach (var node in nodes)
        {
            foreach (var parent in node.GetParents())
            {
                indices.Add(nodes.IndexOf(parent));
            }
        }
        return indices.ToArray();
    }

    private float[] FlattenCPTs(List<Node> nodes)
    {
        List<float> cpts = new List<float>();
        foreach (var node in nodes)
        {
            cpts.AddRange(node.GetCPT());
        }
        return cpts.ToArray();
    }
}
