using UnityEngine;
using System.Collections;

public class SemiCircleCameraMover : MonoBehaviour
{
    [Header("Path")]
    [Min(0.01f)] public float radius = 5f;
    [Min(0.01f)] public float duration = 2f;
    [Tooltip("If true, rotate clockwise relative to the chosen axis.")]
    public bool clockwise = false;

    [Header("Axis")]
    [Tooltip("Rotation axis for the arc. If Use Camera Up is true, this is ignored.")]
    public Vector3 axisWorld = Vector3.up;
    [Tooltip("If true, uses the camera's current up vector as the rotation axis instead of axisWorld.")]
    public bool useCameraUp = false;

    [Header("Easing")]
    public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Trigger")]
    [Tooltip("Start automatically on Start() in play mode.")]
    public float delay = 0.0f;
    public bool playOnStart = true;

    Coroutine _running;

    void Start()
    {
        if (Application.isPlaying && playOnStart)
        {
            Play();
        }
    }

    [ContextMenu("Play")]
    public void Play()
    {
        if (_running != null) StopCoroutine(_running);
        _running = StartCoroutine(MoveSemiCircleRoutine());
    }

    public void Play(float newRadius, float newDuration, Vector3 newAxisWorld, bool newClockwise, bool newUseCameraUp = false)
    {
        radius = Mathf.Max(0.01f, newRadius);
        duration = Mathf.Max(0.01f, newDuration);
        axisWorld = newAxisWorld;
        clockwise = newClockwise;
        useCameraUp = newUseCameraUp;
        Play();
    }

    IEnumerator MoveSemiCircleRoutine()
    {
        yield return new WaitForSeconds(delay);
        Vector3 center = transform.position + transform.forward * radius;
        Vector3 axis = useCameraUp ? transform.up : axisWorld;
        if (axis.sqrMagnitude < 1e-6f) axis = Vector3.up; // safety
        axis.Normalize();

        Vector3 startOffset = transform.position - center;
        if (startOffset.sqrMagnitude < 1e-6f)
        {
            startOffset = Vector3.Cross(axis, transform.forward).normalized * radius;
        }
        else
        {
            startOffset = startOffset.normalized * radius;
        }

        float totalDegrees = 180f * (clockwise ? -1f : 1f);
        Quaternion startRot = transform.rotation;

        float t = 0f;
        while (t < duration)
        {
            float u = ease.Evaluate(Mathf.Clamp01(t / duration));
            float angle = totalDegrees * u;

            Vector3 offset = Quaternion.AngleAxis(angle, axis) * startOffset;
            Vector3 pos = center + offset;

            transform.position = pos;
            transform.LookAt(center);

            t += Time.deltaTime;
            yield return null;
        }

        {
            Vector3 offset = Quaternion.AngleAxis(totalDegrees, axis) * startOffset;
            Vector3 pos = center + offset;
            transform.position = pos;
            transform.LookAt(center);
        }

        _running = null;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // Preview the arc based on current inspector values (in edit or play mode)
        Vector3 center = transform.position + transform.forward.normalized * Mathf.Max(0.01f, radius);
        Vector3 axis = (useCameraUp ? transform.up : axisWorld);
        if (axis.sqrMagnitude < 1e-6f) axis = Vector3.up;
        axis.Normalize();

        Vector3 startOffset = transform.position - center;
        if (startOffset.sqrMagnitude < 1e-6f)
            startOffset = Vector3.Cross(axis, transform.forward).normalized * Mathf.Max(0.01f, radius);
        else
            startOffset = startOffset.normalized * Mathf.Max(0.01f, radius);

        float totalDegrees = 180f * (clockwise ? -1f : 1f);

        Gizmos.color = Color.cyan;
        const int steps = 32;
        Vector3 prev = center + startOffset;
        for (int i = 1; i <= steps; i++)
        {
            float u = i / (float)steps;
            float angle = totalDegrees * u;
            Vector3 p = center + (Quaternion.AngleAxis(angle, axis) * startOffset);
            Gizmos.DrawLine(prev, p);
            prev = p;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(center, 0.05f);
        Gizmos.DrawLine(center, center + startOffset);
    }
#endif
}