using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class ConstantHorizontalFOV : MonoBehaviour
{
    [SerializeField] float referenceVerticalFOV = 60f;
    [SerializeField] float referenceAspect = 16f / 9f;

    void Awake()
    {
        Camera cam = GetComponent<Camera>();
        if (Mathf.Abs(cam.aspect - referenceAspect) < 0.001f) return;
        float hFOV = 2f * Mathf.Atan(Mathf.Tan(referenceVerticalFOV * Mathf.Deg2Rad / 2f) * referenceAspect);
        float vFOV = 2f * Mathf.Atan(Mathf.Tan(hFOV / 2f) / cam.aspect);
        cam.fieldOfView = vFOV * Mathf.Rad2Deg;
    }
}
