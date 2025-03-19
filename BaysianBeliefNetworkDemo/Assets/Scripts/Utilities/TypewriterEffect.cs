using System;
using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private float timeBetweenCharacters = 0.1f;
    [SerializeField] private string typingSoundName;
    [SerializeField] private string deletingSoundName;
    [SerializeField] private int charsBetweenSounds;
    [SerializeField] private string specialCharacters;
    [SerializeField] private float specialWaitTimeMultiplier;

    private AudioManager audioManager;
    private TextMeshProUGUI textComponent;
    private Coroutine typingCoroutine;
    private string fullText;
    private int charsBeforeSound = 0;

    void Awake()
    {
        audioManager = AudioManager.instance;
        textComponent = GetComponent<TextMeshProUGUI>();
        fullText = textComponent.text;
    }

    public void UpdateText(string newText)
    {
        charsBeforeSound = 1;
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        fullText = newText;
        typingCoroutine = StartCoroutine(TypeText());
    }

    public void UpdateText()
    {
        StartCoroutine(TypeText());
    }

    public void TypewriterDelete()
    {
        charsBeforeSound = 1;
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(UntypeText());    
    }

    private IEnumerator UntypeText()
    {
        string originalText = String.Copy(fullText);
        fullText = "";
        for (int i=0; i<=originalText.Length; i++)
        {
            textComponent.text = originalText.Substring(0,originalText.Length-i);
            PlayTypingSound(false);
            yield return new WaitForSeconds(timeBetweenCharacters);
        }
    }

    private void PlayTypingSound(bool typingIn)
    {
        string soundName = typingIn ? typingSoundName : deletingSoundName;
        if (soundName != "" && charsBeforeSound-- <= 0)
        {
            audioManager.PlayEffect(soundName);
            charsBeforeSound = charsBetweenSounds;
        }
    }

    public void Interrupt()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        textComponent.text = fullText;
        typingCoroutine = null;
    }

    public bool IsTyping()
    {
        return typingCoroutine != null;
    }

    private IEnumerator TypeText()
    {
        textComponent.text = "";
        foreach (char letter in fullText)
        {
            textComponent.text += letter;
            PlayTypingSound(true);
            float waitTime = GetWaitTimeForCharacter(letter);
            yield return new WaitForSeconds(waitTime);
        }
        typingCoroutine = null;
    }

    public void Clear()
    {
        textComponent.text = "";
    }

    public float GetTypingTime(string text, bool typing=true)
    {
        if (!typing) return text.Length * timeBetweenCharacters;
        float totalTime = 0f;
        foreach(char letter in text)
        {
            totalTime += GetWaitTimeForCharacter(letter);
        }
        return totalTime;
    }

    private float GetWaitTimeForCharacter(char letter)
    {
        if (specialCharacters.Contains(letter))
        {
            charsBeforeSound = 0;
            return timeBetweenCharacters * specialWaitTimeMultiplier;
        }
        return timeBetweenCharacters;
    }
}
