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
    [SerializeField] private List<string> greetings;
    [SerializeField] private List<string> eventNames;
    [SerializeField] private List<NodeDescriptions> descriptions;
    [SerializeField] private List<string> friendlyDescriptions;
    [SerializeField] private List<string> aggressiveDescriptions;
    [SerializeField] private float friendlyBias;
    private Dictionary<string, NodeDescriptions> eventDictionary;
    private List<NodeDescriptions> seasons = new List<NodeDescriptions>();
    private List<NodeDescriptions> weather = new List<NodeDescriptions>();
    private List<NodeDescriptions> consequences = new List<NodeDescriptions>();
    private List<NodeDescriptions> humanActivity = new List<NodeDescriptions>();
    private List<NodeDescriptions> animalBehavior = new List<NodeDescriptions>();
    private List<List<NodeDescriptions>> nonSeasonEvents = new List<List<NodeDescriptions>>();
    private LikelihoodWeightingSampler sampler;
    private Graph graph;
    private Dictionary<string, int> eventIndices;
    private string lastEventDescription = "";
    private string lastEventEvidence = "";
    private bool lastEventAggression;
    private bool hasSeason;
    private int eventCount = 0;
    private HashSet<string> evidenceCollected = new HashSet<string>();
    private int seasonIndex;

    private void Start()
    {
        eventDictionary = new Dictionary<string, NodeDescriptions>();
        for (int i = 0; i < eventNames.Count; i++)
        {
            eventDictionary[eventNames[i]] = descriptions[i];
        }
        graph = GameObject.Find("Graph").GetComponent<Graph>();
        sampler = graph.gameObject.GetComponent<LikelihoodWeightingSampler>();
        StartCoroutine(InstantiateManager());
    }

    private IEnumerator InstantiateManager()
    {
        yield return null;
        eventIndices = GameObject.Find("Graph").GetComponent<Graph>().AssignIndices();
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
        //Debug.Log(GetAlienProbability());
        DrawRandomEvents(2);
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

    private (Node, string) DrawRandomEvent(List<NodeDescriptions> eventType)
    {
        int index = GetRandomIndex(eventType);
        int stringIndex = GetRandomIndex(eventType[index].eventDescriptions);
        return (eventType[index].node, eventType[index].eventDescriptions[stringIndex]);
    }

    private int GetRandomIndex<T>(List<T> l)
    {
        return (int)Mathf.Round(Random.Range(0, l.Count - 0.51f));
    }

    private float CalculateProbability(int numberOfSamples=10000)
    {
        int[] positiveQuery = new int[1] {eventIndices["Alien"]};
        int[] negativeQuery = new int[0];
        for (int i = 0; i < numberOfSamples; i++)
        {
            sampler.Sample(positiveQuery, negativeQuery, graph.GetRootNodes().ToList());
        }
        return sampler.CalculateProbability();
    }

    private float CalculateProbability(float precision, int maxIterations, int windowSize, float z=1.96f)
    {
        List<float> slidingWindow = new List<float>();
        for (int i = 0; i < windowSize - 1; i++)
        {
            slidingWindow.Add(CalculateProbability());
        }
        slidingWindow.Add(0f);
        float ciWidth = 0f;
        for(int i = windowSize - 1; i < maxIterations; i++)
        {
            slidingWindow[i % windowSize] = CalculateProbability();
            float mean = slidingWindow.Average();
            float std = Mathf.Sqrt(slidingWindow.Select(p => (p - mean) * (p - mean)).Average());

            float standardError = std / Mathf.Sqrt(windowSize);
            ciWidth = z * standardError;

            if (ciWidth < slidingWindow[i % windowSize] * (1 - precision))
            {
                return slidingWindow[i % windowSize];
            }
        }
        return slidingWindow[(maxIterations-1)%windowSize];
    }

    private void DrawRandomEvents(int numberOfEvents)
    {
        ResetEventState();
        AppendGreeting();
        GenerateRandomEvents(numberOfEvents);
        AddAggressionDescription();
        LogEventDetails();
    }

    private void ResetEventState()
    {
        sampler.ClearEvidence();
        sampler.Reset();

        hasSeason = false;
        eventCount = 0;
        evidenceCollected.Clear();
        lastEventDescription = string.Empty;
        lastEventEvidence = string.Empty;
    }

    private void AppendGreeting()
    {
        lastEventDescription += greetings[GetRandomIndex(greetings)];
    }

    private void GenerateRandomEvents(int numberOfEvents)
    {
        while (eventCount < numberOfEvents)
        {
            List<NodeDescriptions> eventType = DrawRandomEventType(!hasSeason);
            bool eventOccurs = DetermineIfEventOccurs(eventType);

            (Node node, string description) = DrawRandomEvent(eventType);
            AddEventToDescription(node, description, eventOccurs);
        }
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

        sampler.AddToEvidence(node, eventOccurs);
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

    private void LogEventDetails()
    {
        Debug.Log(lastEventEvidence);
        Debug.Log(lastEventAggression);
        Debug.Log(CalculateProbability(0.98f, 15, 3));
    }
    private float GetAlienProbability()
    {
        ResetEventState();
        return CalculateProbability(0.995f, 50, 5, 2.576f);
    }
}