using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Graph : MonoBehaviour
{
    [SerializeField] private List<Node> rootNodes;
    [SerializeField] private TMP_Text queryTextDisplay;
    private RejectionSampler rejectionSampler;
    private string queryText;
    private List<Node> positiveQuery = new List<Node>();
    private List<Node> negativeQuery = new List<Node>();
    private List<Node> positiveEvidence = new List<Node>();
    private List<Node> negativeEvidence = new List<Node>();
    bool isNegative;

    private void Start()
    {
        rejectionSampler = GetComponent<RejectionSampler>();
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

    public void AddToEvidence(Node node)
    {
        List<Node> relevantList = isNegative ? negativeEvidence : positiveEvidence;
        relevantList.Add(node);
    }

    public void AddToQuery(Node node)
    {
        List<Node> relevantList = isNegative ? negativeQuery : positiveQuery;
        relevantList.Add(node);
    }

    public void RemoveFromEvidence(Node node)
    {
        List<Node> relevantList = positiveEvidence;
        if (negativeEvidence.Any(n => n == node))
        {
            relevantList = negativeEvidence;
        }
        relevantList.Remove(node);
    }

    public void RemoveFromQuery(Node node)
    {
        List<Node> relevantList = positiveQuery;
        if (negativeQuery.Any(n => n == node))
        {
            relevantList = negativeQuery;
        }
        relevantList.Remove(node);
    }

    public void UpdateText()
    {
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
    }
}