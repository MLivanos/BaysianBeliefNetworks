using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningCutsceneManager : MonoBehaviour
{
    [SerializeField] private SlideInBehavior[] photoSlides;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform[] cameraEnds;
    [SerializeField] private float cameraMoveDuration;
    [SerializeField] private SlideInBehavior busSlideIn;
    [SerializeField] private Transform[] busWheels;
    [SerializeField] private float wheelSpeed;
    [SerializeField] private Transform[] busDoors;
    [SerializeField] private float doorOpenSpeed;

    private void Start()
    {
        StartCoroutine(PlayGoodbyeScene());
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

    private IEnumerator PlayGoodbyeScene()
    {
        busSlideIn.BeginSlideIn();
        float timer = 0f;
        while(timer < busSlideIn.GetDuration())
        {
            foreach(Transform wheel in busWheels)
            {
                wheel.Rotate(Vector3.right * wheelSpeed * Time.deltaTime);
            }
            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.75f);
        while(busDoors[0].localEulerAngles.y < 90f)
        {
            busDoors[0].Rotate(Vector3.up * doorOpenSpeed * Time.deltaTime);
            busDoors[1].Rotate(-Vector3.up * doorOpenSpeed * Time.deltaTime);
            yield return null;
        }
    }
}