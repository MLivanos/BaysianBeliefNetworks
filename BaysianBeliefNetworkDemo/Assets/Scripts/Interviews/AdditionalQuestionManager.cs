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
	private EventDrawer eventDrawer;

	private void Start()
	{
		eventDrawer = GetComponent<EventDrawer>();
	}

	public void AskSeasonQuestion() => AskQuestion(EventCategory.Season);
	public void AskWeatherQuestion() => AskQuestion(EventCategory.Weather);
	public void AskHumanQuestion() => AskQuestion(EventCategory.HumanBehavior);
	public void AskAnimalQuestion() => AskQuestion(EventCategory.AnimalBehavior);
	public void AskConsequenceQuestion() => AskQuestion(EventCategory.EnvironmentalConsequence);

	private void AskQuestion(EventCategory category)
	{
	    List<NodeDescriptions> nodePool = eventDrawer.GetNodePool(category);
	    NodeDescriptions alreadyDrawn = nodePool.FirstOrDefault(description =>
	        eventDrawer.DrawnNodes.Any(d => d.node == description.node));

	    if (alreadyDrawn != null)
	    {
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
		//eventDrawer.DrawnNodes.Add((chosen.node, value));

	    Debug.Log($"[Follow-up] Asking about {chosen.node.name} → {(value ? "Yes" : "No")}");
	}

}