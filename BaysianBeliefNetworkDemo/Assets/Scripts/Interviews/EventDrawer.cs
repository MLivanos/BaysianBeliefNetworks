using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    private List<List<NodeDescriptions>> nonSeasonEvents = new List<List<NodeDescriptions>>();
    private Dictionary<string, NodeDescriptions> eventDictionary;
    private InterviewManager manager;

    // Internal state for drawing events
    private bool eventAggression;
    private int seasonIndex;
    private int eventCount = 0;
    private HashSet<string> evidenceCollected = new HashSet<string>();
    private string rawEventDescription = "";
    private string rawEventEvidence = "";

    /// <summary>
    /// Initializes the event drawer by building the event dictionary and populating
    /// the lists of events. The serialized event data (names and descriptions) come from IM.
    /// </summary>
    public void Initialize(InterviewManager interviewManager)
    {
        manager = interviewManager;
        Graph graph = GameObject.Find("Graph").GetComponent<Graph>();
        Dictionary<string, int> eventIndices = graph.GetComponent<Graph>().AssignIndices();
        // Build dictionary from event names to descriptions.
        eventDictionary = new Dictionary<string, NodeDescriptions>();
        for (int i = 0; i < eventNames.Count; i++)
        {
            eventDictionary[eventNames[i]] = descriptions[i];
        }
        List<Node> nodes = graph.GetAllNodes();
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
        nonSeasonEvents = new List<List<NodeDescriptions>> { weather, consequences, humanActivity, animalBehavior };
        seasonIndex = nonSeasonEvents.Count;
        PopulateEventDictionary();
    }

    private void PopulateEventDictionary()
    {
        eventDictionary = new Dictionary<string, NodeDescriptions>();
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

    /// <summary>
    /// Resets the internal state for drawing events.
    /// </summary>
    public void ResetEventState()
    {
        eventCount = 0;
        evidenceCollected.Clear();
        rawEventDescription = "";
        rawEventEvidence = "";
        rawEventDescription += greetings[GetRandomIndex(greetings)] + "\n";
    }

    /// <summary>
    /// Chooses a random event type list based on whether a season event is allowed.
    /// </summary>
    public List<NodeDescriptions> DrawRandomEventType(bool canBeSeason)
    {
        int index = (int)Mathf.Round(Random.Range(0, seasonIndex + 0.49f - (canBeSeason ? 0 : 1)));
        if (index == seasonIndex)
            return seasons;
        return nonSeasonEvents[index];
    }

    /// <summary>
    /// Draws a random event from the provided event type list.
    /// </summary>
    public (Node, string) DrawRandomEvent(List<NodeDescriptions> eventType, bool occurs)
    {
        int index = GetRandomIndex(eventType);
        List<string> eventList = occurs ? eventType[index].eventDescriptions : eventType[index].eventNegationDescriptions;
        int stringIndex = GetRandomIndex(eventList);
        return (eventType[index].node, eventList[stringIndex]);
    }

    private int GetRandomIndex<T>(List<T> list)
    {
        return (int)Mathf.Round(Random.Range(0, list.Count - 0.51f));
    }

    /// <summary>
    /// Draws the specified number of random events and accumulates their descriptions and evidence.
    /// (Note: InterviewManager will add greetings and additional narrative text.)
    /// </summary>
    public void DrawRandomEvents(int numberOfEvents)
    {
        ResetEventState();
        while (eventCount < numberOfEvents)
        {
            List<NodeDescriptions> eventType = DrawRandomEventType(true);
            bool eventOccurs = (eventType == seasons) ? true : (Random.value > 0.5f);
            (Node node, string description) = DrawRandomEvent(eventType, eventOccurs);
            AddEventToRawText(node, description, eventOccurs);
        }
        if (rawEventEvidence.Length > 0)
            rawEventEvidence = rawEventEvidence.Substring(0, rawEventEvidence.Length - 1); // Remove trailing comma.
        AddAggressionDescription();
    }

    private void AddEventToRawText(Node node, string description, bool eventOccurs)
    {
        if (evidenceCollected.Contains(node.GetName()))
            return;
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

    // Public accessors for InterviewManager to retrieve the drawn event text.
    public string GetEventDescription() => rawEventDescription;
    public string GetEventEvidence() => rawEventEvidence;
    public bool GetAggression() => eventAggression;
}
