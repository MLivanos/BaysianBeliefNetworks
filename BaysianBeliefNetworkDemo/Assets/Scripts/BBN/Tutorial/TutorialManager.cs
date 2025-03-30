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
    [Header("Tooltip Objects")]
    [SerializeField] private RectTransform tooltipPanelTransform;
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private bool debug;
    private int currentStep = 0;
    private Vector2 originalHighlightSize;
    private bool tutorialOngoing = false;
    private string lastMessageID = "";

    public static TutorialManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    private void Start()
    {
        tooltipPanelTransform.gameObject.SetActive(false);
        if (debug) PlayerPrefs.SetInt("TutorialCompleted", 0);
        PlayerPrefs.Save();
        if (PlayerPrefs.GetInt("TutorialCompleted", 0) == 0) tutorialSelectionWindow.SetActive(true);
        else EndTutorial();
    }

    public void StartTutorial()
    {
        tutorialOngoing = true;
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
        dropdownList.transform.parent.gameObject.SetActive(false);
        gameManager.PromptGameMode();
        tutorialSelectionWindow.SetActive(false);
        overlayCanvas.SetActive(false);
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();
        interactionBlocker.SetActive(false);
        gameObject.SetActive(false);
        tutorialOngoing = false;
    }

    private void HideIncrementalObjects()
    {
        foreach(GameObject obj in incrementalHiddenObjects)
        {
            obj.SetActive(false);
        }
    }

    public bool HandleTooltipHoverEnter(string triggerName)
    {
        if (!tutorialOngoing) return false;
        TutorialTooltipMessage tooltipMessage = tutorialSteps[currentStep].FindTooltip(triggerName);
        if (tooltipMessage == null) return true;
        if (triggerName != lastMessageID)
        {
            tooltipPanelTransform.GetComponent<PopEffect>().SetProgress(0f);    
        }
        lastMessageID = triggerName;
        tooltipPanelTransform.GetComponent<PopEffect>().PlayPopIn();
        tooltipPanelTransform.localPosition = tooltipMessage.worldPositionOverride;
        tooltipPanelTransform.sizeDelta = tooltipMessage.boxSize;
        Vector2 textSize = new Vector2(tooltipMessage.boxSize.x-25f,tooltipMessage.boxSize.y-70f);
        tooltipText.GetComponent<RectTransform>().sizeDelta = textSize;
        tooltipText.text = tooltipMessage.tooltipText;
        return true;
    }

    public void HandleTooltipHoverExit()
    {
        if (tooltipPanelTransform.gameObject.activeSelf) tooltipPanelTransform.GetComponent<PopEffect>().PlayPopDown();
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