using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private ObjectiveSpawner objectiveSpawner;
    [SerializeField] private DropdownList dropdownList;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject overlayCanvas;
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private RectTransform highlightArea;
    [SerializeField] private List<TutorialStep> tutorialSteps = new List<TutorialStep>();
    [SerializeField] private bool debug;
    private int currentStep = 0;
    private Vector2 originalHighlightSize;
    
    private void Start()
    {
        if (debug) PlayerPrefs.SetInt("TutorialCompleted", 0);
        PlayerPrefs.Save();
        if (PlayerPrefs.GetInt("TutorialCompleted", 0) == 0) StartTutorial();
        else EndTutorial();
    }

    private void StartTutorial()
    {
        tutorialSteps[0].Initialize(this);
    }

    public void NextStep()
    {
        if (currentStep >= tutorialSteps.Count)
        {
            EndTutorial();
            return;
        }
        tutorialSteps[currentStep].ClearObjectives();
        tutorialSteps[++currentStep].Initialize(this);
    }
    
    private void EndTutorial()
    {
        gameManager.PromptGameMode();
        overlayCanvas.SetActive(false);
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();
        gameObject.SetActive(false);
    }

    public DropdownList GetDropdownList()
    {
        return dropdownList;
    }

    public ObjectiveSpawner GetObjectSpawner()
    {
        return objectiveSpawner;
    }
}