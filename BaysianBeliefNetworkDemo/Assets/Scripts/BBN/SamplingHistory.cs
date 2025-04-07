using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

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
        this.positiveQuery = posQuery.Select(n => n.GetAbriviation()).OrderBy(n => n).ToList();
        this.negativeQuery = negQuery.Select(n => n.GetAbriviation()).OrderBy(n => n).ToList();
        this.positiveEvidence = posEvidence.Select(n => n.GetAbriviation()).OrderBy(n => n).ToList();
        this.negativeEvidence = negEvidence.Select(n => n.GetAbriviation()).OrderBy(n => n).ToList();
        this.probability = probability;
        this.numberOfSamples = samples;
        this.samplerName = samplerName;
        this.timestamp = DateTime.Now;
    }

    public string GetFormattedQuery()
    {
        List<string> queryTerms = new List<string>();
        queryTerms.AddRange(positiveQuery);
        queryTerms.AddRange(negativeQuery.Select(n => $"¬{n}"));

        List<string> evidenceTerms = new List<string>();
        evidenceTerms.AddRange(positiveEvidence);
        evidenceTerms.AddRange(negativeEvidence.Select(n => $"¬{n}"));

        string Format(List<string> list) => string.Join(", ", list.OrderBy(s => s));

        return $"P({Format(queryTerms)} | {Format(evidenceTerms)})";
    }

    public string FormatSampleCount()
    {
        if (numberOfSamples >= 1_000_000_000) return "Many";
        if (numberOfSamples >= 1_000_000) return $"{(numberOfSamples / 1_000_000f):0.#}M";
        if (numberOfSamples >= 1_000) return $"{(numberOfSamples / 1_000f):0.#}K";
        return numberOfSamples.ToString();
    }

    public string GetSamplerAbbreviation()
    {
        return samplerName switch
        {
            "RejectionSampler" => "RS",
            "LikelihoodWeightingSampler" => "LWS",
            "GibbsSampler" => "GS",
            "HamiltonianSampler" => "HS",
            _ => samplerName.Substring(0, 3).ToUpper()
        };
    }

    public string GetFormattedDisplay()
    {
        string query = GetFormattedQuery();
        string prob = $"≈{probability:0.###}";
        string samples = FormatSampleCount();
        string sampler = GetSamplerAbbreviation();

        return $"{query} {prob} | {samples} | {sampler}";
    }

}

public class SamplingHistory : MonoBehaviour
{
    private List<SamplingRecord> history = new List<SamplingRecord>();
    [SerializeField] private TextMeshProUGUI historyText;

    public static SamplingHistory instance;
    public string historyRecord {get; private set;}

    private void Awake()
    {
        historyText.text = "P(Q|E) ≈ Prob | Samples | Algo\n";
        if (instance != null)
        {
            historyText.text += instance.historyRecord;
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddRecord(SamplingRecord record)
    {
        historyText.text += record.GetFormattedDisplay() + "\n";
        historyRecord += record.GetFormattedDisplay() + "\n";
        history.Add(record);
    }

    public List<SamplingRecord> GetHistory()
    {
        return history;
    }

    public void SetHistoryText(string newText)
    {
        if (historyText == null) historyText = GameObject.FindWithTag("HistoryText").GetComponent<TextMeshProUGUI>();
        historyText.text = newText;
    }
    public string GetHistoryText() => historyText.text;

    public void ClearHistory()
    {
        history.Clear();
    }

}