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

    [Header("Timing")]
    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] private float stayTime = 2f;

    [Header("Visual Effects")]
    [SerializeField] private List<FadableElement> fadableElements;

    private Coroutine notificationCoroutine;

    public void ShowNotification(string message, string description)
    {
        if (notificationCoroutine != null)
        {
            StopCoroutine(notificationCoroutine);
        }

        notificationCoroutine = StartCoroutine(NotifyUserRoutine(message, description));
    }

    private IEnumerator NotifyUserRoutine(string message, string description)
    {
        notificationPanel.SetActive(true);
        messageText.text = message;
        eventText.text = description;

        foreach (var element in fadableElements)
        {
            element.FadeIn(fadeTime);
        }

        yield return new WaitForSeconds(fadeTime + stayTime);

        foreach (var element in fadableElements)
        {
            element.FadeOut(fadeTime);
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
            element.SetAlpha(0f);
        }
    }
}