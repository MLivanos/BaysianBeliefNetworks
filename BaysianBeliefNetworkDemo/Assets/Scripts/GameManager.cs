using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject difficultySettings;
    [SerializeField] private CircularProgressBar timeLimit;
    [SerializeField] private float[] difficultyTimes;
    private int difficulty;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void UpdateTimer(float decrement)
    {
        if (timeLimit.gameObject.activeSelf) timeLimit.IncrementProgress(decrement);
    }

    public void PromptGameMode()
    {
        difficultySettings.SetActive(true);
    }

    public void ChangeGamemode(int gamemodeNumber)
    {
        difficultySettings.SetActive(false);
        difficulty = gamemodeNumber;
        if (gamemodeNumber < 2)
        {
            timeLimit.SetMaxValue(float.MaxValue);
            timeLimit.gameObject.SetActive(false);
            return;
        }
        timeLimit.SetMaxValue(difficultyTimes[gamemodeNumber]);
    }
}