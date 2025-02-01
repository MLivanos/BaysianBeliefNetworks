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
        ProbabilityQuestion,
        Negate,
        NegateProbabilityQuestion,
        AnyProbabilityQuestion,
        AnyProbabilityQuestionWithQuery,
        AnyProbabilityQuestionWithEvidence
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
        if (IsProbabilityTypeQuestion() && IsValidProbability(targetText))
        {
            (targetQuery, targetEvidence) = ParseProbabilityQuestion(targetText);
        }
        else if (IsProbabilityTypeQuestion())
        {
            Debug.LogError("Target text improperly formatted for probability question!");
            return;
        }
        StartCoroutine(ListenForTextChange());
    }

    private bool IsProbabilityTypeQuestion()
    {
        return mode != Mode.Normal && mode != Mode.Negate && mode != Mode.AnyProbabilityQuestion;
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
            case Mode.AnyProbabilityQuestion:
                return IsValidProbability(textField.text);
            case Mode.ProbabilityQuestion:
                return IsValidProbability(textField.text) && QuestionsEqual();
            case Mode.NegateProbabilityQuestion:
                return IsValidProbability(textField.text) && !QuestionsEqual();
            case Mode.AnyProbabilityQuestionWithQuery:
                return IsValidProbability(textField.text) && QuestionContains(true, false);
            case Mode.AnyProbabilityQuestionWithEvidence:
                return IsValidProbability(textField.text) && QuestionContains(false, true);
            case Mode.Negate:
                return textField.text != targetText;
            default:
                return textField.text == targetText;
        }
    }


    private bool QuestionsEqual(bool queryRelevant=true, bool evidenceRelevant=true)
    {
        (HashSet<string> query, HashSet<string> evidence) = ParseProbabilityQuestion(textField.text);
        bool queryEqual = query.SetEquals(targetQuery);
        bool evidenceEqual = evidence.SetEquals(targetEvidence);
        return (queryEqual || !queryRelevant) && (evidenceEqual || !evidenceRelevant);
    }

    private bool QuestionContains(bool queryRelevant=true, bool evidenceRelevant=true)
    {
        (HashSet<string> query, HashSet<string> evidence) = ParseProbabilityQuestion(textField.text);
        bool queryContains = targetQuery.All(element => query.Contains(element));
        bool evidenceContains = targetEvidence.All(element => evidence.Contains(element));
        return (queryContains || !queryRelevant) && (evidenceContains || !evidenceRelevant);
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