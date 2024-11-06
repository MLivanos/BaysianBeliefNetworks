using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnimationController : MonoBehaviour
{
    private Animator controller;
    bool isWaiting;
    [SerializeField] int maxAnimations = 4;
    private void Start()
    {
        controller = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isWaiting)
        {
            StartCoroutine(WaitInIdle(1.5f,3.5f));
        }
    }

    private IEnumerator WaitInIdle(float minWaitTime, float maxWeightTime)
    {
        isWaiting = true;
        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWeightTime));
        isWaiting = false;
        controller.SetInteger("RandomInt", Random.Range(0,maxAnimations));
    }
}
