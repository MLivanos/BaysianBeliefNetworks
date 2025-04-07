using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class NodeDescriptions
{
    public Node node;
    public List<string> eventDescriptions;
    public List<string> eventNegationDescriptions;
}

/// <summary>
/// Contains only the core event-drawing logic:
/// - Drawing a random event type
/// - Drawing a random event from that type
/// - Aggregating events and resetting state
/// </summary>
public class EventDrawer : MonoBehaviour
{
    // Private lists used solely for event drawing.
    [SerializeField] private List<string> greetings;
    [SerializeField] private List<string> eventNames;
    [SerializeField] private List<NodeDescriptions> descriptions;
    [SerializeField] private List<string> friendlyDescriptions;
    [SerializeField] private List<string> aggressiveDescriptions;
    [SerializeField] private float friendlyBias;

    private List<NodeDescriptions> seasons = new List<NodeDescriptions>();
    private List<NodeDescriptions> weather = new List<NodeDescriptions>();
    private List<NodeDescriptions> consequences = new List<NodeDescriptions>();
    private List<NodeDescriptions> humanActivity = new List<NodeDescriptions>();
    private List<NodeDescriptions> animalBehavior = new List<NodeDescriptions>();
    private List<NodeDescriptions> markovBlanket = new List<NodeDescriptions>();
    private List<List<NodeDescriptions>> nonSeasonEvents = new List<List<NodeDescriptions>>();
    private Dictionary<string, NodeDescriptions> eventDictionary;
    private InterviewManager manager;
    private QuestionTracker questionRepo = new QuestionTracker(20);

    // Internal state for drawing events
    public List<(Node node, bool occurs)> DrawnNodes { get; private set; } = new List<(Node, bool)>();
    private bool eventAggression;
    private int seasonIndex;
    private int eventCount = 0;
    private HashSet<string> evidenceCollected = new HashSet<string>();
    private string rawEventDescription = "";
    private string rawEventEvidence = "";

    /// <summary>
    /// Initializes the event drawer by building the event dictionary and populating the lists of events.
    /// </summary>
    public void Initialize(InterviewManager interviewManager)
    {
        manager = interviewManager;
        Graph graph = GameObject.Find("Graph").GetComponent<Graph>();
        Dictionary<string, int> eventIndices = graph.GetComponent<Graph>().AssignIndices();
        List<Node> nodes = graph.GetAllNodes();
        eventDictionary = new Dictionary<string, NodeDescriptions>();
        PopulateEventDictionary();
        // Populate event-type lists
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
        PopulateMarkovBlanket();
        nonSeasonEvents = new List<List<NodeDescriptions>> { weather, consequences, humanActivity, animalBehavior };
        seasonIndex = nonSeasonEvents.Count;
    }

    private void PopulateEventDictionary()
    {
        for (int i = 0; i < eventNames.Count; i++)
        {
            eventDictionary[eventNames[i]] = descriptions[i];
        }
    }

    private void AddToNodeTypeList(List<NodeDescriptions> list, Node node, string eventName)
    {
        eventDictionary[eventName].node = node;
        list.Add(eventDictionary[eventName]);
    }

    private void PopulateMarkovBlanket()
    {
        markovBlanket.Add(eventDictionary["Cloudy"]);
        markovBlanket.Add(eventDictionary["Busy"]);
        markovBlanket.Add(eventDictionary["Dog"]);
        markovBlanket.Add(eventDictionary["Thunder"]);
    }

    public void ResetEventState()
    {
        eventCount = 0;
        evidenceCollected.Clear();
        DrawnNodes.Clear();
        manager.ClearCalculator();
        rawEventDescription = "";
        rawEventEvidence = "";
        rawEventDescription += greetings[GetRandomIndex(greetings)] + "\n";
    }

    public List<NodeDescriptions> DrawRandomEventType(bool canBeSeason)
    {
        int index = (int)Mathf.Round(Random.Range(0, seasonIndex + 0.49f - (canBeSeason ? 0 : 1)));
        if (index == seasonIndex) return seasons;
        return nonSeasonEvents[index];
    }

    public void DrawRandomEvent(List<NodeDescriptions> eventType, bool occurs)
    {
        int index = GetRandomIndex(eventType);
        List<string> eventList = occurs ? eventType[index].eventDescriptions : eventType[index].eventNegationDescriptions;
        int stringIndex = GetRandomIndex(eventList);
        AddEventToRawText(eventType[index].node, eventList[stringIndex], occurs);
    }

    private int GetRandomIndex<T>(List<T> list)
    {
        return (int)Mathf.Round(Random.Range(0, list.Count - 0.51f));
    }

    public void DrawRandomEvents(int numberOfEvents, EventDrawMode mode)
    {
        questionRepo.AskingNewQuestion();
        while(!questionRepo.QuestionIsUnique())
        {
            AttemptUniqueQuestion(numberOfEvents, mode);
            questionRepo.CheckQuestionUniqueness(rawEventEvidence);
        }
        TruncateTrailingComma();
        AddAggressionDescription();
    }

    private void AttemptUniqueQuestion(int numberOfEvents, EventDrawMode mode)
    {
        ResetEventState();
        switch (mode)
        {
            case EventDrawMode.AllEvents:
                DrawFromAllEvents(numberOfEvents);
                break;
            case EventDrawMode.SeasonOnly:
                DrawSeason();
                break;
            case EventDrawMode.SeasonPlusRandom:
                DrawSeasonAndExtra(numberOfEvents);
                break;
            case EventDrawMode.WeatherOnly:
                DrawWeather();
                break;
            case EventDrawMode.MarkovBlanket:
                DrawRandomMarkovBlanket();
                break;
            case EventDrawMode.MarkovBlanketPlusRandom:
                DrawRandomMarkovBlanketAndExtra(numberOfEvents);
                break;
            default:
                Debug.LogWarning("Warning: Default interviewee case encountered");
                DrawFromAllEvents(numberOfEvents);
                break;
        }
    }

    private void DrawFromAllEvents(int numberOfEvents, bool canBeSeason=true)
    {
        while (eventCount < numberOfEvents)
        {
            List<NodeDescriptions> eventType = DrawRandomEventType(canBeSeason);
            bool eventOccurs = Random.value > 0.5f;
            if (eventType == seasons)
            {
                canBeSeason = false;
                eventOccurs = true;
            }
            DrawRandomEvent(eventType, eventOccurs);
        }
    }

    private void DrawSeason()
    {
        DrawRandomEvent(seasons, true);
    }

    private void DrawWeather()
    {
        DrawRandomEvent(weather, Random.value > 0.5f);
    }

    private void DrawSeasonAndExtra(int numberOfExtraEvents)
    {
        DrawSeason();
        DrawFromAllEvents(numberOfExtraEvents, false);
    }

    private void DrawRandomMarkovBlanket()
    {
        foreach(NodeDescriptions nodeDescription in markovBlanket)
        {
            bool eventOccurs = Random.value > 0.5f;
            List<string> relevantList = eventOccurs ? nodeDescription.eventDescriptions : nodeDescription.eventNegationDescriptions;
            AddEventToRawText(nodeDescription.node, relevantList[GetRandomIndex(relevantList)], eventOccurs);
        }
    }

    private void DrawRandomMarkovBlanketAndExtra(int numberOfExtraEvents)
    {
        DrawRandomMarkovBlanket();
        DrawFromAllEvents(numberOfExtraEvents);
    }

    private void TruncateTrailingComma()
    {
        if (rawEventEvidence.Length > 0)
            rawEventEvidence = rawEventEvidence.Substring(0, rawEventEvidence.Length - 1);
    }

    public void AddEventToRawText(Node node, string description, bool eventOccurs)
    {
        if (evidenceCollected.Contains(node.GetName()))
            return;
        DrawnNodes.Add((node,eventOccurs));
        rawEventDescription += description + "\n";
        evidenceCollected.Add(node.GetName());
        rawEventEvidence += (eventOccurs ? "" : "¬") + node.GetAbriviation() + ",";
        eventCount++;
    }

    private void AddAggressionDescription()
    {
        eventAggression = Random.value > friendlyBias;
        List<string> relevantList = eventAggression ? aggressiveDescriptions : friendlyDescriptions;
        rawEventDescription += relevantList[GetRandomIndex(relevantList)];
    }

    public List<NodeDescriptions> GetNodePool(EventCategory category)
    {
        List<NodeDescriptions> pool = category switch
        {
            EventCategory.Season => seasons,
            EventCategory.Weather => weather,
            EventCategory.HumanBehavior => humanActivity,
            EventCategory.AnimalBehavior => animalBehavior,
            EventCategory.EnvironmentalConsequence => consequences,
            _ => null
        };

        return pool;
    }

    public string GetEventDescription() => rawEventDescription;
    public string GetEventEvidence() => rawEventEvidence;
    public bool GetAggression() => eventAggression;
}