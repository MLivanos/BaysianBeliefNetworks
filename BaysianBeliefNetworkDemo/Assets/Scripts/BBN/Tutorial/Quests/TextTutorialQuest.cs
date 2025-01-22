using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextTutorialQuest : TutorialQuestBase
{
    public enum Mode
    {
        Normal,
        ProbabilityQuestion
    }

    [SerializeField] private Mode mode;
    [SerializeField] private string targetText;

    private TMP_Text textField;
    private HashSet<string> targetQuery;
    private HashSet<string> targetEvidence;

    public override void OnInitialize()
    {
        textField = objectToAttach.GetComponent<TMP_Text>();
        if (textField == null)
        {
            Debug.LogError("No Text attached to the object!");
            return;
        }
        if (mode == Mode.ProbabilityQuestion && IsValidProbability(targetText))
        {
            (targetQuery, targetEvidence) = ParseProbabilityQuestion(targetText);
        }
        else if (mode == Mode.ProbabilityQuestion)
        {
            Debug.LogError("No Text attached to the object!");
            return;
        }
        StartCoroutine(ListenForTextChange());
    }

    public IEnumerator ListenForTextChange()
    {
        while(!TextIsTarget())
        {
            yield return new WaitForSeconds(0.1f);
        }
        Complete();
    }

    private bool TextIsTarget()
    {
        switch (mode)
        {
            case Mode.ProbabilityQuestion:
                return IsValidProbability(textField.text) && QuestionsEqual();
            default:
                return textField.text == targetText;
        }
    }


    private bool QuestionsEqual()
    {
        (HashSet<string> query, HashSet<string> evidence) = ParseProbabilityQuestion(textField.text);
        return query.SetEquals(targetQuery) && evidence.SetEquals(targetEvidence);
    }

    private (HashSet<string> query, HashSet<string> evidence) ParseProbabilityQuestion(string text)
    {
        string content = text.Substring(2, text.Length - 3);
        string[] parts = content.Split('|');
        HashSet<string> query = new HashSet<string>(parts[0].Split(',').Select(s => s.Trim()));
        HashSet<string> evidence;

        if (parts.Length == 1) evidence = new HashSet<string>();
        else evidence = new HashSet<string>(parts[1].Split(',').Select(s => s.Trim()));

        return (query, evidence);
    }

    private bool IsValidProbability(string s)
    {
        return (s.StartsWith("P(") && s.EndsWith(")"));
    }

    public override void HandleInteraction()
    {
        Debug.LogWarning("This overload should not be called directly!");
    }
}