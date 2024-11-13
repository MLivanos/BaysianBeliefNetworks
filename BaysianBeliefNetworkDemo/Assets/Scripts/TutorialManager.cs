using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class TutorialStep
{
    [TextArea] public string message;
    public GameObject icon;
    public Vector2 position;
    public Vector2 size;
}

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject overlayCanvas;
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private RectTransform highlightArea;
    [SerializeField] private List<TutorialStep> tutorialSteps = new List<TutorialStep>();
    private int currentStep = 0;
    private Vector2 originalHighlightSize;
    
    private void Start()
    {
        // TODO: Remove next two lines after testing
        PlayerPrefs.SetInt("TutorialCompleted", 0);
        PlayerPrefs.Save();
        originalHighlightSize = highlightArea.localScale;
        if (PlayerPrefs.GetInt("TutorialCompleted", 0) == 0)
        {
            StartTutorial();
        }
    }

    private void Update()
    {
        if (Enumerable.Range(0, 3).Any(Input.GetMouseButtonDown))
        {
            Vector3 mousePos = Input.mousePosition;
            NextStep(true);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            NextStep(false);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            EndTutorial();
        }
    }

    private void StartTutorial()
    {
        overlayCanvas.SetActive(true);
        currentStep = 0;
        ShowStep();
    }

    private void NextStep(bool forward)
    {
        currentStep += forward ? 1 : -1;
        ShowStep();
    }

    private void ShowStep()
    {
        if (currentStep >= tutorialSteps.Count)
        {
            EndTutorial();
            return;
        }

        var step = tutorialSteps[currentStep];
        tutorialText.text = step.message;
        highlightArea.position = new Vector2(Screen.width * step.position.x, Screen.height * step.position.y);
        highlightArea.localScale = originalHighlightSize * (step.size == Vector2.zero ? new Vector2(1f,1f) : step.size);
        if (step.icon)
        {
            Instantiate(step.icon, step.position, Quaternion.identity);
        }
    }

    private void EndTutorial()
    {
        overlayCanvas.SetActive(false);
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();
    }
}