using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    [SerializeField] private List<float> timers;
    [SerializeField] private Vector2 timeBetweenClaps;
    [SerializeField] private int numberOfThunderClips;
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        foreach(float time in timers)
        {
            StartCoroutine(ThunderAfterTime(time));
        }
    }

    private IEnumerator ThunderAfterTime(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        string thunderId = "Thunder" + Random.Range(1, numberOfThunderClips+1).ToString();
        audioManager.PlayEffect(thunderId);
        StartCoroutine(ThunderAfterTime(Random.Range(timeBetweenClaps.x, timeBetweenClaps.y)));
    }
}
