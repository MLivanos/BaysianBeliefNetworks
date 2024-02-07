using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariableChecks : MonoBehaviour
{
    [SerializeField] private Node node;
    private Toggle queryToggle;
    private Toggle evidenceToggle;
    private GameObject graphObject;
    private Graph graph;

    private void Start()
    {
        queryToggle = transform.Find("QueryCheckbox").GetComponent<Toggle>();
        evidenceToggle = transform.Find("EvidenceCheckbox").GetComponent<Toggle>();
        graphObject = GameObject.Find("Graph");
        graph = graphObject.GetComponent<Graph>();
        queryToggle.onValueChanged.AddListener(delegate {ChangeQuery(); });
        evidenceToggle.onValueChanged.AddListener(delegate {ChangeEvidence(); });
    }

    private void ChangeQuery()
    {
        if (queryToggle.isOn)
        {
            graph.AddToQuery(node);
        }
        else
        {
            graph.RemoveFromQuery(node);
        }
        graph.UpdateText();
    }

    private void ChangeEvidence()
    {
        if (evidenceToggle.isOn)
        {
            graph.AddToEvidence(node);
        }
        else
        {
            graph.RemoveFromEvidence(node);
        }
        graph.UpdateText();
    }
}
