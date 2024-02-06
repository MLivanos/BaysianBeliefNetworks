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
}

