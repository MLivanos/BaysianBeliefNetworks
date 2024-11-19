using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Graph : MonoBehaviour
{
    [SerializeField] private GameObject graphUI;
    [SerializeField] private List<Node> rootNodes;
    [SerializeField] private TMP_Text queryTextDisplay;
    [SerializeField] private TMP_Text sampleInfo;
    [SerializeField] private TMP_Text calculateText;
    [SerializeField] private GameObject gibbsOptions;
    [SerializeField] private GameObject hamiltonianOptions;
    [SerializeField] private Slider progressBar;
    [SerializeField] private bool test;
    private Sampler currentSampler;
    private Sampler[] samplers;
    private string queryText;
    private List<Node> allNodes;
    private List<Node> positiveQuery;
    private List<Node> negativeQuery;
    private List<Node> positiveEvidence;
    private List<Node> negativeEvidence;
    bool isNegative;
    bool hasTested;
    float lastProbability;

    private void Start()
    {
        positiveQuery = new List<Node>();
        negativeQuery = new List<Node>();
        positiveEvidence = new List<Node>();
        negativeEvidence = new List<Node>();
        SaveGraph();
        samplers = new Sampler[4];
        samplers[0] = GetComponent<RejectionSampler>();
        samplers[1] = GetComponent<LikelihoodWeightingSampler>();
        samplers[2] = GetComponent<GibbsSampler>();
        samplers[3] = GetComponent<HamiltonianSampler>();

        currentSampler = samplers[0];
    }

    private void SaveGraph()
    {
        DontDestroyOnLoad(gameObject);
        List<Node> currentNodes;
        currentNodes = rootNodes.ToList();
        allNodes = rootNodes.ToList();
        while(currentNodes.Count > 0)
        {
            Node node = currentNodes[0];
            foreach(Node child in node.GetChildren())
            {
                if (!allNodes.Contains(child))
                {
                    allNodes.Add(child);
                    currentNodes.Add(child);
                }
            }
            currentNodes.RemoveAt(0);
            DontDestroyOnLoad(node);
        }
    }

    public void UnsaveGraph()
    {
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
        List<Node> currentNodes;
        currentNodes = rootNodes.ToList();
        while(currentNodes.Count > 0)
        {
            Node node = currentNodes[0];
            foreach(Node child in node.GetChildren())
            {
                if (!currentNodes.Contains(child))
                {
                    currentNodes.Add(child);
                }
            }
            currentNodes.RemoveAt(0);
            SceneManager.MoveGameObjectToScene(node.gameObject, SceneManager.GetActiveScene());
        }
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
        if (test && !hasTested)
        {
            TestGraph();
        }
    }

    public List<Node> GetRootNodes()
    {
        return rootNodes;
    }

    public void AddToEvidence(Node node, VariableChecks checks)
    {
        List<Node> relevantList = isNegative ? negativeEvidence : positiveEvidence;
        relevantList.Add(node);
        if (positiveQuery.Any(n => n == node) || negativeQuery.Any(n => n == node))
        {
            checks.SwitchQuery();
        }
        // These samplers clamp evidence, so prior samples are invalid
        GetComponent<LikelihoodWeightingSampler>().Reset();
        GetComponent<GibbsSampler>().Reset();
        GetComponent<HamiltonianSampler>().Reset();
    }

    public void AddToQuery(Node node, VariableChecks checks)
    {
        List<Node> relevantList = isNegative ? negativeQuery : positiveQuery;
        relevantList.Add(node);
        if (positiveEvidence.Any(n => n == node) || negativeEvidence.Any(n => n == node))
        {
            checks.SwitchEvidence();
        }
        GetComponent<LikelihoodWeightingSampler>().Reset();
    }

    public void RemoveFromEvidence(Node node)
    {
        List<Node> relevantList = positiveEvidence;
        if (negativeEvidence.Any(n => n == node))
        {
            relevantList = negativeEvidence;
        }
        relevantList.Remove(node);
        GetComponent<LikelihoodWeightingSampler>().Reset();
        GetComponent<GibbsSampler>().Reset();
        GetComponent<HamiltonianSampler>().Reset();
    }

    public void RemoveFromQuery(Node node)
    {
        List<Node> relevantList = positiveQuery;
        if (negativeQuery.Any(n => n == node))
        {
            relevantList = negativeQuery;
        }
        relevantList.Remove(node);
        GetComponent<LikelihoodWeightingSampler>().Reset();
    }

    public void UpdateText(float probabilityValue=-1.0f)
    {
        if (queryTextDisplay == null)
        {
            queryTextDisplay = GameObject.Find("QueryText").GetComponent<TMP_Text>();
        }
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

    public void Sample()
    {
        if (currentSampler.Busy()) currentSampler.Interupt();
        else StartCoroutine(RunSamples());
    }

    private IEnumerator RunSamples()
    {
        float startTime = Time.realtimeSinceStartup;
        progressBar.gameObject.SetActive(true);
        queryTextDisplay.gameObject.SetActive(false);
        calculateText.text = "Stop";
        Coroutine samples = StartCoroutine(currentSampler.RunSamples());
        while (currentSampler.Busy())
        {
            progressBar.value = currentSampler.GetProgress();
            yield return null;
        }
        float probability = currentSampler.CalculateProbability();
        lastProbability = probability;
        currentSampler.AddTime(Time.realtimeSinceStartup - startTime);
        UpdateUI();
    }

    private void UpdateUI()
    {
        progressBar.gameObject.SetActive(false);
        queryTextDisplay.gameObject.SetActive(true);
        calculateText.text = "Calculate";
        int numberOfSamples = currentSampler.GetNumberOfSamples();
        int numberOfAcceptedSamples = currentSampler.GetNumberOfAcceptedSamples();
        float acceptanceRatio = 100f * numberOfAcceptedSamples / numberOfSamples;
        float timeElapsed = currentSampler.GetTimeElapsed();

        string sampleInfoText = string.Format(
            "{0}\n{1} ({2:F2}%)\n{3:F2}s",
            numberOfSamples,
            numberOfAcceptedSamples,
            acceptanceRatio,
            timeElapsed
        );
        sampleInfo.text = sampleInfoText;
        UpdateText(lastProbability);
    }

    public void ChangeSampler(int index)
    {
        currentSampler = samplers[index];
        gibbsOptions.SetActive(index == 2);
        hamiltonianOptions.SetActive(index == 3);
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
        foreach(Sampler sampler in samplers)
        {
            sampler.SetNumberOfSamples(Int32.Parse(numberOfSamplesText));
        }
    }

    public bool[] VisualizeSample()
    {
        GibbsSampler sampler = GetComponent<GibbsSampler>();
        sampler.GatherEvidence();
        sampler.Sample();
        bool[] truthValues = sampler.GetLastSample();
        return truthValues;
    }

    public List<Node> GetAllNodes()
    {
        return allNodes;
    }

    private void TestGraph()
    {
        foreach (Node node in allNodes)
        {
            Debug.Log("=======");
            Debug.Log("Testing node: " + node.name);
            // Test all combinations of true/false for each parent
            List<Node> parents = node.GetParents().ToList();
            positiveQuery.Add(node);
            TestNodeWithAllParentCombinations(node, parents, new Dictionary<Node, bool>(), 0);
            positiveQuery.Clear();
            Debug.Log("=======");
        }
        hasTested = true;
    }

    // Recursive function to test all combinations of parent values
    private void TestNodeWithAllParentCombinations(Node node, List<Node> parents, Dictionary<Node, bool> evidence, int parentIndex)
    {
        if (parentIndex == parents.Count)
        {
            // All parents have been assigned a true/false value, now test this combination
            foreach (var kvp in evidence)
            {
                if (kvp.Value)
                {
                    positiveEvidence.Add(kvp.Key);
                }
                else
                {
                    negativeEvidence.Add(kvp.Key);
                }
                Debug.Log($"Given: {(kvp.Value ? "" : "Not ")}{kvp.Key.GetName()}");
            }

            // Sample the probability with this evidence combination
            Sample();
            Debug.Log($"P({node.name}) = {lastProbability}");

            // Clear evidence for next combination
            positiveEvidence.Clear();
            negativeEvidence.Clear();
        }
        else
        {
            // For each parent, recursively assign true/false and test
            Node currentParent = parents[parentIndex];

            // Set current parent to true
            evidence[currentParent] = true;
            TestNodeWithAllParentCombinations(node, parents, evidence, parentIndex + 1);

            // Set current parent to false
            evidence[currentParent] = false;
            TestNodeWithAllParentCombinations(node, parents, evidence, parentIndex + 1);
        }
    }

    public void ClearGraph()
    {
        UncheckAllCheckboxes(graphUI);
    }

    private void UncheckAllCheckboxes(GameObject root)
    {
        Toggle toggle = root.GetComponent<Toggle>();
        if (toggle != null)
        {
            toggle.isOn = false;
        }
        foreach (Transform child in root.transform)
        {
            UncheckAllCheckboxes(child.gameObject);
        }
    }

    public Dictionary<string,int> GetNodeOrder()
    {
        Dictionary<string,int> order = new Dictionary<string,int>();
        for(int i=0; i<allNodes.Count; i++)
        {
            order[allNodes[i].name] = i;
        }
        return order;
    }

}