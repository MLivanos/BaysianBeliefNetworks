using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class GameBootManager : MonoBehaviour
{
    [SerializeField] private GameObject loadMenuCanvas;
    [SerializeField] private List<ButtonFontChangeEffect> buttonFontEffects;
    [SerializeField] private List<PlayButtonGlitch> glitches;
    [SerializeField] private string saveFileName = "savegame.json";
    private TransitionToCutscenes transitionEffect;
    private SceneManagerScript sceneManager;
    private SaveSystem saveSystem;

    private string saveFilePath;

    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);
        saveSystem = FindObjectOfType<SaveSystem>();
        sceneManager = FindObjectOfType<SceneManagerScript>();
        transitionEffect = FindObjectOfType<TransitionToCutscenes>();
    }

    public void CheckForSave()
    {
        if (File.Exists(saveFilePath)) loadMenuCanvas.SetActive(true);
        else PlayIntroSequence();
    }

    public void StartNewGame()
    {
        if (saveSystem != null) saveSystem.DeleteSaveData();

        loadMenuCanvas.SetActive(false);
        PlayIntroSequence();
    }

    public void LoadGame()
    {
        if (saveSystem != null) saveSystem.SetShouldLoadFlag();
        sceneManager.StartGame();
    }

    private void PlayIntroSequence()
    {
        transitionEffect.PerformTransition();
        foreach(ButtonFontChangeEffect buttonFontEffect in buttonFontEffects) buttonFontEffect.Click();
        foreach(PlayButtonGlitch glitch in glitches) glitch.TriggerGlitch();
        loadMenuCanvas.SetActive(false);
    }
}