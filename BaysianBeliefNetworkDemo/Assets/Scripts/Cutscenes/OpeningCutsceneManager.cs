using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningCutsceneManager : MonoBehaviour
{
    [SerializeField] private SlideInBehavior[] photoSlides;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform[] cameraEnds;
    [SerializeField] private float cameraMoveDuration;

    private void Start()
    {
        StartCoroutine(NightSkyOpener());
    }

    private IEnumerator NightSkyOpener()
    {
        yield return new WaitForSeconds(1f);
        float timer = 0f;
        while(timer < cameraMoveDuration)
        {
            mainCamera.position = Vector3.Lerp(cameraEnds[0].position, cameraEnds[1].position, timer/cameraMoveDuration);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator PlayPhotoSlideScene()
    {
        foreach(SlideInBehavior photo in photoSlides)
        {
            photo.BeginSlideIn();
            yield return new WaitForSeconds(photo.GetDuration());
        }
    }
}