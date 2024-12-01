using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogController : MonoBehaviour
{
    public bool barking;
    public float timeBetweenBarks = 5.0f;
    private Animator controller;

    private void Start()
    {
        controller = GetComponent<Animator>();
        if (barking)
        {
            StartCoroutine(Bark());
        }
        else
        {
            controller.SetBool("Sleep_b", true);
        }
    }

    private void StopBarking()
    {
        controller.SetInteger("ActionType_int", 0);
    }

    private IEnumerator Bark()
    {
        int counter = 0;
        int howlInterval = Random.Range(2,5);
        while(true)
        {
            // Every few barks, howl
            int action = 1;
            if (counter == howlInterval)
            {
                action = 6;
                howlInterval = Random.Range(2,5);
                counter = 0;
            }
            controller.SetInteger("ActionType_int", action);
            yield return new WaitForSeconds(3.0f);
            StopBarking();
            yield return new WaitForSeconds(timeBetweenBarks);
            counter ++;
        }
    }
}
