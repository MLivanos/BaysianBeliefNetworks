using UnityEngine;
using System.Collections;

public class TitleUFOBehavior : MonoBehaviour
{
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
    public float oscillationAmplitude = 2f;

    [Tooltip("Frequency of the up-and-down oscillation.")]
    public float oscillationFrequency = 1f;

    [Tooltip("Duration to perform UFO-like motions before stopping.")]
    public float motionDuration = 10f;

    [Tooltip("Point to move towards")]
    public float toPlanetMotionDuration = 20f;
    public Vector3 planetLocation;
    public Vector3 minimumScale;
    [Header("Disappear Settings")]
    [Tooltip("The particle effect to activate on collision.")]
    public ParticleSystem landingEffect;

    private Vector3 startPosition;
    private Vector3 initialPosition;
    private float elapsedMoveTime = 0f;
    private float elapsedMotionTime = 0f;
    private bool instantiatedGleam = false;

    void Start()
    {
        if (landingEffect != null)
        {
            landingEffect.Stop();
        }
        initialPosition = targetPosition;
        StartCoroutine(StartBehavior());
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
    }

    IEnumerator StartBehavior()
    {
        yield return StartCoroutine(MoveTowardsLocation(targetPosition, transform.lossyScale, moveDuration));
        yield return StartCoroutine(UFOMotions());
        yield return StartCoroutine(MoveTowardsLocation(planetLocation, minimumScale, toPlanetMotionDuration));
    }

    IEnumerator MoveTowardsLocation(Vector3 endingPos, Vector3 endScale, float duration)
    {
        Vector3 startingPos = transform.position;
        Vector3 startingScale = transform.lossyScale;
        elapsedMoveTime = 0f;

        while (elapsedMoveTime < moveDuration)
        {
            // Lerp the position based on elapsed time
            transform.position = Vector3.Lerp(startingPos, endingPos, elapsedMoveTime / duration);
            transform.localScale = Vector3.Lerp(startingScale, endScale, elapsedMoveTime / duration);
            elapsedMoveTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the UFO reaches the exact target position
        transform.position = endingPos;

        // Start UFO-like motions after moving in
        StartCoroutine(UFOMotions());
    }

    IEnumerator UFOMotions()
    {
        float originalY = transform.position.y;
        float time = 0f;

        while (time < motionDuration)
        {
            // Apply up-and-down oscillation
            float newY = originalY + Mathf.Sin(Time.time * oscillationFrequency * 2 * Mathf.PI) * oscillationAmplitude;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

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
}
