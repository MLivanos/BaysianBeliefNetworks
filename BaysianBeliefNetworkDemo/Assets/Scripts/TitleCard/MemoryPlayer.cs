using UnityEngine;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class MemoryPlayer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Video Settings")]
    [SerializeField] private VideoClip videoClip;
    [SerializeField] private float fadeInTime = 0.5f;
    [SerializeField] private float fadeOutTime = 0.5f;
    [SerializeField] private float pauseAfterEnd = 0.5f;

    [Header("Render Texture Settings")]
    // "Video meta-data? Never heard of her" - Unity 2022
    [SerializeField] private int renderTextureWidth = 1280;
    [SerializeField] private int renderTextureHeight = 720;

    [Header("Display Component")]
    [SerializeField] private FadableImage videoRenderer;

    [Header("MemoryBorder")]
    [SerializeField] private FadableImage memoryBorder;

    private VideoPlayer videoPlayer;
    private RenderTexture renderTexture;
    private Coroutine currentCoroutine;
    private bool isPointerOver = false;

    private void Awake()
    {
        // Create the RenderTexture
        renderTexture = new RenderTexture(renderTextureWidth, renderTextureHeight, 0);
        renderTexture.Create();

        // Dynamically add a VideoPlayer component
        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.waitForFirstFrame = true;
        videoPlayer.skipOnDrop = true;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;

        // If videoRenderer isn't assigned via the Inspector, try to find it on children.
        if (videoRenderer == null)
        {
            videoRenderer = GetComponentInChildren<FadableImage>();
        }
        // Ensure the videoRenderer is displaying our RenderTexture.
        if (videoRenderer != null)
        {
            videoRenderer.GetComponent<RawImage>().texture = renderTexture;
        }
        memoryBorder.SetAlpha(0f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        PlayClip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
        StopClip();
    }

    private void PlayClip()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(FadeInAndPlay());
    }

    private void StopClip()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(FadeOutAndStop());
    }

    private IEnumerator FadeInAndPlay()
    {
        yield return SetFirstFrame();
        videoRenderer.FadeIn(fadeInTime);
        memoryBorder.FadeIn(fadeInTime, 0.7f);
        videoPlayer.Play();

        // Wait until the video finishes playing
        while (videoPlayer.isPlaying)
        {
            yield return null;
        }

        // Pause a moment on the final frame
        yield return new WaitForSeconds(pauseAfterEnd);

        if (!isPointerOver)
            yield return FadeOutAndStop();
    }

    private IEnumerator SetFirstFrame()
    {
        videoRenderer.SetAlpha(0);
        videoPlayer.clip = videoClip;
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
            yield return null;
        videoPlayer.time = 0;
        videoPlayer.Play();
        videoPlayer.Pause();
        yield return null;
    }

    private IEnumerator FadeOutAndStop()
    {
        videoRenderer.FadeOut(fadeOutTime);
        memoryBorder.FadeOut(fadeInTime, 0.7f);
        yield return new WaitForSeconds(fadeOutTime);
        videoPlayer.Stop();
    }
}
