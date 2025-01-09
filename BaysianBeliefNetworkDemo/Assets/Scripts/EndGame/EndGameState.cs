using System.Collections.Generic;
using UnityEngine;

public class EndGameState : MonoBehaviour
{
    public bool aliensAreReal;
    public bool aliensAreAggressive;
    public bool predictedReal;
    public bool predictedAggressive;
    public static EndGameState instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /* Key: 4-bit number with digits:
    aliens real, aliens aggressive, predicted real, predicted aggressive */
    public int GetConsequenceCode()
    {
        return BoolsToInt(new List<bool> { predictedAggressive, predictedReal, aliensAreAggressive, aliensAreReal });
    }

    /* Key: 2-bit number with digits:
    predicted aggressive, predicted real*/
    public int GetPredictionCode()
    {
        return BoolsToInt(new List<bool> { predictedReal, predictedAggressive });
    }

    /* Key: 2-bit number with digits:
    aliens real, aliens aggressive */
    public int GetRealityCode()
    {
        return BoolsToInt(new List<bool> { aliensAreAggressive, aliensAreReal });
    }

    /* Key:
        0 - bad ending (none correct)
        1 - mid ending (one correct)
        2 - good ending (both correct)*/
    public int GetScore()
    {
        return (aliensAreReal == predictedReal ? 1 : 0) + (aliensAreAggressive == predictedAggressive ? 1 : 0);
    }

    private int BoolsToInt(List<bool> bools)
    {
        int value = 0;
        for (int i = 0; i < bools.Count; i++)
        {
            if (bools[i]) value += 1 << i;
        }
        return value;
    }
}
