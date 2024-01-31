using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] List<Node> children;
    [SerializeField] List<Node> parents;
    [SerializeField] float[] jointProbabilityDistribution;
    bool isTrue;

    public float Query()
    {
        int index = 0;
        int parentIndex = 0;
        foreach(Node parent in parents)
        {
            if(parent.IsTrue())
            {
                index += (int)(Mathf.Pow(2, parents.Count-(parentIndex+1)));
            }
            parentIndex ++;
        }
        return jointProbabilityDistribution[index];
    }

    public bool Query(float threshold)
    {
        return threshold < Query();
    }

    public bool IsTrue()
    {
        return isTrue;
    }

    public void IsTrue(bool truthValue)
    {
        isTrue = truthValue;
    }

    public void AddChild(Node child)
    {
        children.Add(child);
    }

    public List<Node> GetChildren()
    {
        return children;
    }

    public List<Node> GetParents()
    {
        return parents;
    }
}

