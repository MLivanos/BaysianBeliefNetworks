using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InterviewManager : MonoBehaviour
{
    [SerializeField] private IntervieweeSpawner intervieweeSpawner;
    [SerializeField] private TimestepManager timestepManager;
    private InterviewCalculator calculator;
    private Recorder recorder;
    private EventDrawer eventDrawer;
    private Graph graph;
    private InterviewUIManager uiManager;
    private bool lastEventAggression;
    private bool lastEventBelieved;
    private HashSet<string> evidenceCollected = new HashSet<string>();
    private int stage=-1;
    private int numberOfStages = 5;
    private int questionsRemaining = 15;

    private void Start()
    {
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
                eventDrawer.DrawRandomEvents(2);
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
                float eventProbability = calculator.CalculateProbability(0.98f, 15, 3);
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
        recorder.LogAlienProbability(GetAlienProbability());
    }

    
    private void ResetEventState()
    {
        calculator.Reset();
        eventDrawer.ResetEventState();
        evidenceCollected.Clear();
    }

    private float GetAlienProbability()
    {
        ResetEventState();
        return calculator.CalculateProbability(0.995f, 50, 5, 2.576f);
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
}