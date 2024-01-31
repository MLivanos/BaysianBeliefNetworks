using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField] private List<Node> rootNodes;

    private void Update()
    {
        if(Input.GetKeyDown("s"))
        {
            Sample();
        }
    }

    private void Sample()
    {
        Debug.Log("===========");
        List<Node> currentNodes = rootNodes.ToList();
        while(currentNodes.Count > 0)
        {
            Node node = currentNodes[0];
            currentNodes.RemoveAt(0);
            if (node.IsReadyToCalculateProbability())
            {
                node.IsTrue(node.Query(Random.value));
                Debug.Log(node.IsTrue());
                AddChildren(currentNodes, node.GetChildren());
            }
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
}

