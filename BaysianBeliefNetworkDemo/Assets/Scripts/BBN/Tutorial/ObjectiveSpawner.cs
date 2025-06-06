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

    public void AddObjective(string description)
    {
        ObjectiveMark newObjective = Instantiate(objectivePrefab.GetComponent<ObjectiveMark>(), objectivePanel);
        newObjective.SetText(description);

        Vector2 anchoredPos = new Vector2(topPosition.x, topPosition.y - offset * numberOfObjectives++);
        RectTransform rectTransform = newObjective.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPos;

        marks.Add(newObjective);
    }

    public void ClearObjectives()
    {
        foreach(ObjectiveMark mark in marks)
        {
            Destroy(mark.gameObject);
        }
        numberOfObjectives = 0;
        marks = new List<ObjectiveMark>();
    }

    public void CompleteQuest(int index)
    {
        marks[index].Check();
    }

}
