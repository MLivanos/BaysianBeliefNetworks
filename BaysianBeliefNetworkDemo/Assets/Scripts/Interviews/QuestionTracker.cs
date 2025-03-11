using System;
using System.Collections.Generic;
using System.Linq;

public class QuestionTracker
{
    private HashSet<string> askedQuestions = new HashSet<string>();
    public int maxNumberOfAttempts;
    private int numberOfAttempts;
    public bool uniqueQuestionFound;

    public QuestionTracker(int maxAttempts)
    {
        maxNumberOfAttempts = maxAttempts;
        numberOfAttempts = 0;
        uniqueQuestionFound = false;
    }

    /// <summary>
    /// Normalizes and adds a new question to the asked questions set.
    /// Sets uniqueQuestionFound if question is new
    /// </summary>
    public void CheckQuestionUniqueness(string question)
    {
        string normalizedQuestion = NormalizeQuestion(question);
        if (askedQuestions.Contains(normalizedQuestion) && numberOfAttempts < maxNumberOfAttempts)
        {
            numberOfAttempts++;
            return;
        }
        askedQuestions.Add(normalizedQuestion);
        numberOfAttempts = 0;
        uniqueQuestionFound = true;
    }

    /// <summary>
    /// Normalizes a question by sorting the event components alphabetically.
    /// </summary>
    private string NormalizeQuestion(string question)
    {
        string[] events = question.Split(',');
        Array.Sort(events, StringComparer.OrdinalIgnoreCase); // Sort alphabetically
        return string.Join(",", events).Trim(); // Reconstruct sorted string
    }

    /// <summary>
    /// Clears the asked questions when resetting.
    /// </summary>
    public void Reset()
    {
        askedQuestions.Clear();
    }

    public void AskingNewQuestion()
    {
        uniqueQuestionFound = false;
    }

    public bool QuestionIsUnique() => uniqueQuestionFound;
}
