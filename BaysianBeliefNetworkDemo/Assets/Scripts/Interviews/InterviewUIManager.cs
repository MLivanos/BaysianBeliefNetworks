using System.Collections;
using UnityEngine;
using TMPro;

public class InterviewUIManager : InterviewEventSystem
{
    [SerializeField] private TMP_Text eyewitnessAccountText;
    [SerializeField] private TMP_Text evidenceText;
    [SerializeField] private SlideInBehavior slideInBehavior;
    [SerializeField] private GameObject eyewitnessPanel;
    
    public void DisplayEyewitnessAccount(string eyewitnessAccount)
    {
        eyewitnessPanel.SetActive(true);
        eyewitnessAccountText.text = eyewitnessAccount;
    }

    public void DisplayEvidence(string evidence, bool slide=true)
    {
        evidenceText.text = $"P(Aliens|{evidence}) > P(Aliens)?";
        if(slide) slideInBehavior.BeginSlideIn();
    }

    public void SendBelief(bool belief)
    {
        interviewManager.SetBelief(belief);
        interviewManager.Advance();
        eyewitnessPanel.SetActive(false);
        slideInBehavior.BeginSlideOut();
    }
}