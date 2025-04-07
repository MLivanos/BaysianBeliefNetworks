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
    [SerializeField] private List<Node> rootNodes;
    [SerializeField] private GameObject gibbsOptions;
    [SerializeField] private GameObject hamiltonianOptions;
    [SerializeField] private GameObject graphUI;
    [SerializeField] private SamplingHistory queryRecord;
    [SerializeField] private bool test;
    private GraphUIManager graphUIManager;
    private AudioManager audioManager;
    private Sampler currentSampler;
    private Sampler[] samplers;
    private string queryText;
    private List<Node> allNodes;
    private List<Node> positiveQuery;
    private List<Node> negativeQuery;
    private List<Node> positiveEvidence;
    private List<Node> negativeEvidence;
    public static Graph instance;
    bool isNegative;
    float lastProbability;

    public List<Node> GetRootNodes() => rootNodes;
    public List<Node> GetPositiveEvidence() => positiveEvidence.ToList();
    public List<Node> GetNegativeEvidence() => negativeEvidence.ToList();
    public List<Node> GetPositiveQuery() => positiveQuery.ToList();
    public List<Node> GetNegativeQuery() => negativeQuery.ToList();
    public float GetLastProbability() => lastProbability;

    private void Awake()
    {
        SaveGraph();
        if (instance != null)
        {
            MigrateGraph();
        }
        instance = this;
    }

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        positiveQuery = new List<Node>();
        negativeQuery = new List<Node>();
        positiveEvidence = new List<Node>();
        negativeEvidence = new List<Node>();
        samplers = new Sampler[4];
        samplers[0] = GetComponent<RejectionSampler>();
        samplers[1] = GetComponent<LikelihoodWeightingSampler>();
        samplers[2] = GetComponent<GibbsSampler>();
        samplers[3] = GetComponent<HamiltonianSampler>();
        graphUIManager = GetComponent<GraphUIManager>();
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

    public void MigrateGraph()
    {
        List<Node> oldGraphNodes = instance.GetAllNodes();
        for(int i=0; i < allNodes.Count; i++)
        {
            allNodes[i].CopyNode(oldGraphNodes[i]);
            Destroy(oldGraphNodes[i].gameObject);
        }
        Destroy(instance.gameObject);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) isNegative = true;
        if(Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift)) isNegative = false;
        if (test)
        {
            GetComponent<GraphTester>().TestGraph();
            test = false;
        }
    }

    public void AddToEvidence(Node node, VariableChecks checks)
    {
        AddToEvidence(node, !isNegative);
        if (positiveQuery.Any(n => n == node) || negativeQuery.Any(n => n == node)) checks.SwitchQuery();
    }

    public void AddToQuery(Node node, VariableChecks checks)
    {
        AddToQuery(node, !isNegative);
        if (positiveEvidence.Any(n => n == node) || negativeEvidence.Any(n => n == node)) checks.SwitchEvidence();
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

    public void Sample()
    {
        if (!gameManager.CanSample()) return;
        if (currentSampler.Busy()) currentSampler.Interupt();
        else StartCoroutine(RunSamples());
    }

    private IEnumerator RunSamples()
    {
        audioManager.PlayEffect("Computation2");
        float startTime = Time.realtimeSinceStartup;
        graphUIManager.DisplayProgressBar();
        Coroutine samples = StartCoroutine(currentSampler.RunSamples());
        while (currentSampler.Busy())
        {
            graphUIManager.UpdateProgressBar(currentSampler.GetProgress());
            yield return null;
        }
        float timeElapsed = Time.realtimeSinceStartup - startTime;
        float probability = currentSampler.CalculateProbability();
        SamplingRecord record = new SamplingRecord(
            GetPositiveQuery(), GetNegativeQuery(),
            GetPositiveEvidence(), GetNegativeEvidence(),
            probability,
            currentSampler.GetNumberOfSamples(),
            currentSampler.GetType().Name
        );
        queryRecord.AddRecord(record);
        lastProbability = probability;
        currentSampler.AddTime(timeElapsed);
        gameManager.UpdateTimer(-timeElapsed);
        graphUIManager.UpdateUI(currentSampler.GetNumberOfSamples(), currentSampler.GetNumberOfAcceptedSamples(), currentSampler.GetTimeElapsed());
        FindObjectOfType<SaveSystem>().SaveGame();
    }

    public void UpdateText(float probability=-1f)
    {
        graphUIManager.UpdateText(probability);
    }

    public void ChangeSampler(int index)
    {
        currentSampler = samplers[index];
        gibbsOptions.SetActive(index == 2);
        hamiltonianOptions.SetActive(index == 3);
    }

    public void SetNumberOfSamples(int samples)
    {
        foreach(Sampler sampler in samplers)
        {
            sampler.SetNumberOfSamples(samples);
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

    public void ClearGraph()
    {
        UncheckAllCheckboxes(graphUI);
        positiveQuery.Clear();
        negativeQuery.Clear();
        positiveEvidence.Clear();
        negativeEvidence.Clear();
    }

    public void ClearSamples()
    {
        GetComponent<RejectionSampler>().Reset();
        GetComponent<LikelihoodWeightingSampler>().Reset();
        GetComponent<GibbsSampler>().Reset();
        GetComponent<HamiltonianSampler>().Reset();
    }

    private void UncheckAllCheckboxes(GameObject root)
    {
        Toggle toggle = root.GetComponent<Toggle>();
        if (toggle != null) toggle.isOn = false;
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