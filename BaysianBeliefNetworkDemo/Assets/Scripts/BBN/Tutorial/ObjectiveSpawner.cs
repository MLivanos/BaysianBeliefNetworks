using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveSpawner : MonoBehaviour
{
    [SerializeField] private GameObject objectivePrefab;
    [SerializeField] private Transform objectivePanel;
    [SerializeField] private Vector3 topPosition;
    [SerializeField] private float offset;
    private List<ObjectiveMark> marks = new List<ObjectiveMark>();
    private int numberOfObjectives;

    private void Start()
    {
        AddObjective("Objective 1");
        AddObjective("Objective 2");
        AddObjective("Objective 3");
    }

    private void AddObjective(string description)
    {
        ObjectiveMark newObjective = Instantiate(objectivePrefab.GetComponent<ObjectiveMark>(), objectivePanel);
        newObjective.SetText(description);

        Vector2 anchoredPos = new Vector2(topPosition.x, topPosition.y - offset * numberOfObjectives++);
        RectTransform rectTransform = newObjective.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPos;

        marks.Add(newObjective);
    }

}
