using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text queryTextDisplay;
    [SerializeField] private TMP_Text sampleInfo;
    [SerializeField] private TMP_Text calculateText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private int maxNumberOfNoises = 2;
    private int numberOfNoises = 0;
    private AudioManager audioManager;
    private Graph graph;
    private GraphActionRecorder graphActionRecorder;
    private string queryText;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        graph = gameObject.GetComponent<Graph>();
        graphActionRecorder = gameObject.GetComponent<GraphActionRecorder>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) graph.Sample();
        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.C)) graph.ClearGraph();
        if (UndoButtonsPressded())
        {
            graphActionRecorder.Undo();
            UpdateText();
        }
        if (RedoButtonsPressed())
        {
            graphActionRecorder.Redo();
            UpdateText();
        }
    }

    public void UpdateText(float probabilityValue=-1.0f)
    {
        List<Node> positiveQuery = graph.GetPositiveQuery();
        List<Node> negativeQuery = graph.GetNegativeQuery();
        List<Node> positiveEvidence = graph.GetPositiveEvidence();
        List<Node> negativeEvidence = graph.GetNegativeEvidence();

        if (queryTextDisplay == null)
        {
            queryTextDisplay = GameObject.Find("QueryText").GetComponent<TMP_Text>();
        }
        queryText = "P(";
        queryText += GetPartialQuery(graph.GetPositiveQuery());
        if (positiveQuery.Count > 0 && negativeQuery.Count > 0)
        {
            queryText += ",";
        }
        queryText += GetPartialQuery(graph.GetNegativeQuery(), true);
        if (positiveEvidence.Count + negativeEvidence.Count > 0)
        {
            queryText += "|";
        }
        queryText += GetPartialQuery(graph.GetPositiveEvidence());
        if (positiveEvidence.Count > 0 && negativeEvidence.Count > 0)
        {
            queryText += ",";
        }
        queryText += GetPartialQuery(graph.GetNegativeEvidence(), true);
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
    
    public void UpdateUI(int numberOfSamples, int numberOfAcceptedSamples, float timeElapsed)
    {
        progressBar.gameObject.SetActive(false);
        queryTextDisplay.gameObject.SetActive(true);
        calculateText.text = "Calculate";
        float acceptanceRatio = 100f * numberOfAcceptedSamples / numberOfSamples;

        string sampleInfoText = string.Format(
            "{0}\n{1} ({2:F2}%)\n{3:F2}s",
            numberOfSamples,
            numberOfAcceptedSamples,
            acceptanceRatio,
            timeElapsed
        );
        sampleInfo.text = sampleInfoText;
        UpdateText(graph.GetLastProbability());
    }

    public void DisplayProgressBar()
    {
        numberOfNoises = 0;
        progressBar.gameObject.SetActive(true);
        queryTextDisplay.gameObject.SetActive(false);
        calculateText.text = "Stop";
    }

    public void UpdateProgressBar(float newProgress)
    {
        if (numberOfNoises++ < maxNumberOfNoises) audioManager.PlayEffect("Computation1");
        progressBar.value = newProgress;
    }

    private bool RedoButtonsPressed()
    {
        return (Input.GetKeyDown(KeyCode.Y) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))) ||
            (Input.GetKeyDown(KeyCode.Z) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) &&
            (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)));
    }

    private bool UndoButtonsPressded()
    {
        return Input.GetKeyDown(KeyCode.Z) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl));
    }
}