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
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        queryToggle = transform.Find("QueryCheckbox").GetComponent<Toggle>();
        evidenceToggle = transform.Find("EvidenceCheckbox").GetComponent<Toggle>();
        graphObject = Graph.instance.gameObject;
        graph = graphObject.GetComponent<Graph>();
        queryToggle.onValueChanged.AddListener(delegate {ChangeQuery(); });
        evidenceToggle.onValueChanged.AddListener(delegate {ChangeEvidence(); });
    }

    private void ChangeQuery()
    {
        if (queryToggle.isOn)
        {
            audioManager.PlayEffect("ClickOn");
            graph.AddToQuery(node, this);
        }
        else
        {
            audioManager.PlayEffect("ClickOff");
            graph.RemoveFromQuery(node);
        }
        graph.UpdateText();
    }

    private void ChangeEvidence()
    {
        if (evidenceToggle.isOn)
        {
            audioManager.PlayEffect("ClickOn");
            graph.AddToEvidence(node, this);
        }
        else
        {
            audioManager.PlayEffect("ClickOff");
            graph.RemoveFromEvidence(node);
        }
        graph.UpdateText();
    }

    public void SwitchEvidence()
    {
        SwitchToggle(evidenceToggle);
    }

    public void SwitchQuery()
    {
        SwitchToggle(queryToggle);
    }

    private void SwitchToggle(Toggle toggle)
    {
        toggle.isOn = !toggle.isOn;
    }
}
