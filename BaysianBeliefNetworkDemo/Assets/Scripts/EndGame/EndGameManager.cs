using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndGameManager : MonoBehaviour
{
    [SerializeField] private TypewriterEffect typewriterEffect;
    [SerializeField] private GameObject textPanel;
    [SerializeField] private Transform mainCamera;
    [SerializeField] protected List<EndGameSlide> endSlides;
    [SerializeField] protected List<string> tracks;
    [SerializeField] private TMP_Text text;
    [SerializeField] private List<string> responses;
    [SerializeField] private bool test;
    private bool isExiting = false;
    private Coroutine currentCoroutine;
    private int currentSceneId = 0;
    private List<int> sceneCodes = new List<int>();
    private EndGameState endGameState;

    private void Start()
    {
        endGameState = EndGameState.instance;
        GetSceneCodes();
        if (test) StartCoroutine(Test());
        else Advance();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isExiting) StartCoroutine(Exit());
    }

    private void Advance()
    {
        if (currentSceneId >= endSlides.Count) return;
        currentCoroutine = StartCoroutine(endSlides[currentSceneId].Run(sceneCodes[currentSceneId], mainCamera, textPanel, typewriterEffect));
        currentSceneId++;
    }

    private IEnumerator Exit()
    {
        isExiting = true;
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
        yield return endSlides[currentSceneId-1].Exit();
        isExiting = false;
        Advance();
    }

    private void GetSceneCodes()
    {
        sceneCodes.Add(0);
        sceneCodes.Add(endGameState.GetPredictionCode());
        sceneCodes.Add(endGameState.GetRealityCode());
        sceneCodes.Add(endGameState.GetConsequenceCode());
        sceneCodes.Add(endGameState.GetScore());
    }

    private IEnumerator Test()
    {
        string response;
        endGameState.aliensAreReal = true;
        endGameState.aliensAreAggressive = true;
        endGameState.predictedReal = true;
        endGameState.predictedAggressive = true;
        for(int i=0; i<responses.Count; i++)
        {
            if (i%1 == 0) endGameState.aliensAreReal = !endGameState.aliensAreReal;
            if (i%2 == 0) endGameState.aliensAreAggressive = !endGameState.aliensAreAggressive;
            if (i%4 == 0) endGameState.predictedReal = !endGameState.predictedReal;
            if (i%8 == 0) endGameState.predictedAggressive = !endGameState.predictedAggressive;
            response = responses[endGameState.GetScore()];
            yield return new WaitForSeconds(1f);
        }
    }
}
