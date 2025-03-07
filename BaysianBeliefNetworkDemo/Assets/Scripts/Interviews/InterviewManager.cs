using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class NodeDescriptions
{
    public Node node;
    public List<string> eventDescriptions;
    public List<string> eventNegationDescriptions;
}

public class InterviewManager : MonoBehaviour
{
    [SerializeField] private IntervieweeSpawner intervieweeSpawner;
    [SerializeField] private TimestepManager timestepManager;
    [SerializeField] private List<string> greetings;
    [SerializeField] private List<string> eventNames;
    [SerializeField] private List<NodeDescriptions> descriptions;
    [SerializeField] private List<string> friendlyDescriptions;
    [SerializeField] private List<string> aggressiveDescriptions;
    [SerializeField] private float friendlyBias;
    private InterviewCalculator calculator;
    private Recorder recorder;
    private Dictionary<string, NodeDescriptions> eventDictionary;
    private List<NodeDescriptions> seasons = new List<NodeDescriptions>();
    private List<NodeDescriptions> weather = new List<NodeDescriptions>();
    private List<NodeDescriptions> consequences = new List<NodeDescriptions>();
    private List<NodeDescriptions> humanActivity = new List<NodeDescriptions>();
    private List<NodeDescriptions> animalBehavior = new List<NodeDescriptions>();
    private List<List<NodeDescriptions>> nonSeasonEvents = new List<List<NodeDescriptions>>();
    private Graph graph;
    private InterviewUIManager uiManager;
    private string lastEventDescription = "";
    private string lastEventEvidence = "";
    private bool lastEventAggression;
    private bool lastEventBelieved;
    private bool hasSeason;
    private int eventCount = 0;
    private HashSet<string> evidenceCollected = new HashSet<string>();
    private int seasonIndex;
    private int stage=-1;
    private int numberOfStages = 5;
    private int totalNumberOfQuestions = 5;
    private int questionsSeen = 0;

    private void Start()
    {
        GetComponents();
        PopulateEventDictionary();
        StartCoroutine(InstantiateManager());
        Advance();
    }

    public void Advance()
    {
        stage++;
        stage %= numberOfStages;
        switch (stage)
        {
            case 0:
                intervieweeSpawner.SpawnInterviewee();
                break;
            case 1:
                DrawRandomEvents(2);
                uiManager.DisplayEyewitnessAccount(lastEventDescription);
                uiManager.DisplayEvidence(lastEventEvidence);
                break;
            case 2:
                intervieweeSpawner.DespawnInterviewee();
                break;
            case 3:
                timestepManager.Step();
                break;
            case 4:
                float eventProbability = calculator.CalculateProbability(0.98f, 15, 3);
                recorder.AddEntry(lastEventEvidence, eventProbability, lastEventBelieved, lastEventAggression);
                questionsSeen++;
                if (questionsSeen == totalNumberOfQuestions) EndInterviews();
                break;
            default:
                break;
        }
    }

    private void GetComponents()
    {
        graph = GameObject.Find("Graph").GetComponent<Graph>();
        calculator = GetComponent<InterviewCalculator>();
        recorder = GetComponent<Recorder>();
        uiManager = GetComponent<InterviewUIManager>();
        intervieweeSpawner.Initialize(this);
        recorder.Initialize(this);
        timestepManager.Initialize(this);
        uiManager.Initialize(this);
    }

    private void PopulateEventDictionary()
    {
        eventDictionary = new Dictionary<string, NodeDescriptions>();
        for (int i = 0; i < eventNames.Count; i++)
        {
            eventDictionary[eventNames[i]] = descriptions[i];
        }
    }

    private IEnumerator InstantiateManager()
    {
        yield return null;
        Dictionary<string, int> eventIndices = graph.GetComponent<Graph>().AssignIndices();
        List<Node> nodes = GameObject.Find("Graph").GetComponent<Graph>().GetAllNodes();
        AddToNodeTypeList(seasons, nodes[eventIndices["Winter"]], "Winter");
        AddToNodeTypeList(seasons, nodes[eventIndices["Spring"]], "Spring");
        AddToNodeTypeList(seasons, nodes[eventIndices["Summer"]], "Summer");
        AddToNodeTypeList(seasons, nodes[eventIndices["Fall"]], "Fall");
        AddToNodeTypeList(weather, nodes[eventIndices["APD"]], "APD");
        AddToNodeTypeList(weather, nodes[eventIndices["Wind"]], "Wind");
        AddToNodeTypeList(weather, nodes[eventIndices["Rain"]], "Rain");
        AddToNodeTypeList(weather, nodes[eventIndices["Cloudy"]], "Cloudy");
        AddToNodeTypeList(weather, nodes[eventIndices["Thunder"]], "Thunder");
        AddToNodeTypeList(consequences, nodes[eventIndices["Power"]], "Power");
        AddToNodeTypeList(consequences, nodes[eventIndices["Tree"]], "Tree");
        AddToNodeTypeList(humanActivity, nodes[eventIndices["Busy"]], "Busy");
        AddToNodeTypeList(humanActivity, nodes[eventIndices["Cafe"]], "Cafe");
        AddToNodeTypeList(animalBehavior, nodes[eventIndices["Dog"]], "Dog");
        AddToNodeTypeList(animalBehavior, nodes[eventIndices["Cat"]], "Cat");
        nonSeasonEvents = new List<List<NodeDescriptions>> {weather, consequences, humanActivity, animalBehavior};
        seasonIndex = nonSeasonEvents.Count;
        calculator.Initialize(graph.GetRootNodes(), eventIndices, graph.gameObject.GetComponent<LikelihoodWeightingSampler>());
        recorder.LogAlienProbability(GetAlienProbability());
    }

    private void AddToNodeTypeList(List<NodeDescriptions> list, Node node, string eventName)
    {
        eventDictionary[eventName].node = node;
        list.Add(eventDictionary[eventName]);
    }

    private List<NodeDescriptions> DrawRandomEventType(bool canBeSeason)
    {
        int index = (int)Mathf.Round(Random.Range(0, seasonIndex + 0.49f - (canBeSeason ? 0 : 1)));
        if (index == seasonIndex) return seasons;
        return nonSeasonEvents[index];
    }

    private (Node, string) DrawRandomEvent(List<NodeDescriptions> eventType, bool occurs)
    {
        int index = GetRandomIndex(eventType);
        List<string> eventList = occurs ? eventType[index].eventDescriptions : eventType[index].eventNegationDescriptions; 
        int stringIndex = GetRandomIndex(eventList);
        return (eventType[index].node, eventList[stringIndex]);
    }

    private int GetRandomIndex<T>(List<T> l)
    {
        return (int)Mathf.Round(Random.Range(0, l.Count - 0.51f));
    }

    private void DrawRandomEvents(int numberOfEvents)
    {
        ResetEventState();
        AppendGreeting();
        GenerateRandomEvents(numberOfEvents);
        AddAggressionDescription();
    }

    private void ResetEventState()
    {
        calculator.Reset();
        hasSeason = false;
        eventCount = 0;
        evidenceCollected.Clear();
        lastEventDescription = string.Empty;
        lastEventEvidence = string.Empty;
    }

    private void AppendGreeting()
    {
        lastEventDescription += greetings[GetRandomIndex(greetings)] + "\n";
    }

    private void GenerateRandomEvents(int numberOfEvents)
    {
        while (eventCount < numberOfEvents)
        {
            List<NodeDescriptions> eventType = DrawRandomEventType(!hasSeason);
            bool eventOccurs = DetermineIfEventOccurs(eventType);

            (Node node, string description) = DrawRandomEvent(eventType, eventOccurs);
            AddEventToDescription(node, description, eventOccurs);
        }
        lastEventEvidence = lastEventEvidence.Substring(0, lastEventEvidence.Length-1);
    }

    private bool DetermineIfEventOccurs(List<NodeDescriptions> eventType)
    {
        if (eventType == seasons)
        {
            hasSeason = true;
            return true;
        }
        return Random.value > 0.5f;
    }

    private void AddEventToDescription(Node node, string description, bool eventOccurs)
    {
        if (evidenceCollected.Contains(node.GetName())) return;

        lastEventDescription += description + "\n";
        evidenceCollected.Add(node.GetName());

        calculator.AddToEvidence(node, eventOccurs);
        lastEventEvidence += eventOccurs ? "" : "¬";
        lastEventEvidence += node.GetAbriviation() + ",";
        eventCount++;
    }

    private void AddAggressionDescription()
    {
        lastEventAggression = Random.value > friendlyBias;
        List<string> relevantList = lastEventAggression ? aggressiveDescriptions : friendlyDescriptions;
        lastEventDescription += relevantList[GetRandomIndex(relevantList)];
    }

    private float GetAlienProbability()
    {
        ResetEventState();
        return calculator.CalculateProbability(0.995f, 50, 5, 2.576f);
    }

    public void SetBelief(bool belief)
    {
        lastEventBelieved = belief;
    }

    private void EndInterviews()
    {
        recorder.DetermineBehavior(0.33f);
        recorder.StoreEndGameState();
    }
}