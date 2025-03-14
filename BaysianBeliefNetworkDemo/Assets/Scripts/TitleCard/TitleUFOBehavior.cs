using UnityEngine;
using System.Collections;

public class TitleUFOBehaviorPortal : MonoBehaviour
{
    [Header("Title Settings")]
    public FadableTextMeshPro[] titleFadeInText;
    public float titleFadeInTime;
    public TextGlow titleGlow;
    public bool shouldGlowTitle = true;

    [Header("UFO Movement Settings")]
    [Tooltip("Target position where the UFO will exit the portal.")]
    public Vector3 targetPosition = Vector3.zero;
    [Tooltip("Duration for the UFO to move from the portal to target.")]
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

    [Header("Zoom to Planet Settings")]
    [Tooltip("Duration for the UFO to move toward the planet.")]
    public float toPlanetMotionDuration = 20f;
    public Vector3 planetLocation;
    public Vector3 minimumScale;

    [Header("Portal Settings")]
    public Transform portalTransform;          // Portal transform.
    [Tooltip("Duration for the portal to grow from tiny to full size.")]
    public float portalGrowDuration = 1f;
    [Tooltip("Duration for the portal to shrink back.")]
    public float portalShrinkDuration = 0.5f;
    [Tooltip("Full scale of the portal when active.")]
    public Vector3 portalFullScale = Vector3.one;
    [Tooltip("Initial (tiny) scale of the portal.")]
    public Vector3 portalInitialScale = new Vector3(0.05f, 0.05f, 0.05f);

    [Header("Disappear Settings")]
    [Tooltip("The particle effect to activate on crash.")]
    public ParticleSystem landingEffect;

    [Header("Background Image")]
    public FadableImage backgroundFadableImage;
    public float fadeInTime;

    private float elapsedMoveTime = 0f;

    void Start()
    {
        // Ensure background and title start faded out.
        backgroundFadableImage.SetAlpha(0f);
        foreach (FadableTextMeshPro element in titleFadeInText)
            element.SetAlpha(0f);

        if (landingEffect != null)
            landingEffect.Stop();

        // Start with both UFO and portal deactivated.
        ufoTransform.gameObject.SetActive(false);
        portalTransform.gameObject.SetActive(false);

        StartCoroutine(StartBehavior());
    }

    void Update()
    {
        // Rotate the UFO continuously.
        ufoTransform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
    }

    IEnumerator StartBehavior()
    {
        // 1. Fade in the title.
        yield return StartCoroutine(FadeInTitle());
        if (shouldGlowTitle) titleGlow.StartGlow();

        // 2. Activate and grow the portal.
        portalTransform.localScale = portalInitialScale;
        portalTransform.gameObject.SetActive(true);
        yield return StartCoroutine(ScaleTransform(portalTransform, portalFullScale, portalGrowDuration));
        yield return new WaitForSeconds(0.75f);

        // 3. Activate the UFO at the portal's position.
        ufoTransform.gameObject.SetActive(true);

        // 4. Move UFO from the portal to the target exit position.
        yield return StartCoroutine(MoveTowardsLocation(targetPosition, ufoTransform.lossyScale, moveDuration));

        // 5. Shrink and deactivate the portal.
        yield return StartCoroutine(ScaleTransform(portalTransform, portalInitialScale, portalShrinkDuration));
        portalTransform.gameObject.SetActive(false);

        // 6. UFO oscillates in place for a while.
        yield return StartCoroutine(UFOMotions());

        // 7. UFO zooms from its current location to the planet.
        yield return StartCoroutine(MoveTowardsLocation(planetLocation, minimumScale, toPlanetMotionDuration));

        // 8. Crash: Play landing effect.
        yield return StartCoroutine(PlayLight());

        // 9. Short wait, then fade in background image.
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(FadeInImage());

        // 10. Deactivate the UFO.
        ufoTransform.gameObject.SetActive(false);
    }

    IEnumerator FadeInTitle()
    {
        foreach (FadableTextMeshPro element in titleFadeInText)
        {
            element.FadeIn(titleFadeInTime);
            yield return new WaitForSeconds(titleFadeInTime * 2);
        }
    }

    // This coroutine moves and scales the UFO over a given duration.
    IEnumerator MoveTowardsLocation(Vector3 endingPos, Vector3 endScale, float duration)
    {
        Vector3 startingPos = ufoTransform.position;
        Vector3 startingScale = ufoTransform.lossyScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            ufoTransform.position = Vector3.Lerp(startingPos, endingPos, elapsed / duration);
            ufoTransform.localScale = Vector3.Lerp(startingScale, endScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        ufoTransform.position = endingPos;
    }

    // Scales a transform from its current scale to a target scale over duration.
    IEnumerator ScaleTransform(Transform t, Vector3 targetScale, float duration)
    {
        Vector3 startingScale = t.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            t.localScale = Vector3.Lerp(startingScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        t.localScale = targetScale;
    }

    IEnumerator UFOMotions()
    {
        float originalY = ufoTransform.position.y;
        float time = 0f;

        while (time < motionDuration)
        {
            float newY = originalY + Mathf.Sin(Time.time * oscillationFrequency * 2 * Mathf.PI) * oscillationAmplitude;
            ufoTransform.position = new Vector3(ufoTransform.position.x, newY, ufoTransform.position.z);
            time += Time.deltaTime;
            yield return null;
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
        if (landingEffect != null)
            landingEffect.Stop();
    }

    IEnumerator FadeInImage()
    {
        float timer = 0f;
        while (timer < fadeInTime)
        {
            backgroundFadableImage.SetAlpha(timer / fadeInTime);
            timer += Time.deltaTime;
            yield return null;
        }
        backgroundFadableImage.SetAlpha(1);
    }
}