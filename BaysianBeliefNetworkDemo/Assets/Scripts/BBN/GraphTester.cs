using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphTester : MonoBehaviour
{
    private List<Node> allNodes;
    private Graph graph;

    private void Start()
    {
        graph = gameObject.GetComponent<Graph>();
    }

    public void TestGraph()
    {
        foreach (Node node in graph.GetAllNodes())
        {
            Debug.Log("=======");
            Debug.Log("Testing node: " + node.name);
            List<Node> parents = node.GetParents().ToList();
            graph.AddToQuery(node, true);
            TestNodeWithAllParentCombinations(node, parents, new Dictionary<Node, bool>(), 0);
            graph.ClearGraph();
            Debug.Log("=======");
        }
    }
    
    private void TestNodeWithAllParentCombinations(Node node, List<Node> parents, Dictionary<Node, bool> evidence, int parentIndex)
    {
        if (parentIndex == parents.Count)
        {
            // All parents have been assigned a true/false value, now test this combination
            foreach (var kvp in evidence)
            {
                if (kvp.Value)
                {
                    graph.AddToEvidence(kvp.Key, true);
                }
                else
                {
                    graph.AddToEvidence(kvp.Key, false);
                }
                Debug.Log($"Given: {(kvp.Value ? "" : "Not ")}{kvp.Key.GetName()}");
            }
            graph.Sample();
            Debug.Log($"P({node.name}) = {graph.GetLastProbability()}");
            graph.ClearGraph();
            graph.AddToQuery(node, true);
        }
        else
        {
            Node currentParent = parents[parentIndex];
            // Set current parent to true
            evidence[currentParent] = true;
            TestNodeWithAllParentCombinations(node, parents, evidence, parentIndex + 1);
            // Set current parent to false
            evidence[currentParent] = false;
            TestNodeWithAllParentCombinations(node, parents, evidence, parentIndex + 1);
        }
    }
}