using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using UnityEngine.UI;

public class VideoClipManager : MonoBehaviour
{
    public static VideoClipManager Instance;

    [Header("Video Settings")]
    [SerializeField] private VideoPlayer videoPlayer; // Reference to the VideoPlayer component
    [SerializeField] private CanvasGroup videoCanvasGroup; // Used for fade in/out
    [SerializeField] private float fadeInTime = 0.5f;  // Time for fade in
    [SerializeField] private float fadeOutTime = 0.5f; // Time for fade out
    [SerializeField] private float pauseAfterEnd = 0.5f; // Pause on final frame before fade out

    private Coroutine currentCoroutine;
    private bool isPointerOver = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Called by the photo trigger when pointer enters.
    /// Snaps the video player to the target and plays the clip.
    /// </summary>
    public void PlayClipAt(Transform target, VideoClip clip)
    {
        // Mark that the pointer is over a photo.
        isPointerOver = true;
        
        // Snap the video player to the target photo's position & rotation.
        transform.position = target.position;
        transform.rotation = target.rotation;

        // Assign the clip.
        videoPlayer.clip = clip;
        videoPlayer.Stop();

        // Stop any current coroutine before starting a new one.
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(FadeInAndPlay());
    }

    /// <summary>
    /// Called when pointer exits the photo.
    /// </summary>
    public void StopClip()
    {
        isPointerOver = false;
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(FadeOutAndStop());
    }

    private IEnumerator FadeInAndPlay()
    {
        // Start with the video canvas group fully transparent.
        videoCanvasGroup.alpha = 0;

        // Fade in.
        float t = 0f;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            videoCanvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeInTime);
            yield return null;
        }
        videoCanvasGroup.alpha = 1;

        // Play the video.
        videoPlayer.Play();

        // Wait for the video to finish.
        while (videoPlayer.isPlaying)
        {
            yield return null;
        }

        // Once finished, wait for a short pause.
        yield return new WaitForSeconds(pauseAfterEnd);

        // If the pointer is no longer over the photo, fade out.
        if (!isPointerOver)
            yield return StartCoroutine(FadeOutAndStop());
    }

    private IEnumerator FadeOutAndStop()
    {
        float t = 0f;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            videoCanvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeOutTime);
            yield return null;
        }
        videoCanvasGroup.alpha = 0;
        videoPlayer.Stop();
    }
}