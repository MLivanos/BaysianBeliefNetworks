using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private float timeBetweenCharacters = 0.1f;
    [SerializeField] private string soundName;
    [SerializeField] private string specialCharacters;
    [SerializeField] private float specialWaitTimeMultiplier;

    private AudioManager audioManager;
    private TextMeshProUGUI textComponent;
    private Coroutine typingCoroutine;
    private string fullText;

    void Start()
    {
        audioManager = AudioManager.instance;
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateText(string newText)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        fullText = newText;
        typingCoroutine = StartCoroutine(TypeText());
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
            if (soundName != "") audioManager.PlayEffect(soundName);
            float waitTime = specialCharacters.Contains(letter) ? timeBetweenCharacters * specialWaitTimeMultiplier : timeBetweenCharacters;
            yield return new WaitForSeconds(waitTime);
        }
        typingCoroutine = null;
    }

    public void Clear()
    {
        textComponent.text = "";
    }
}
