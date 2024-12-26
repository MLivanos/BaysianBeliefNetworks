using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndGameManager : MonoBehaviour
{
    [SerializeField] protected List<string> tracks;
    [SerializeField] private TMP_Text text;
    [SerializeField] private List<string> responses;
    [SerializeField] private bool test;
    private EndGameState endGameState;

    private void Start()
    {
        endGameState = EndGameState.instance;
        string response = responses[endGameState.GetPerformanceCode()];
        text.text = response;
        if (test) StartCoroutine(Test());
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
            response = responses[endGameState.GetPerformanceCode()];
            yield return new WaitForSeconds(1f);
        }
    }
}
