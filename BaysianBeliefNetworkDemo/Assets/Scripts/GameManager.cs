using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject difficultySettings;
    [SerializeField] private GameObject warningPanel;
    [SerializeField] private CircularProgressBar timeLimit;
    [SerializeField] private float[] difficultyTimes;
    private static int difficulty = -1;

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
        if (difficulty == -1) difficultySettings.SetActive(true);
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

    public bool CanSample()
    {
        bool canRun = difficulty < 2 || timeLimit.GetMaxValue() - timeLimit.GetProgress() < difficultyTimes[difficulty];
        if (!canRun)
        {
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
        foreach (IFadable fadable in go.GetComponents<IFadable>())
        {
            fadable.SetAlpha(alpha);
        }
        foreach (Transform child in go.transform)
        {
            FadeAllObjects(child.gameObject, alpha);
        }
    }
}