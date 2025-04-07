using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private IntroCutscene[] cutscenes;
    [Header("Universal Objects")]
    [SerializeField] private TypewriterEffect typewriterEffect;
    [SerializeField] private GameObject textPanel;
    [SerializeField] private Transform mainCamera;
    [Header("Character Portrait")]
    [SerializeField] private GameObject characterImage;
    [SerializeField] private GameObject characterName;
    [Header("Debug Tools")]
    [SerializeField] private bool debugMode;
    [SerializeField] private TextMeshProUGUI fpsCounterText;
    private AudioManager audioManager;
    private SceneManagerScript sceneManager;
    private CutsceneBehavior currentCutScene;
    private Coroutine currentCoroutine;
    private int cutsceneIndex = 0;
    private bool exiting;

    private void Start()
    {
        audioManager = AudioManager.instance;
        characterImage.SetActive(false);
        characterName.SetActive(false);
        if (debugMode) GetComponent<FPSTracker>().StartTracking();
        else fpsCounterText.gameObject.SetActive(false);
        sceneManager = GetComponent<SceneManagerScript>();
        textPanel.SetActive(false);
        currentCoroutine = StartCoroutine(PlayNextScene());
        audioManager.FadeInMusicAndAmbient(cutscenes[0].GetMusic(), cutscenes[0].GetFadeSoundTime());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            sceneManager.StartGame();
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (exiting) return;
            else if (typewriterEffect.IsTyping()) typewriterEffect.Interrupt();
            else EndScene();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            audioManager.ToggleMuteState();
        }
    }

    private IEnumerator PlayNextScene()
    {
        if (cutsceneIndex < cutscenes.Length)
        {
            currentCutScene = cutscenes[cutsceneIndex];
            UpdateCharacter(currentCutScene);
            currentCutScene.SetupObjects(mainCamera, textPanel, typewriterEffect);
            yield return currentCutScene.Play();  
        }
        else
        {
            audioManager.PauseMusic();
            sceneManager.StartGame();
        }
    }

    private void UpdateCharacter(CutsceneBehavior currentCutScene)
    {
        bool textPanelActive = textPanel.activeSelf;
        textPanel.SetActive(true);
        Sprite characterSprite = currentCutScene.CharacterImage;
        string characterNameString = currentCutScene.CharacterName;
        characterImage.GetComponent<Image>().sprite  = characterSprite;
        characterName.GetComponentInChildren<TextMeshProUGUI>().text = characterNameString;
        characterImage.SetActive(characterSprite != null);
        characterName.SetActive(!string.IsNullOrEmpty(characterNameString));
        textPanel.SetActive(textPanelActive);
    }

    private IEnumerator ExitScene()
    {
        if (cutsceneIndex + 1 < cutscenes.Length && cutscenes[cutsceneIndex + 1].NeedsPrewarm()) cutscenes[cutsceneIndex + 1].Prewarm();
        currentCutScene.Interrupt();
        yield return currentCutScene.Exit();
        HandleMusic();
        exiting = false;
        cutsceneIndex ++;
        yield return PlayNextScene();
    }

    private void EndScene()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
        exiting = true;
        StartCoroutine(ExitScene());
    }

    private void HandleMusic()
    {
        if (ShouldKeepTrack()) return;
        float fadeTime = cutscenes[cutsceneIndex+1].GetFadeSoundTime();
        audioManager.FadeOutMusic(fadeTime);
        audioManager.FadeOutSFX(fadeTime);
        audioManager.FadeInMusicAndAmbient(cutscenes[cutsceneIndex+1].GetMusic(), fadeTime);
    }

    private bool ShouldKeepTrack()
    {
        return cutsceneIndex > cutscenes.Length - 2 || cutscenes[cutsceneIndex + 1].ShouldConinueTrack();
    }
}