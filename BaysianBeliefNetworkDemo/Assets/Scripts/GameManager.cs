using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject difficultySettings;
    [SerializeField] private CircularProgressBar timeLimit;
    [SerializeField] private bool hasSelectedDifficulty;
    [SerializeField] private float[] difficultyTimes;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void UpdateTimer(float decrement)
    {
        timeLimit.IncrementProgress(decrement);
    }

    public void PromptGameMode()
    {
        
    }
}