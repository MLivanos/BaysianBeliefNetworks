using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EventCategory
{
    Season,
    Weather,
    HumanBehavior,
    AnimalBehavior,
    EnvironmentalConsequence
}

public class AdditionalQuestionManager : MonoBehaviour
{
    [SerializeField] private List<string> newTextResponse;
    [SerializeField] private List<string> rejectedResponse;

    private InterviewUIManager uiManager;
    private EventDrawer eventDrawer;

    private Dictionary<string, string> nodeDescriptions = new Dictionary<string, string>()
	{
		{ "WinterNode", "winter" },
		{ "SpringNode", "spring" },
		{ "SummerNode", "summer" },
		{ "FallNode", "fall" },
	    { "RainNode", "raining" },
	    { "CloudNode", "cloudy" },
	    { "HighWindNode", "windy" },
	    { "APDNode", "feeling heavy in the air"},
	    { "PowerOutageNode", "in the middle of a power outage" },
	    { "BusyNode", "a busy day" },
	    { "CafeNode", "business as usual at the cafe" },
	    { "DogNode", "strange to hear the neighborhood dogs barking" },
	    { "CatNode", "interesting to see a cat running away" },
	    { "TreeNode", "shocking to see a fallen tree" },
	    { "ThunderNode", "thundering" },
	};

	private Dictionary<string, string> nodeNegations = new Dictionary<string, string>()
	{
	    { "RainNode", "dry out" },
	    { "CloudNode", "clear skies" },
	    { "HighWindNode", "calm air" },
	    { "APDNode", "stable pressure" },
	    { "PowerOutageNode", "business as usual at the power plant" },
	    { "BusyNode", "a slow day" },
	    { "CafeNode", "sad to see the cafe was closed" },
	    { "DogNode", "a quiet time with the dog fast asleep" },
	    { "CatNode", "nice seeing the neighborhood cats relaxing" },
	    { "TreeNode", "peaceful among the trees" },
	    { "ThunderNode", "quiet in the skies" }
	};


    private void Start()
    {
        eventDrawer = GetComponent<EventDrawer>();
        uiManager = GetComponent<InterviewUIManager>();
    }

    public void AskSeasonQuestion() => AskQuestion(EventCategory.Season);
    public void AskWeatherQuestion() => AskQuestion(EventCategory.Weather);
    public void AskHumanQuestion() => AskQuestion(EventCategory.HumanBehavior);
    public void AskAnimalQuestion() => AskQuestion(EventCategory.AnimalBehavior);
    public void AskConsequenceQuestion() => AskQuestion(EventCategory.EnvironmentalConsequence);

    private void AskQuestion(EventCategory category)
	{
	    if (HasCategoryBeenDrawn(category, out var existingNode, out bool occurs))
	    {
	        PresentAlreadyAsked(existingNode, occurs);
	        return;
	    }

	    if (!TryPickRandomNode(category, out var chosen))
	    {
	        Debug.LogWarning($"No nodes found for category: {category}");
	        return;
	    }

	    bool value = GenerateBoolValue(category);
	    string description = GetDescriptionForNode(chosen, value);

	    eventDrawer.AddEventToRawText(chosen.node, description, value);
	    PresentFollowupResult(chosen.node, value);
	}

	private bool HasCategoryBeenDrawn(EventCategory category, out Node alreadyDrawnNode, out bool occurs)
	{
	    alreadyDrawnNode = null;
	    occurs = false;

	    var pool = eventDrawer.GetNodePool(category);

	    foreach (var description in pool)
	    {
	        var match = eventDrawer.DrawnNodes.FirstOrDefault(x => x.node == description.node);
	        if (match.node != null)
	        {
	            alreadyDrawnNode = match.node;
	            occurs = match.occurs;
	            return true;
	        }
	    }

	    return false;
	}


	private bool TryPickRandomNode(EventCategory category, out NodeDescriptions chosen)
	{
	    var pool = eventDrawer.GetNodePool(category);
	    if (pool.Count == 0)
	    {
	        chosen = null;
	        return false;
	    }

	    chosen = pool[UnityEngine.Random.Range(0, pool.Count)];
	    return true;
	}

	private bool GenerateBoolValue(EventCategory category)
	{
	    return category == EventCategory.Season || UnityEngine.Random.value > 0.5f;
	}

	private string GetDescriptionForNode(NodeDescriptions node, bool occurs)
	{
	    var list = occurs ? node.eventDescriptions : node.eventNegationDescriptions;
	    return GetRandomLine(list);
	}

	private void PresentAlreadyAsked(Node node, bool occurs)
	{
	    var rejection = GetRandomLine(rejectedResponse);
	    Dictionary<string,string> relevantDictionary = occurs ? nodeDescriptions : nodeNegations;
	    uiManager.DisplayEyewitnessAccount($"{rejection}\n it was {relevantDictionary[node.name]}!");
	    Debug.Log($"Already saw {node.name}");
	}

	private void PresentFollowupResult(Node node, bool occurs)
	{
	    var greeting = GetRandomLine(newTextResponse);
	    Dictionary<string,string> relevantDictionary = occurs ? nodeDescriptions : nodeNegations;
	    uiManager.DisplayEyewitnessAccount($"{greeting}\n it was {relevantDictionary[node.name]}!");
	    uiManager.DisplayEvidence(eventDrawer.GetEventEvidence(), false);
	    Debug.Log($"[Follow-up] Asked about {node.name} → {(occurs ? "Yes" : "No")}");
	}

	private string GetRandomLine(List<string> source)
	{
	    return source[UnityEngine.Random.Range(0, source.Count)];
	}
}