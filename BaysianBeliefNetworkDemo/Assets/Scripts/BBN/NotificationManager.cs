using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text eventText;
    [SerializeField] private TMP_Text alienMessageText;
    [SerializeField] private TMP_Text alienEventText;

    [Header("Timing")]
    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] private float stayTime = 2f;

    [Header("Visual Effects")]
    [SerializeField] private List<FadableElement> fadableElements;

    private Coroutine notificationCoroutine;

    public void ShowNotification(string message, string description, bool alienText=false)
    {
        if (notificationCoroutine != null)
        {
            StopCoroutine(notificationCoroutine);
        }

        notificationCoroutine = StartCoroutine(NotifyUserRoutine(message, description, alienText));
    }

    private IEnumerator NotifyUserRoutine(string message, string description, bool alienText)
    {
        TMP_Text messageElement = alienText ? alienMessageText : messageText;
        TMP_Text eventElement = alienText ? alienEventText : eventText;
        TMP_Text messageElementToHide = alienText ? messageText : alienMessageText;
        TMP_Text eventElementToHide = alienText ? eventText : alienEventText;

        messageElement.gameObject.SetActive(true);
        eventElement.gameObject.SetActive(true);
        messageElementToHide.gameObject.SetActive(false);
        eventElementToHide.gameObject.SetActive(false);

        notificationPanel.SetActive(true);
        messageElement.text = message;
        eventElement.text = description;

        foreach (var element in fadableElements)
        {
            if (element.gameObject.activeSelf) element.FadeIn(fadeTime);
        }

        yield return new WaitForSeconds(fadeTime + stayTime);

        foreach (var element in fadableElements)
        {
            if (element.gameObject.activeSelf) element.FadeOut(fadeTime);
        }

        yield return new WaitForSeconds(fadeTime);

        notificationPanel.SetActive(false);
        notificationCoroutine = null;
    }

    public void ClearNotification()
    {
        StopCoroutine(notificationCoroutine);
        foreach(FadableElement element in fadableElements)
        {
            if (element.gameObject.activeSelf) element.SetAlpha(0f);
        }
    }
}