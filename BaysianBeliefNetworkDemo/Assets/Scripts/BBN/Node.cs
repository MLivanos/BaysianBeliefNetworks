using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] private string nodeName;
    [SerializeField] private string abriviation;
    [SerializeField] List<Node> children;
    [SerializeField] List<Node> parents;
    [SerializeField] float[] jointProbabilityDistribution;
    private List<ProbabilityDisplay> displays = new List<ProbabilityDisplay>();
    bool isTrue;
    bool isSet;

    public float[] JointProbabilityDistribution() => jointProbabilityDistribution;

    public void CopyNode(Node other)
    {
        this.jointProbabilityDistribution = (float[])other.JointProbabilityDistribution().Clone();
    }

    public void AddDisplay(ProbabilityDisplay display)
    {
        displays.Add(display);
    }

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
        isSet = true;
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

    public bool IsSet()
    {
        return isSet;
    }

    public void IsSet(bool set)
    {
        isSet = set;
    }

    public string GetName()
    {
        return name;
    }

    public string GetAbriviation()
    {
        return abriviation;
    }

    public bool IsReadyToCalculateProbability()
    {
        foreach(Node parent in parents)
        {
            if (!parent.IsSet())
            {
                return false;
            }
        }
        return true;
    }

    public void SetProbability(float probability, int index)
    {
        jointProbabilityDistribution[index] = probability;
    }

    public float[] GetCPT()
    {
        return jointProbabilityDistribution;
    }

    public void SetJointProbabilityDistribution(float[] newJPD)
    {
        jointProbabilityDistribution = newJPD;
        foreach(ProbabilityDisplay display in displays)
        {
            display.RefreshDisplay();
        }
    }
}

