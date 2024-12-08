using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CutsceneBehavior : MonoBehaviour
{
    [SerializeField] private AudioClip music;
    [SerializeField] private GameObject scene;
    [SerializeField] private Transform cameraMark;
    [SerializeField] private Material skyMaterial;
    [SerializeField] private float ambientIntensity;
    [SerializeField] private bool needsPrewarm;
    private Transform cameraTransform;

    protected abstract IEnumerator PlayScene();
    protected abstract IEnumerator ExitTransition();

    protected void SetupScene()
    {
        SetupCamera();
        RenderSettings.ambientIntensity = ambientIntensity;
        RenderSettings.skybox = skyMaterial;
    }

    protected void SetupCamera()
    {
        cameraTransform.parent = cameraMark;
        cameraTransform.position = Vector3.zero;
        cameraTransform.eulerAngles = Vector3.zero;
    }

    public void SetCameraTransform(Transform mainCameraTransform)
    {
        cameraTransform = mainCameraTransform;
    }

    public void Play()
    {
        StartCoroutine(PlayScene());
    }

    public void Exit()
    {
        StartCoroutine(ExitTransition());
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
