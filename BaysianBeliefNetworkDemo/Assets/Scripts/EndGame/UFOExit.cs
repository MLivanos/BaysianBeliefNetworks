using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOExit : MonoBehaviour
{
    [SerializeField] private Vector3 shakeVector;
    [SerializeField] private Vector3 initialSize;
    [SerializeField] private Vector3 endSize;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float ufoSpeed;
    [SerializeField] private float travelTime;
    [SerializeField] private float cameraShakeStrength;
    [SerializeField] private float cameraShakeTime;

    private void Start()
    {
        StartCoroutine(PlayUFO());
    }

    private IEnumerator PlayUFO()
    {
        StartCoroutine(FlyUFO());
        yield return SizeUFO();
        yield return ShakeCamera();
        StopAllCoroutines();
        Destroy(gameObject);
    }

    private IEnumerator SizeUFO()
    {
        float timer = 0f;
        while(timer < travelTime)
        {
            transform.localScale = Vector3.Lerp(initialSize, endSize, timer/travelTime);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = endSize;
    }

    private IEnumerator ShakeCamera()
    {
        float timer = 0f;
        Vector3 originalPosition = cameraTransform.localPosition;
        
        while (timer < cameraShakeTime)
        {
            float dampingFactor = Mathf.Exp(-3f * timer / cameraShakeTime);
            float oscillation = Mathf.Sin(timer * Mathf.PI * 10f);
            float shakeOffset = dampingFactor * oscillation * cameraShakeStrength;

            cameraTransform.localPosition = originalPosition + shakeOffset*shakeVector;

            timer += Time.deltaTime;
            yield return null;
        }
        cameraTransform.localPosition = originalPosition;
    }

    private IEnumerator FlyUFO()
    {
        while(true)
        {
            transform.Translate(Vector3.right*ufoSpeed*Time.deltaTime, Space.Self);
            yield return null;
        }
    }
}
