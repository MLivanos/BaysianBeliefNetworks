using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum EventDrawMode
{
    AllEvents,
    SeasonOnly,
    SeasonPlusRandom,
    WeatherOnly,
    MarkovBlanket,
    MarkovBlanketPlusRandom
}

[System.Serializable]
public class QuestionSequenceEntry
{
    public EventDrawMode eventMode;
    public int numberOfEvents;
}

[System.Serializable]
public class QuestionSequence
{
    public List<QuestionSequenceEntry> questionSequence;
}

public class InterviewManager : MonoBehaviour
{
    [SerializeField] private IntervieweeSpawner intervieweeSpawner;
    [SerializeField] private TimestepManager timestepManager;
    [SerializeField] private List<QuestionSequence> questionsByDifficulty;
    private InterviewCalculator calculator;
    private Recorder recorder;
    private EventDrawer eventDrawer;
    private Graph graph;
    private InterviewUIManager uiManager;
    private List<QuestionSequenceEntry> questionSequence;
    private bool lastEventAggression;
    private bool lastEventBelieved;
    private int stage=-1;
    private int numberOfStages = 5;
    private int questionsRemaining;
    private int adaptiveDifficultyBonus = 0;

    private void Start()
    {
        questionSequence = questionsByDifficulty[PlayerPrefs.GetInt("Difficulty", 1)].questionSequence;
        questionsRemaining = questionSequence.Count;
        GetComponents();
        StartCoroutine(InstantiateManager());
        Advance();
    }

    public void Advance()
    {
        stage++;
        stage %= numberOfStages;
        switch (stage)
        {
            case 0:
                intervieweeSpawner.SpawnInterviewee();
                break;
            case 1:
                SetAdaptiveDifficulty(recorder.PlayerAccuracy());
                Debug.Log(recorder.PlayerAccuracy());
                Debug.Log(adaptiveDifficultyBonus);
                QuestionSequenceEntry question = questionSequence[questionSequence.Count - questionsRemaining];
                int numberOfEvents = (int)Mathf.Max(question.numberOfEvents + adaptiveDifficultyBonus, 1f);
                eventDrawer.DrawRandomEvents(numberOfEvents, question.eventMode);
                uiManager.DisplayEyewitnessAccount(eventDrawer.GetEventDescription());
                uiManager.DisplayEvidence(eventDrawer.GetEventEvidence());
                break;
            case 2:
                intervieweeSpawner.DespawnInterviewee();
                break;
            case 3:
                timestepManager.Step();
                break;
            case 4:
                AddNodesToCalculator();
                float eventProbability = calculator.CalculateProbability(0.99f, 20, 10, 2.576f);
                Debug.Log(eventProbability);
                recorder.AddEntry(eventDrawer.GetEventEvidence(), eventProbability, lastEventBelieved, eventDrawer.GetAggression());
                if (--questionsRemaining == 0) EndInterviews();
                break;
            default:
                break;
        }
    }

    private void GetComponents()
    {
        graph = GameObject.Find("Graph").GetComponent<Graph>();
        calculator = GetComponent<InterviewCalculator>();
        recorder = GetComponent<Recorder>();
        uiManager = GetComponent<InterviewUIManager>();
        eventDrawer = GetComponent<EventDrawer>();
        intervieweeSpawner.Initialize(this);
        recorder.Initialize(this);
        timestepManager.Initialize(this);
        uiManager.Initialize(this);
        eventDrawer.Initialize(this);
    }

    private IEnumerator InstantiateManager()
    {
        yield return null;
        Dictionary<string, int> eventIndices = graph.AssignIndices();
        calculator.Initialize(graph.GetRootNodes(), eventIndices, graph.gameObject.GetComponent<LikelihoodWeightingSampler>());
        float alienProbability = GetAlienProbability();
        Debug.Log(alienProbability);
        recorder.LogAlienProbability(alienProbability);
    }

    private float GetAlienProbability()
    {
        calculator.Reset();
        return calculator.CalculateProbability(0.995f, 50, 10, 2.576f);
    }

    public void SetBelief(bool belief)
    {
        lastEventBelieved = belief;
    }

    private void EndInterviews()
    {
        recorder.DetermineBehavior(0.33f);
        recorder.StoreEndGameState();
    }

    public void ClearCalculator()
    {
        calculator.Reset();
    }

    private void AddNodesToCalculator()
    {
        foreach(var (node, eventOccurs) in eventDrawer.DrawnNodes)
        {
            calculator.AddToEvidence(node, eventOccurs);
        }
    }

    private void SetAdaptiveDifficulty(float playerAccuracy)
    {
        if (questionSequence.Count - questionsRemaining >= 4)
        {
            if (playerAccuracy > 0.99f) adaptiveDifficultyBonus = 2;
            else if (playerAccuracy >= 0.8f) adaptiveDifficultyBonus = 1;
            else if (playerAccuracy < 0.4f) adaptiveDifficultyBonus = -1;
            else if (playerAccuracy < 0.2f) adaptiveDifficultyBonus = -2;
            else adaptiveDifficultyBonus = 0;
        }
        else adaptiveDifficultyBonus = 0;
    }
}