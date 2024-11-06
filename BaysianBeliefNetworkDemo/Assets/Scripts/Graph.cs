using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class Graph : MonoBehaviour
{
    [SerializeField] private List<Node> rootNodes;
    [SerializeField] private TMP_Text queryTextDisplay;
    [SerializeField] private TMP_Text sampleInfo;
    private Sampler currentSampler;
    // Replace with sampler array if new sampler is added. Not planned.
    private RejectionSampler rejectionSampler;
    private LikelihoodWeightingSampler likelihoodWeightingSampler;
    private string queryText;
    private List<Node> positiveQuery;
    private List<Node> negativeQuery;
    private List<Node> positiveEvidence;
    private List<Node> negativeEvidence;
    bool isNegative;

    private void Start()
    {
        positiveQuery = new List<Node>();
        negativeQuery = new List<Node>();
        positiveEvidence = new List<Node>();
        negativeEvidence = new List<Node>();
        SaveGraph();
        rejectionSampler = GetComponent<RejectionSampler>();
        likelihoodWeightingSampler = GetComponent<LikelihoodWeightingSampler>();
        currentSampler = rejectionSampler;
    }

    private void SaveGraph()
    {
        DontDestroyOnLoad(gameObject);
        List<Node> currentNodes;
        currentNodes = rootNodes.ToList();
        while(currentNodes.Count > 0)
        {
            Node node = currentNodes[0];
            foreach(Node child in node.GetChildren())
            {
                if (!currentNodes.Contains(child))
                {
                    currentNodes.Add(child);
                }
            }
            currentNodes.RemoveAt(0);
            DontDestroyOnLoad(node);
        }
    }

    public void UnsaveGraph()
    {
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
        List<Node> currentNodes;
        currentNodes = rootNodes.ToList();
        while(currentNodes.Count > 0)
        {
            Node node = currentNodes[0];
            foreach(Node child in node.GetChildren())
            {
                if (!currentNodes.Contains(child))
                {
                    currentNodes.Add(child);
                }
            }
            currentNodes.RemoveAt(0);
            SceneManager.MoveGameObjectToScene(node.gameObject, SceneManager.GetActiveScene());
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            isNegative = true;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            isNegative = false;
        }
    }

    public List<Node> GetRootNodes()
    {
        return rootNodes;
    }

    public void AddToEvidence(Node node, VariableChecks checks)
    {
        List<Node> relevantList = isNegative ? negativeEvidence : positiveEvidence;
        relevantList.Add(node);
        if (positiveQuery.Any(n => n == node) || negativeQuery.Any(n => n == node))
        {
            checks.SwitchQuery();
        }
        likelihoodWeightingSampler.Reset();
    }

    public void AddToQuery(Node node, VariableChecks checks)
    {
        List<Node> relevantList = isNegative ? negativeQuery : positiveQuery;
        relevantList.Add(node);
        if (positiveEvidence.Any(n => n == node) || negativeEvidence.Any(n => n == node))
        {
            checks.SwitchEvidence();
        }
        likelihoodWeightingSampler.Reset();
    }

    public void RemoveFromEvidence(Node node)
    {
        List<Node> relevantList = positiveEvidence;
        if (negativeEvidence.Any(n => n == node))
        {
            relevantList = negativeEvidence;
        }
        relevantList.Remove(node);
        likelihoodWeightingSampler.Reset();
    }

    public void RemoveFromQuery(Node node)
    {
        List<Node> relevantList = positiveQuery;
        if (negativeQuery.Any(n => n == node))
        {
            relevantList = negativeQuery;
        }
        relevantList.Remove(node);
        likelihoodWeightingSampler.Reset();
    }

    public void UpdateText(float probabilityValue=-1.0f)
    {
        if (queryTextDisplay == null)
        {
            queryTextDisplay = GameObject.Find("QueryText").GetComponent<TMP_Text>();
            /*positiveQuery = new List<Node>();
            negativeQuery = new List<Node>();
            positiveEvidence = new List<Node>();
            negativeEvidence = new List<Node>();*/
        }
        queryText = "P(";
        queryText += GetPartialQuery(positiveQuery);
        if (positiveQuery.Count > 0 && negativeQuery.Count > 0)
        {
            queryText += ",";
        }
        queryText += GetPartialQuery(negativeQuery, true);
        if (positiveEvidence.Count + negativeEvidence.Count > 0)
        {
            queryText += "|";
        }
        queryText += GetPartialQuery(positiveEvidence);
        if (positiveEvidence.Count > 0 && negativeEvidence.Count > 0)
        {
            queryText += ",";
        }
        queryText += GetPartialQuery(negativeEvidence, true);
        queryText += ")";
        if (probabilityValue >= 0.0f)
        {
            queryText += "≈" + probabilityValue.ToString("0.00000");
        }
        queryTextDisplay.text = queryText;
    }

    private string GetPartialQuery(List<Node> nodeList, bool isNegative=false)
    {
        string queryText = "";
        for(int i=0; i < nodeList.Count; i++)
        {
            if (isNegative)
            {
                queryText += "¬";
            }
            queryText += nodeList[i].GetAbriviation();
            if (i < nodeList.Count - 1)
            {
                queryText += ",";
            }
        }
        return queryText;
    }

    public void Sample()
    {
        float probability = currentSampler.Sample();
        string numberOfSamples = currentSampler.GetNumberOfSamples().ToString();
        string numberOfAcceptedSamples = currentSampler.GetNumberOfAcceptedSamples().ToString();
        string sampleInfoText = numberOfSamples + "\n" + numberOfAcceptedSamples;
        sampleInfo.text = sampleInfoText;
        UpdateText(probability);
    }

    public void ChangeSampler(int index)
    {
        // Replace with sampler array if new sampler is added. Not planned.
        if (index == 0)
        {
            currentSampler = rejectionSampler;
        }
        else
        {
            currentSampler = likelihoodWeightingSampler;
        }
    }

    public List<Node> GetPositiveEvidence()
    {
        return positiveEvidence.ToList();
    }

    public List<Node> GetNegativeEvidence()
    {
        return negativeEvidence.ToList();
    }

    public List<Node> GetPositiveQuery()
    {
        return positiveQuery.ToList();
    }

    public List<Node> GetNegativeQuery()
    {
        return negativeQuery.ToList();
    }

    public void SetNumberOfSamples(string numberOfSamplesText)
    {
        rejectionSampler.SetNumberOfSamples(Int32.Parse(numberOfSamplesText));
        likelihoodWeightingSampler.SetNumberOfSamples(Int32.Parse(numberOfSamplesText));
    }

    public bool[] VisualizeSample()
    {
        likelihoodWeightingSampler.SetNumberOfSamples(1);
        likelihoodWeightingSampler.Sample();
        bool[] truthValues = likelihoodWeightingSampler.GetLastSample();
        return truthValues;
    }
}