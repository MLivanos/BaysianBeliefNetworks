using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CutsceneBehavior : MonoBehaviour
{
    [SerializeField] protected AudioClip music;
    [SerializeField] protected GameObject scene;
    [SerializeField] protected Transform cameraMark;
    [SerializeField] protected Material skyMaterial;
    [SerializeField] protected float ambientIntensity;
    [SerializeField] protected bool fadeInPanel;
    [SerializeField] protected float fadeInTime;
    [SerializeField] protected float textPanelOpacity;
    [SerializeField] protected bool needsPrewarm;
    [SerializeField] protected bool isAtTop;
    protected FadableImage fadablePanel;
    protected TypewriterEffect typewriterEffect;
    protected GameObject textPanel;
    protected Transform cameraTransform;

    protected abstract IEnumerator PlayScene();
    public abstract void Interrupt();
    protected abstract IEnumerator ExitTransition();

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
        if (fadeInPanel) yield return fadablePanel.Fade(fadeInTime, true, textPanelOpacity / 255f);
        else fadablePanel.SetAlpha(textPanelOpacity / 255f);
    }

    protected void SetupCamera()
    {
        cameraTransform.parent = cameraMark;
        cameraTransform.localPosition = Vector3.zero;
        cameraTransform.localEulerAngles = Vector3.zero;
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
    }

    public virtual void Prewarm()
    {
        return;
    }

    public bool NeedsPrewarm()
    {
        return needsPrewarm;
    }
}
