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

    [SerializeField] private GameObject transitScene;
    [SerializeField] private GameObject meshGeneratorPrefab;
    [SerializeField] private Transform train;
    [SerializeField] private Transform cloudRing;
    [SerializeField] private float trainSpeed;
    [SerializeField] private float cloudRotationSpeed;

    private void Start()
    {
        StartCoroutine(PlayTransitScene());
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

    private IEnumerator PlayTransitScene()
    {
        mainCamera.parent = train;
        GameObject[] meshGenerators = InitializeMeshGenerators();

        float distanceTraveled = 0f;

        while (true)
        {
            MoveTrainAndRotateClouds(ref distanceTraveled);
            if (distanceTraveled >= 50f)
            {
                UpdateMeshGenerators(ref meshGenerators, ref distanceTraveled);
            }

            yield return null;
        }
    }

    private GameObject[] InitializeMeshGenerators()
    {
        GameObject[] meshGenerators = new GameObject[3];

        // Create the middle generator
        meshGenerators[1] = Instantiate(meshGeneratorPrefab, transitScene.transform);

        // Calculate positions for trailing and leading generators
        Vector3 middlePosition = meshGenerators[1].transform.position;
        Vector3 trailingPosition = middlePosition + new Vector3(49.5f, 0f, 0f);
        Vector3 leadingPosition = middlePosition - new Vector3(49.5f, 0f, 0f);

        // Create the trailing and leading generators
        meshGenerators[0] = Instantiate(meshGeneratorPrefab, trailingPosition, meshGenerators[1].transform.rotation, transitScene.transform);
        meshGenerators[2] = Instantiate(meshGeneratorPrefab, leadingPosition, meshGenerators[1].transform.rotation, transitScene.transform);

        return meshGenerators;
    }

    private void MoveTrainAndRotateClouds(ref float distanceTraveled)
    {
        train.Translate(Vector3.back * trainSpeed * Time.deltaTime);
        cloudRing.Rotate(Vector3.up * cloudRotationSpeed * Time.deltaTime);
        distanceTraveled += trainSpeed * Time.deltaTime;
    }

    private void UpdateMeshGenerators(ref GameObject[] meshGenerators, ref float distanceTraveled)
    {
        // Destroy the trailing mesh generator
        Destroy(meshGenerators[0]);

        // Shift references down the array
        meshGenerators[0] = meshGenerators[1];
        meshGenerators[1] = meshGenerators[2];

        // Create a new leading mesh generator
        Vector3 newMeshPosition = meshGenerators[1].transform.position - new Vector3(49.5f, 0f, 0f);
        meshGenerators[2] = Instantiate(meshGeneratorPrefab, newMeshPosition, meshGenerators[1].transform.rotation);

        // Adjust the traveled distance
        distanceTraveled -= 50f;
    }

}