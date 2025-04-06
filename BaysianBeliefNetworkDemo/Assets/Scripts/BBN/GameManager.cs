using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour, ISceneDetectorTarget
{
    public string HomeSceneName { get; } = "BBN";
    [SerializeField] private GameObject difficultySettings;
    [SerializeField] private GameObject warningPanel;
    [SerializeField] private Button toInterviewButton;
    [SerializeField] private GameObject interactionBlocker;
    [SerializeField] private CircularProgressBar timeLimit;
    [SerializeField] private float[] difficultyTimes;
    private EntropyGorilla gorilla;
    private AudioManager audioManager;
    private Playlist playlist;
    private static int difficulty = -1;
    public static GameManager instance;
    private static float timeProgress;

    public float TimeProgress() => timeProgress;
    public void SetTimeProgress(float progress){
        difficulty = PlayerPrefs.GetInt("Difficulty", 0);
        timeProgress = progress;
        timeLimit.SetMaxValue(difficultyTimes[difficulty]);
        timeLimit.UpdateProgress(timeProgress);
    } 

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

    private void Start()
    {
        gorilla = FindObjectOfType<EntropyGorilla>();
        audioManager = FindObjectOfType<AudioManager>();
        if (difficulty > 0)
        {
            timeLimit.UpdateProgress(timeProgress);
        }
        if (playlist == null)
        {
            playlist = GetComponent<Playlist>();
            playlist.Play();
        }
    }

    public void UpdateTimer(float decrement)
    {
        if (timeLimit.gameObject.activeSelf)
        {
            timeLimit.IncrementProgress(decrement);
            timeProgress = timeLimit.GetProgress();
        }
    }

    public void PromptGameMode()
    {
        if (difficulty == -1)
        {
            interactionBlocker.SetActive(true);
            difficultySettings.SetActive(true);
            toInterviewButton.interactable = false;
        }
        if (gorilla == null)
        {
            gorilla = FindObjectOfType<EntropyGorilla>();
        }
        if (difficulty == 2)
        {
            gorilla.PokeGorilla();
        }
    }

    public void ChangeGamemode(int gamemodeNumber)
    {
        PlayerPrefs.SetInt("Difficulty", gamemodeNumber);
        PlayerPrefs.Save();
        difficultySettings.SetActive(false);
        interactionBlocker.SetActive(false);
        toInterviewButton.interactable = true;
        difficulty = gamemodeNumber;
        timeLimit.SetMaxValue(difficultyTimes[gamemodeNumber]);
        timeLimit.ResetProgress();
    }

    public bool CanSample()
    {
        bool canRun = difficulty == 0 || timeLimit.GetMaxValue() - timeLimit.GetProgress() < difficultyTimes[difficulty];
        if (!canRun)
        {
            audioManager.PlayEffect("OutOfCompute");
            WarnPlayer("CRITICAL ERROR: Resource budget exceeded. Further operations are suspended");
        }
        return canRun;
    }

    private void WarnPlayer(string warning)
    {
        warningPanel.SetActive(true);
        TMP_Text warningText = warningPanel.GetComponentInChildren<TMP_Text>();
        warningText.text = warning;
        StartCoroutine(FadeWarning(5f,1f));
    }

    private IEnumerator FadeWarning(float timeBefore, float fadeTime)
    {
        yield return Fade(warningPanel, 0, 1, 1f);
        yield return new WaitForSeconds(timeBefore);
        yield return Fade(warningPanel, 1, 0, fadeTime);
        warningPanel.SetActive(false);
    }

    private IEnumerator Fade(GameObject target, float startAlpha, float endAlpha, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            float t = timer / duration;
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, t);
            FadeAllObjects(target, currentAlpha);
            timer += Time.deltaTime;
            yield return null;
        }
        FadeAllObjects(target, endAlpha);
    }

    public static void FadeAllObjects(GameObject go, float alpha)
    {
        foreach (FadableElement fadable in go.GetComponents<FadableElement>())
        {
            fadable.SetAlpha(alpha);
        }
        foreach (Transform child in go.transform)
        {
            FadeAllObjects(child.gameObject, alpha);
        }
    }

    public void OnSceneChange(bool isHomeScene)
    {
        if (timeLimit == null) timeLimit = FindFirstObjectByType<CircularProgressBar>();
        HandleAudioSceneChange(isHomeScene);
        if (isHomeScene && difficulty > -1) ResetTimer();
    }

    public void HandleAudioSceneChange(bool isHomeScene)
    {
        if (!playlist) return;
        if (isHomeScene) playlist.Resume();
        else playlist.Pause();
    }

    public void ResetTimer()
    {
        timeLimit.SetMaxValue(difficultyTimes[difficulty]);
        timeLimit.UpdateProgress(timeProgress);
    }
}