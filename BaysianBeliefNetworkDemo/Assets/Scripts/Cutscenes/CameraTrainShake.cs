using System.Collections;
using UnityEngine;

public class CameraTrainShake : MonoBehaviour
{
    [SerializeField] private float minInterval = 2.5f;
    [SerializeField] private float maxInterval = 7.5f;
    [SerializeField] private float shakeDuration = 1f;
    [SerializeField] private float shakesPerCycle = 2f;
    [SerializeField] private float minShakeStrength = 0.02f;
    [SerializeField] private float maxShakeStrength = 0.04f;

    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.localPosition;
        StartCoroutine(ShakeLoop());
    }

    private IEnumerator ShakeLoop()
    {
        while (true)
        {
            float interval = Random.Range(minInterval, maxInterval);
            float strength = Random.Range(minShakeStrength, maxShakeStrength);
            yield return new WaitForSeconds(interval);
            yield return StartCoroutine(DoShake(shakeDuration, strength));
        }
    }

    private IEnumerator DoShake(float duration, float strength)
    {
        float elapsed = 0f;
        float periodicity = 2f * shakesPerCycle * Mathf.PI;
        Vector3 modifiedPos;

        while (elapsed < duration)
        {
            modifiedPos = transform.localPosition;
            modifiedPos.y += Mathf.Sin(Time.time * periodicity) * strength;
            transform.localPosition = modifiedPos;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}
