using System.Collections;
using UnityEngine;

public class ScaleOnHover : MonoBehaviour, IXRaycastTargetScript
{
    [SerializeField] private Transform transformToScale;
    [SerializeField] private Vector2 hoverScale = new Vector2(1.05f, 1.05f);
    [SerializeField] private float scaleSpeed = 8f;

    private Vector3 originalScale;
    private Coroutine scalingCoroutine;

    private void Awake()
    {
        if (transformToScale == null) transformToScale = transform;
        originalScale = transformToScale.localScale;
        hoverScale *= originalScale;
    }

    public void OnDeepPointerEnter()
    {
        StartScaleTween(hoverScale);
    }

    public void OnDeepPointerExit()
    {
        StartScaleTween(new Vector2(originalScale.x, originalScale.y));
    }

    private void StartScaleTween(Vector2 target)
    {
        if (!isActiveAndEnabled) return;
        if (scalingCoroutine != null)
            StopCoroutine(scalingCoroutine);
        scalingCoroutine = StartCoroutine(ScaleTo(new Vector3(target.x, target.y, originalScale.z)));
    }

    private IEnumerator ScaleTo(Vector3 targetScale)
    {
        Vector3 start = transformToScale.localScale;
        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime * scaleSpeed;
            transformToScale.localScale = Vector3.Lerp(start, targetScale, progress);
            yield return null;
        }

        transformToScale.localScale = targetScale;
        scalingCoroutine = null;
    }
}