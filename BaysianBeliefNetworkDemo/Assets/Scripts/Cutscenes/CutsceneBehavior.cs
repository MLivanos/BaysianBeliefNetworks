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
    [SerializeField] protected bool needsPrewarm;
    protected TypewriterEffect typewriterEffect;
    protected GameObject textPanel;
    protected Transform cameraTransform;

    protected abstract IEnumerator PlayScene();
    protected abstract IEnumerator ExitTransition();

    protected void SetupScene()
    {
        scene.SetActive(true);
        SetupCamera();
        RenderSettings.ambientIntensity = ambientIntensity;
        RenderSettings.skybox = skyMaterial;
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
    }

    public IEnumerator Play()
    {
        SetupScene();
        return PlayScene();
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
