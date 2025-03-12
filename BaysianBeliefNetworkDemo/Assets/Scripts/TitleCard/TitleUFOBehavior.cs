using UnityEngine;
using System.Collections;

public class TitleUFOBehavior : MonoBehaviour
{
    [Header("Title Settings")]

    public FadableTextMeshPro[] titleFadeInText;
    public float titleFadeInTime;

    [Header("Movement Settings")]

    [Tooltip("Target position where the UFO will settle.")]
    public Vector3 targetPosition = Vector3.zero;

    [Tooltip("Duration for the UFO to move into the scene.")]
    public float moveDuration = 5f;

    [Header("Rotation Settings")]
    [Tooltip("Rotation speed around the Y-axis (degrees per second).")]
    public float rotationSpeed = 90f;

    [Header("UFO Motion Settings")]
    [Tooltip("Amplitude of the up-and-down oscillation.")]
    public Transform ufoTransform;
    public float oscillationAmplitude = 2f;
    public float oscillationFrequency = 1f;
    public float motionDuration = 10f;

    [Tooltip("Point to move towards")]
    public float toPlanetMotionDuration = 20f;
    public Vector3 planetLocation;
    public Vector3 minimumScale;
    [Header("Disappear Settings")]
    [Tooltip("The particle effect to activate on collision.")]
    public ParticleSystem landingEffect;

    [Header("BackgroundImage")]
    public FadableImage backgroundFadableImage;
    public float fadeInTime;

    private Vector3 startPosition;
    private Vector3 initialPosition;
    private float elapsedMoveTime = 0f;

    void Start()
    {
        backgroundFadableImage.SetAlpha(0f);
        foreach(FadableTextMeshPro element in titleFadeInText) element.SetAlpha(0f);
        if (landingEffect != null) landingEffect.Stop();
        initialPosition = targetPosition;
        StartCoroutine(StartBehavior());
    }

    void Update()
    {
        ufoTransform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
    }

    IEnumerator StartBehavior()
    {
        yield return StartCoroutine(FadeInTitle());
        yield return StartCoroutine(MoveTowardsLocation(targetPosition, ufoTransform.lossyScale, moveDuration));
        yield return StartCoroutine(UFOMotions());
        yield return StartCoroutine(MoveTowardsLocation(planetLocation, minimumScale, toPlanetMotionDuration));
        yield return PlayLight();
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(FadeInImage());
    }

    IEnumerator FadeInTitle()
    {
        foreach(FadableTextMeshPro element in titleFadeInText)
        {
            element.FadeIn(titleFadeInTime);
            yield return new WaitForSeconds(titleFadeInTime*2);
        }
    }

    IEnumerator MoveTowardsLocation(Vector3 endingPos, Vector3 endScale, float duration)
    {
        Vector3 startingPos = ufoTransform.position;
        Vector3 startingScale = ufoTransform.lossyScale;
        elapsedMoveTime = 0f;

        while (elapsedMoveTime < moveDuration)
        {
            // Lerp the position based on elapsed time
            ufoTransform.position = Vector3.Lerp(startingPos, endingPos, elapsedMoveTime / duration);
            ufoTransform.localScale = Vector3.Lerp(startingScale, endScale, elapsedMoveTime / duration);
            elapsedMoveTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the UFO reaches the exact target position
        ufoTransform.position = endingPos;
    }

    IEnumerator UFOMotions()
    {
        float originalY = ufoTransform.position.y;
        float time = 0f;

        while (time < motionDuration)
        {
            // Apply up-and-down oscillation
            float newY = originalY + Mathf.Sin(Time.time * oscillationFrequency * 2 * Mathf.PI) * oscillationAmplitude;
            ufoTransform.position = new Vector3(ufoTransform.position.x, newY, ufoTransform.position.z);

            time += Time.deltaTime;
            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(PlayLight());
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }
    }

    private IEnumerator PlayLight()
    {
        if (landingEffect != null)
        {
            landingEffect.gameObject.SetActive(true);
            landingEffect.Play();
        }
        yield return new WaitForSeconds(1.5f);
        landingEffect.Stop();
    }

    private IEnumerator FadeInImage()
    {
        float timer = 0f;
        while(timer < fadeInTime)
        {
            backgroundFadableImage.SetAlpha(timer/fadeInTime);
            timer += Time.deltaTime;
            yield return null;
        }
        backgroundFadableImage.SetAlpha(1);
    }
}
