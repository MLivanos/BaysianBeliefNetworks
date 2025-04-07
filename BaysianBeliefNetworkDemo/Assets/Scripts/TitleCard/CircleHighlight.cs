using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CircleHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image circleImage;
    [SerializeField] private float drawTime = 1.0f;
    [SerializeField] private List<string> drawNoises;
    [SerializeField] private List<string> undrawNoises;
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = AudioManager.instance;
        if (circleImage == null) circleImage = GetComponent<Image>();
    }

    private Coroutine drawingCoroutine;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (drawingCoroutine != null) StopCoroutine(drawingCoroutine);
        PlayRandomNoise(drawNoises);
        drawingCoroutine = StartCoroutine(AnimateCircle(0f, 1f));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (drawingCoroutine != null) StopCoroutine(drawingCoroutine);
        PlayRandomNoise(undrawNoises);
        drawingCoroutine = StartCoroutine(AnimateCircle(circleImage.fillAmount, 0f));
    }

    IEnumerator AnimateCircle(float from, float to)
    {
        float time = 0f;
        while (time < drawTime)
        {
            time += Time.deltaTime;
            float t = time / drawTime;
            circleImage.fillAmount = Mathf.Lerp(from, to, t);
            yield return null;
        }
        circleImage.fillAmount = to;
    }

    private void PlayRandomNoise(List<string> noiseList)
    {
        if (audioManager == null || noiseList == null || noiseList.Count == 0) return;

        string chosenNoise = noiseList[UnityEngine.Random.Range(0, noiseList.Count)];
        if (!string.IsNullOrEmpty(chosenNoise))
        {
            audioManager.PlayEffect(chosenNoise);
        }
    }
}
