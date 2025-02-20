using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> incrementalHiddenObjects;
    [SerializeField] private GameObject tutorialSelectionWindow;
    [SerializeField] private GameObject interactionBlocker;
    [SerializeField] private ObjectiveSpawner objectiveSpawner;
    [SerializeField] private DropdownList dropdownList;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject overlayCanvas;
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private RectTransform highlightArea;
    [SerializeField] private List<TutorialStep> tutorialSteps = new List<TutorialStep>();
    [SerializeField] private TypewriterEffect typewriterEffect;
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private FadableTextMeshPro completionText;
    [SerializeField] private Image advanceButton;
    [SerializeField] private bool debug;
    private int currentStep = 0;
    private Vector2 originalHighlightSize;
    
    private void Start()
    {
        if (debug) PlayerPrefs.SetInt("TutorialCompleted", 0);
        PlayerPrefs.Save();
        if (PlayerPrefs.GetInt("TutorialCompleted", 0) == 0) tutorialSelectionWindow.SetActive(true);
        else EndTutorial();
    }

    public void StartTutorial()
    {
        HideIncrementalObjects();
        GameObject.Find("TimeLimit").SetActive(false);
        tutorialSelectionWindow.SetActive(false);
        tutorialSteps[0].Initialize(this);
    }

    public void NextStep()
    {
        tutorialSteps[currentStep].DestroyQuests();
        tutorialSteps[currentStep].ClearObjectives();
        if (++currentStep >= tutorialSteps.Count) EndTutorial();
        else tutorialSteps[currentStep].Initialize(this);
    }

    public void BlockInteractions(bool block)
    {
        interactionBlocker.SetActive(block);
    }
    
    public void EndTutorial()
    {
        gameManager.PromptGameMode();
        tutorialSelectionWindow.SetActive(false);
        overlayCanvas.SetActive(false);
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();
        gameObject.SetActive(false);
    }

    private void HideIncrementalObjects()
    {
        foreach(GameObject obj in incrementalHiddenObjects)
        {
            obj.SetActive(false);
        }
    }

    public void CloseMessages()
    {
        tutorialSteps[currentStep].CloseMessages();
    }

    public DropdownList GetDropdownList()
    {
        return dropdownList;
    }

    public ObjectiveSpawner GetObjectSpawner()
    {
        return objectiveSpawner;
    }

    public TypewriterEffect GetTypeWriterEffect()
    {
        return typewriterEffect;
    }

    public GameObject GetMessagePanel()
    {
        return messagePanel;
    }

    public FadableTextMeshPro GetCompletionText()
    {
        return completionText;
    }

    public Image GetAdvanceButton()
    {
        return advanceButton;
    }
}