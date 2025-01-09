using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecorderEntry
{
    public string evidence;
    public float probability;
    public bool credibility;
    public bool playerResponse;
    public bool aggressive;

    public RecorderEntry(string evidenceDescribed, float p, bool credible, bool response, bool describedAgressive)
    {
        evidence = evidenceDescribed;
        probability = p;
        credibility = credible;
        playerResponse = response;
        aggressive = describedAgressive;
    }
}

public class Recorder : InterviewEventSystem
{
    private List<RecorderEntry> entries = new List<RecorderEntry>();
    private float alienProbability;
    private bool aliensReal;
    private bool aliensAggressive;
    private bool playerBelieves;
    private bool playerBelievesAggressive;
    private bool complete;

    public void LogAlienProbability(float probability)
    {
        alienProbability = probability;
    }

    public void AddEntry(string evidence, float probability, bool response, bool aggressivity)
    {
        RecorderEntry entry = new RecorderEntry(evidence, probability, probability >= alienProbability, response, aggressivity);
        entries.Add(entry);
        interviewManager.Advance();
    }

    public void DetermineBehavior(float realityThreshold)
    {
        int numberAliensReal = 0;
        int realAggressiveAccounts = 0;
        int numberBelieved = 0;
        int aggressiveAccountsAccepted = 0;

        foreach (RecorderEntry entry in entries)
        {
            if (entry.credibility) numberAliensReal++;
            if (entry.credibility && entry.aggressive) realAggressiveAccounts++;
            if (entry.playerResponse) numberBelieved++;
            if (entry.playerResponse && entry.aggressive) aggressiveAccountsAccepted++;
        }

        aliensReal = (float)numberAliensReal / entries.Count >= realityThreshold;
        aliensAggressive = numberAliensReal > 0 && (float)realAggressiveAccounts / numberAliensReal >= 0.5f;
        playerBelieves = (float)numberBelieved / entries.Count >= realityThreshold;
        playerBelievesAggressive = numberBelieved > 0 && (float)aggressiveAccountsAccepted / numberBelieved >= 0.5f;

        complete = true;
    }

    private void EnsureComplete()
    {
        if (!complete) throw new InvalidOperationException("Behavior determination not yet completed.");
    }

    public bool AliensAreReal()
    {
        EnsureComplete();
        return aliensReal;
    }

    public bool AliensAreAggressive()
    {
        EnsureComplete();
        return aliensAggressive;
    }

    public bool PlayerBelieves()
    {
        EnsureComplete();
        return playerBelieves;
    }

    public bool PlayerBelievesAggressive()
    {
        EnsureComplete();
        return playerBelievesAggressive;
    }

    public void StoreEndGameState()
    {
        if (!complete)
        {
            Debug.LogWarning("End Game states have not been calculated. Aborting");
            return;
        }
        GameObject endGameState = new GameObject("EndGameState");
        EndGameState endState = endGameState.AddComponent(typeof(EndGameState)) as EndGameState;
        endState.aliensAreReal = aliensReal;
        endState.aliensAreAggressive = aliensAggressive;
        endState.predictedReal = playerBelieves;
        endState.predictedAggressive = playerBelievesAggressive;
    }
}