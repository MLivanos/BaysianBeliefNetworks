using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CutsceneBehavior : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] protected GameObject scene;
    [SerializeField] protected Transform cameraMark;
    [SerializeField] protected bool needsPrewarm;
    [Header("Lighting")]
    [SerializeField] protected Material skyMaterial;
    [SerializeField] protected float ambientIntensity;
    [Header("Text")]
    [SerializeField] protected string text;
    [SerializeField] protected bool fadeInPanel;
    [SerializeField] protected float fadeInTime;
    [SerializeField] protected float textPanelOpacity = 160;
    [SerializeField] protected bool isAtTop;
    [Header("Character")]
    [SerializeField] protected Sprite characterImage;
    [SerializeField] protected string characterName;
    protected AudioManager audioManager;
    protected FadableImage fadablePanel;
    protected TypewriterEffect typewriterEffect;
    protected GameObject textPanel;
    protected Transform cameraTransform;
    // Public accessors
    public Sprite CharacterImage => characterImage;
    public string CharacterName => characterName;

    protected abstract IEnumerator PlayScene();
    public abstract void Interrupt();
    protected abstract IEnumerator ExitTransition();

    private void Awake()
    {
        audioManager = AudioManager.instance;
    }

    protected void SetupScene()
    {
        scene.SetActive(true);
        SetupCamera();
        RenderSettings.ambientIntensity = ambientIntensity;
        RenderSettings.skybox = skyMaterial;
        SetupTextPanel();
    }

    protected void SetupTextPanel()
    {
        RectTransform rectTransform = textPanel.GetComponent<RectTransform>();
        float anchorPosition = isAtTop ? 1f : 0f;
        rectTransform.anchorMin = new Vector2(0.5f, anchorPosition);
        rectTransform.anchorMax = new Vector2(0.5f, anchorPosition);
        rectTransform.pivot = new Vector2(0.5f, anchorPosition);
        rectTransform.anchoredPosition = Vector2.zero;
    }

    protected IEnumerator ViewPanel()
    {
        textPanel.SetActive(true);
        if (fadeInPanel) yield return fadablePanel.Fade(fadeInTime, true, textPanelOpacity / 255f);
        else fadablePanel.SetAlpha(textPanelOpacity / 255f);
    }

    protected void SetupCamera()
    {
        cameraTransform.parent = cameraMark;
        cameraTransform.localPosition = Vector3.zero;
        cameraTransform.localEulerAngles = Vector3.zero;
    }

    protected void AnimateText()
    {
        typewriterEffect.UpdateText(text);
    }

    public void SetupObjects(Transform mainCameraTransform, GameObject textPanelObject, TypewriterEffect textAnimation)
    {
        cameraTransform = mainCameraTransform;
        textPanel = textPanelObject;
        typewriterEffect = textAnimation;
        fadablePanel = textPanelObject.GetComponent<FadableImage>();
    }

    public IEnumerator Play()
    {
        SetupScene();
        yield return PlayScene();
    }

    public IEnumerator Exit()
    {
        yield return ExitTransition();
        scene.SetActive(false);
        if (typewriterEffect.gameObject.activeInHierarchy) typewriterEffect.Clear();
        textPanel.SetActive(false);
    }

    public virtual void Prewarm()
    {
        scene.SetActive(true);
    }

    public bool NeedsPrewarm()
    {
        return needsPrewarm;
    }
}
