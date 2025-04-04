using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GraphTextureBehavior : MonoBehaviour
{
    [SerializeField] private Image graphImage;
    [SerializeField] private Image graphBackground;
    [SerializeField] private Vector3 visiblePosition;
    [SerializeField] private Vector3 hiddenPosition;
    [SerializeField] private float slideDuration = 0.5f;

    private RectTransform rectTransform;
    private Coroutine currentSlide;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (graphImage == null)
            graphImage = GetComponent<Image>();
    }

    private void Start()
    {
        TryApplyGraphTexture();
    }

    public void TryApplyGraphTexture()
    {
        if (GraphSnapshotter.CapturedTexture == null)
        {
            Debug.LogWarning("[GraphTextureBehavior] No captured graph texture found.");
            return;
        }

        Texture2D tex = GraphSnapshotter.CapturedTexture;
        graphImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        graphImage.preserveAspect = false;
        ResizePicture(graphImage, tex);
        ResizePicture(graphBackground, tex);
    }

    private void ResizePicture(Image img, Texture2D tex)
    {
        RectTransform rt = img.rectTransform;
        float currentArea = rt.sizeDelta.x * rt.sizeDelta.y;

        float textureRatio = (float)tex.width / tex.height;

        float newWidth = Mathf.Sqrt(currentArea * textureRatio);
        float newHeight = Mathf.Sqrt(currentArea / textureRatio);

        rt.sizeDelta = new Vector2(newWidth, newHeight);
    }


    public void SlideIn()
    {
        StartSlide(visiblePosition);
    }

    public void SlideOut()
    {
        StartSlide(hiddenPosition);
    }

    private void StartSlide(Vector3 target)
    {
        if (currentSlide != null) StopCoroutine(currentSlide);
        currentSlide = StartCoroutine(SlideTo(target));
    }

    private IEnumerator SlideTo(Vector3 target)
    {
        Vector3 start = rectTransform.anchoredPosition;
        float timer = 0f;

        while (timer < slideDuration)
        {
            float t = timer / slideDuration;
            rectTransform.anchoredPosition = Vector3.Lerp(start, target, t);
            timer += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = target;
    }
}
