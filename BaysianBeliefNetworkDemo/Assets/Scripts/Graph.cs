using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Graph : MonoBehaviour
{
    [SerializeField] private List<Node> rootNodes;
    [SerializeField] private TMP_Text queryTextDisplay;
    private string queryText;
    private List<Node> positiveQuery = new List<Node>();
    private List<Node> negativeQuery = new List<Node>();
    private List<Node> positiveEvidence = new List<Node>();
    private List<Node> negativeEvidence = new List<Node>();
    bool isNegative;

    private void Update()
    {
        if(Input.GetKeyDown("s"))
        {
            Sample();
        }
        if(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            isNegative = true;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            isNegative = false;
        }
    }

    private void Sample()
    {
        Debug.Log("===========");
        int[] counts = new int[10];
        string[] names = new string[10];
        List<Node> currentNodes;
        for(int i=0; i<10000; i++)
        {
            int index = 0;
            currentNodes = rootNodes.ToList();
            while(currentNodes.Count > 0)
            {
                Node node = currentNodes[0];
                currentNodes.RemoveAt(0);
                if (node.IsReadyToCalculateProbability())
                {
                    node.IsTrue(node.Query(Random.value));
                    if (node.IsTrue())
                    {
                        counts[index] ++;
                        names[index] = node.GetName();
                    }
                    AddChildren(currentNodes, node.GetChildren());
                    index++;
                }
            }
        }
        for(int i = 0; i<counts.Count(); i++)
        {
            Debug.Log(names[i]);
            Debug.Log(counts[i]);
        }
    }

    private void AddChildren(List<Node> currentNodes, List<Node> children)
    {
        foreach(Node child in children)
        {
            if (!currentNodes.Contains(child))
            {
                currentNodes.Add(child);
            }
        }
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
        if (negativeQuery.Count > 0)
        {
            queryText += ",";
        }
        queryText += GetPartialQuery(negativeQuery, true);
        if (positiveEvidence.Count + negativeEvidence.Count > 0)
        {
            queryText += "|";
        }
        queryText += GetPartialQuery(positiveEvidence);
        if (negativeEvidence.Count > 0)
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
}