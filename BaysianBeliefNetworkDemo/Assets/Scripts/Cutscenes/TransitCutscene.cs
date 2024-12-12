using System.Collections;
using UnityEngine;

public class TransitCutscene : CutsceneBehavior
{
    [SerializeField] private FadableImage fadeOut;
    [SerializeField] private GameObject meshGeneratorPrefab;
    [SerializeField] private Transform train;
    [SerializeField] private Transform cloudRing;
    [SerializeField] private float trainSpeed;
    [SerializeField] private float cloudRotationSpeed;
    [SerializeField] private float chunkWidth;
    [SerializeField] private float timeInTunnel;
    [SerializeField] private float brightenTime;

	protected override IEnumerator PlayScene()
    {
        StartCoroutine(TransitionFade());
        StartCoroutine(ExitTunnel());
    	cameraTransform.parent = train;
        
        yield return ViewPanel();
        AnimateText();
        GameObject[] meshGenerators = InitializeMeshGenerators();
        float distanceTraveled = 0f;
        while (true)
        {
            MoveTrainAndRotateClouds(ref distanceTraveled);
            if (distanceTraveled >= chunkWidth)
            {
                UpdateMeshGenerators(ref meshGenerators, ref distanceTraveled);
            }
            yield return null;
        }
    }

    public override void Interrupt()
    {
    	return;
    }

    protected override IEnumerator ExitTransition()
    {
    	yield return null;
    }

    private GameObject[] InitializeMeshGenerators()
    {
        GameObject[] meshGenerators = new GameObject[3];

        // Create the middle generator
        meshGenerators[1] = Instantiate(meshGeneratorPrefab, scene.transform);

        // Calculate positions for trailing and leading generators
        Vector3 middlePosition = meshGenerators[1].transform.position;
        Vector3 trailingPosition = middlePosition + new Vector3(chunkWidth-0.5f, 0f, 0f);
        Vector3 leadingPosition = middlePosition - new Vector3(chunkWidth-0.5f, 0f, 0f);

        // Create the trailing and leading generators
        meshGenerators[0] = Instantiate(meshGeneratorPrefab, trailingPosition, meshGenerators[1].transform.rotation, scene.transform);
        meshGenerators[2] = Instantiate(meshGeneratorPrefab, leadingPosition, meshGenerators[1].transform.rotation, scene.transform);

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
        Vector3 newMeshPosition = meshGenerators[1].transform.position - new Vector3(chunkWidth-0.5f, 0f, 0f);
        meshGenerators[2] = Instantiate(meshGeneratorPrefab, newMeshPosition, meshGenerators[1].transform.rotation);

        // Adjust the traveled distance
        distanceTraveled -= chunkWidth;
    }

    private IEnumerator ExitTunnel()
    {
        float timer = 0f;
        while(timer < timeInTunnel)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0f;
        while(timer < brightenTime)
        {
            RenderSettings.ambientIntensity = Mathf.Lerp(ambientIntensity, 8f, timer/brightenTime);
            timer += Time.deltaTime;
            yield return null;
        }
        RenderSettings.ambientIntensity = 8f;
    }

    private IEnumerator TransitionFade()
    {
        fadeOut.FadeOut(1f);
        yield return null;
    }
}