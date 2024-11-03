using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] walkers;
    public float zVariation = 1.0f;
    public float xVariation = 1.0f;
    public float minTimeBetweenWalker = 4.0f;
    public float maxTimeBetweenWalker = 8.0f;
    public float minTimeBetweenWalkerFirst = 4.0f;
    public float maxTimeBetweenWalkerFirst = 8.0f;
    private bool hasRecentlyGenerated;
    private bool firstWalker = true;

    private void Update()
    {
        if (!hasRecentlyGenerated)
        {
            StartCoroutine(GenerateWalker());
        }
    }

    private IEnumerator GenerateWalker()
    {
        hasRecentlyGenerated = true;
        float timeToWait = Random.Range(minTimeBetweenWalker,maxTimeBetweenWalker);
        if (firstWalker)
        {
            timeToWait = Random.Range(minTimeBetweenWalkerFirst,maxTimeBetweenWalkerFirst);
            firstWalker = false;
        }
        yield return new WaitForSeconds(timeToWait);
        GameObject walker = Instantiate(walkers[Random.Range(0,walkers.Length)]);
        Vector3 noise = new Vector3(Random.Range(-xVariation,xVariation),0.0f,Random.Range(-zVariation,zVariation));
        walker.transform.position = transform.position + noise;
        walker.transform.eulerAngles = transform.eulerAngles;
        hasRecentlyGenerated = false;
    }
}
