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
    [SerializeField] private float maxFontSize = 32f;
    [SerializeField] private bool resizeText = false;
    [SerializeField] private bool smartDetectTokens = false;

    private AudioManager audioManager;
    private TextMeshProUGUI textComponent;
    private Coroutine typingCoroutine;
    private string fullText;
    private int charsBeforeSound = 0;

    public string Text()=>textComponent.text;

    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        fullText = textComponent.text;
    }

    private void Start()
    {
        audioManager = AudioManager.instance;
    }

    public void UpdateText(string newText)
    {
        charsBeforeSound = 1;
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        fullText = newText;
        UpdateText();
    }

    public void UpdateText()
    {
        if (resizeText) StartCoroutine(PrecomputeFontSizeAndType());
        else typingCoroutine = StartCoroutine(TypeText());
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
        int i = 0;
        while (i < fullText.Length)
        {
            string nextToken = SmartDetectToken(fullText.Substring(i));
            textComponent.text += nextToken;
            PlayTypingSound(true);
            float waitTime = GetWaitTimeForToken(nextToken);
            yield return new WaitForSeconds(waitTime);
            i += nextToken.Length;
        }
        typingCoroutine = null;
    }

    private string SmartDetectToken(string text)
    {
        if (string.IsNullOrEmpty(text))
            return "";

        if (!smartDetectTokens || !text.StartsWith("<"))
            return text[0].ToString();

        int tokenEnd = text.IndexOf('>');
        if (tokenEnd > 1)
        {
            string potentialToken = text.Substring(0, tokenEnd + 1);
            char nextChar = potentialToken[1];
            if (char.IsLetter(nextChar) || nextChar == '/') return potentialToken;
        }
        return text[0].ToString();
    }

    private IEnumerator PrecomputeFontSizeAndType()
    {
        FadableTextMeshPro fader = textComponent.GetComponent<FadableTextMeshPro>() ?? textComponent.gameObject.AddComponent<FadableTextMeshPro>();
        fader.SetAlpha(0f);
        textComponent.enableAutoSizing = true;
        textComponent.text = fullText;
        yield return null;
        float computedFontSize = textComponent.fontSize;
        textComponent.fontSize = Mathf.Min(maxFontSize, computedFontSize);
        textComponent.enableAutoSizing = false;
        Clear();
        fader.SetAlpha(1f);
        typingCoroutine = StartCoroutine(TypeText());
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

    private float GetWaitTimeForToken(string token)
    {
        if (token.Length > 1) return 0f;
        return GetWaitTimeForCharacter(token[0]);
    }
}
