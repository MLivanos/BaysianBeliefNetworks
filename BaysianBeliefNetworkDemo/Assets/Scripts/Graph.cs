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
    [SerializeField] private GameManager gameManager;
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
            GetComponent<GraphTester>().TestGraph();
            hasTested = true;
        }
    }

    public List<Node> GetRootNodes()
    {
        return rootNodes;
    }

    public void AddToEvidence(Node node, VariableChecks checks)
    {
        AddToEvidence(node, !isNegative);
        if (positiveQuery.Any(n => n == node) || negativeQuery.Any(n => n == node))
        {
            checks.SwitchQuery();
        }
    }

    public void AddToQuery(Node node, VariableChecks checks)
    {
        AddToQuery(node, !isNegative);
        if (positiveEvidence.Any(n => n == node) || negativeEvidence.Any(n => n == node))
        {
            checks.SwitchEvidence();
        }
    }

    public void AddToQuery(Node node, bool isTrue)
    {
        List<Node> relevantList = isTrue ? positiveQuery : negativeQuery;
        relevantList.Add(node);
        GetComponent<LikelihoodWeightingSampler>().Reset();
    }

    public void AddToEvidence(Node node, bool isTrue)
    {
        List<Node> relevantList = isTrue ? positiveEvidence : negativeEvidence;
        relevantList.Add(node);
        GetComponent<LikelihoodWeightingSampler>().Reset();
        GetComponent<GibbsSampler>().Reset();
        GetComponent<HamiltonianSampler>().Reset();
    }

    public void RemoveFromEvidence(Node node)
    {
        List<Node> relevantList = negativeEvidence.Any(n => n == node) ? negativeEvidence : positiveEvidence;
        relevantList.Remove(node);
        GetComponent<LikelihoodWeightingSampler>().Reset();
        GetComponent<GibbsSampler>().Reset();
        GetComponent<HamiltonianSampler>().Reset();
    }

    public void RemoveFromQuery(Node node)
    {
        List<Node> relevantList = negativeQuery.Any(n => n == node) ? negativeQuery : positiveQuery;
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
        if (!gameManager.CanSample()) return;
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
        float timeElapsed = Time.realtimeSinceStartup - startTime;
        float probability = currentSampler.CalculateProbability();
        lastProbability = probability;
        currentSampler.AddTime(timeElapsed);
        gameManager.UpdateTimer(-timeElapsed);
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

    public float GetLastProbability()
    {
        return lastProbability;
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

    public void ClearGraph()
    {
        UncheckAllCheckboxes(graphUI);
        positiveQuery.Clear();
        negativeQuery.Clear();
        positiveEvidence.Clear();
        negativeEvidence.Clear();
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

    public Dictionary<string, int> AssignIndices()
    {
        Dictionary<string, int> nodeOrder = GetNodeOrder();
        Dictionary<string, int> eventIndices = new Dictionary<string, int>
        {
            { "Winter", nodeOrder["WinterNode"] },
            { "Spring", nodeOrder["SpringNode"] },
            { "Summer", nodeOrder["SummerNode"] },
            { "Fall", nodeOrder["FallNode"] },
            { "APD", nodeOrder["AtmosphericPressureDropNode"] },
            { "Cloudy", nodeOrder["CloudNode"] },
            { "Rain", nodeOrder["RainNode"] },
            { "Wind", nodeOrder["HighWindNode"] },
            { "Power", nodeOrder["PowerOutageNode"] },
            { "Tree", nodeOrder["TreeNode"] },
            { "Busy", nodeOrder["BusyNode"] },
            { "Thunder", nodeOrder["ThunderNode"] },
            { "Cafe", nodeOrder["CafeNode"] },
            { "Alien", nodeOrder["AlienNode"] },
            { "Dog", nodeOrder["DogNode"] },
            { "Cat", nodeOrder["CatNode"] }
        };
        return eventIndices;
    }
}