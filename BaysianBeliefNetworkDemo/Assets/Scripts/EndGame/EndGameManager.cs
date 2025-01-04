using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    private AudioManager audioManager;
    private bool isExiting = false;
    private Coroutine currentCoroutine;
    private int currentSceneId = 0;
    private List<int> sceneCodes = new List<int>();
    private EndGameState endGameState;
    private bool done;

    private void Start()
    {
        done = false;
        audioManager = AudioManager.instance;
        endGameState = EndGameState.instance;
        GetSceneCodes();
        PlayMusic();
        if (test) StartCoroutine(Test());
        else Advance();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isExiting) HandleClick();
    }

    private void HandleClick()
    {
        if (typewriterEffect.IsTyping()) typewriterEffect.Interrupt();
        else StartCoroutine(Exit());
    }

    private void Advance()
    {
        if (currentSceneId >= endSlides.Count)
        {
            audioManager.PauseMusic();
            done = true;
            return;
        }
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

    private void PlayMusic()
    {
        audioManager.PlayMusic(tracks[endGameState.GetScore()]);
    }

    private IEnumerator Test()
    {
        int i = (endGameState.aliensAreReal ? 8 : 0) + (endGameState.aliensAreAggressive ? 4 : 0) +
            (endGameState.predictedReal ? 2 : 0) + (endGameState.predictedAggressive ? 1 : 0) + 1;
        if (i > 1) currentSceneId = 1;
        Advance();
        while(!done) yield return null;
        AdvanceTestScene(i);
    }

    private void ReloadScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    private void AdvanceTestScene(int i)
    {
        if (i == 16) return;
        Debug.Log(i);
        if (i%1 == 0) endGameState.predictedAggressive = !endGameState.predictedAggressive;
        if (i%2 == 0) endGameState.predictedReal = !endGameState.predictedReal;
        if (i%4 == 0) endGameState.aliensAreAggressive = !endGameState.aliensAreAggressive;
        if (i%8 == 0) endGameState.aliensAreReal = !endGameState.aliensAreReal;
        ReloadScene();
    }
}
