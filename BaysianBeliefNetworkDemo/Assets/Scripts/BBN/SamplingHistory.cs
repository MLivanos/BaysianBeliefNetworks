using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SamplingRecord
{
    public List<string> positiveQuery;
    public List<string> negativeQuery;
    public List<string> positiveEvidence;
    public List<string> negativeEvidence;
    public float probability;
    public int numberOfSamples;
    public string samplerName;
    public DateTime timestamp;
    public bool dirty=false;

    public SamplingRecord(
        List<Node> posQuery, List<Node> negQuery,
        List<Node> posEvidence, List<Node> negEvidence,
        float probability, int samples, string samplerName)
    {
        this.positiveQuery = posQuery.Select(n => n.GetName()).OrderBy(n => n).ToList();
        this.negativeQuery = negQuery.Select(n => n.GetName()).OrderBy(n => n).ToList();
        this.positiveEvidence = posEvidence.Select(n => n.GetName()).OrderBy(n => n).ToList();
        this.negativeEvidence = negEvidence.Select(n => n.GetName()).OrderBy(n => n).ToList();
        this.probability = probability;
        this.numberOfSamples = samples;
        this.samplerName = samplerName;
        this.timestamp = DateTime.Now;
    }

    public string GetFormattedQuery()
    {
        string Format(List<string> list) => string.Join(", ", list);
        return $"P({Format(positiveQuery)} | {Format(positiveEvidence)})";
    }
}

public class SamplingHistory : MonoBehaviour
{
    private List<SamplingRecord> history = new List<SamplingRecord>();

    public void AddRecord(SamplingRecord record)
    {
        history.Add(record);
    }

    public List<SamplingRecord> GetHistory()
    {
        return history;
    }

    public void ClearHistory()
    {
        history.Clear();
    }
}
