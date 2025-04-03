using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

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
	[SerializeField] private List<String> newTextResponse;
	[SerializeField] private List<String> rejectedResponse;
	private InterviewUIManager uiManager;
	private EventDrawer eventDrawer;

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
		string greeting;
	    List<NodeDescriptions> nodePool = eventDrawer.GetNodePool(category);
	    NodeDescriptions alreadyDrawn = nodePool.FirstOrDefault(description =>
	        eventDrawer.DrawnNodes.Any(d => d.node == description.node));

	    if (alreadyDrawn != null)
	    {
	    	greeting = rejectedResponse[UnityEngine.Random.Range(0,rejectedResponse.Count)];
	    	uiManager.DisplayEyewitnessAccount(greeting + "\n it was " + alreadyDrawn.node.name + "!");
	        Debug.Log("I've already seen " + alreadyDrawn.node.name + " before!");
	        return;
	    }

	    if (nodePool.Count == 0)
	    {
	        Debug.LogWarning("No nodes found for category: " + category);
	        return;
	    }
	    NodeDescriptions chosen = nodePool[UnityEngine.Random.Range(0, nodePool.Count)];
	    bool value = category == EventCategory.Season
	        ? true
	        : UnityEngine.Random.value > 0.5f;
	    List<string> relevantList = value ? chosen.eventDescriptions : chosen.eventNegationDescriptions;
		string description = relevantList[UnityEngine.Random.Range(0,relevantList.Count)];
		eventDrawer.AddEventToRawText(chosen.node, description, value);

		greeting = newTextResponse[UnityEngine.Random.Range(0,newTextResponse.Count)];
		uiManager.DisplayEyewitnessAccount(greeting + "\n it was " + chosen.node.name + "!");
        uiManager.DisplayEvidence(eventDrawer.GetEventEvidence());

	    Debug.Log($"[Follow-up] Asking about {chosen.node.name} → {(value ? "Yes" : "No")}");
	}

}