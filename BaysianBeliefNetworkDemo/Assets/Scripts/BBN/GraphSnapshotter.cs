using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphSnapshotter : MonoBehaviour
{
    public static Texture2D CapturedTexture { get; private set; }

    [SerializeField] private Camera graphCamera;
    [SerializeField] private Vector3 snapshotCameraPosition = new Vector3(0, 0, -29);
    [SerializeField] private List<GameObject> elementsToHide;

    private static GraphSnapshotter instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator CaptureRoutine()
    {
        SetupShot(false);
        yield return null;
        Shoot();
        SetupShot(true);
    }

    private void SetupShot(bool objectsVisible)
    {
        foreach (var obj in elementsToHide)
            if (obj != null) obj.SetActive(objectsVisible);
        graphCamera.transform.position = snapshotCameraPosition;
    }

    private void Shoot()
    {
        RenderTexture rt = new RenderTexture(graphCamera.pixelWidth, graphCamera.pixelHeight, 24);
        graphCamera.targetTexture = rt;

        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        graphCamera.Render();
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        graphCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        CapturedTexture = tex;
    }
}