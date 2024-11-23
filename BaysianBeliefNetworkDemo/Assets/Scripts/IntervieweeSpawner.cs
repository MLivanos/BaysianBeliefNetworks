using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntervieweeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] intervieweePrefabs;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float speed;
    GameObject currentInterviewee;
    private int currentWaypointIndex = 0;
    private float proximityCriterion = 0.05f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SpawnInterviewee();
        }
    }

    public void SpawnInterviewee()
    {
        currentInterviewee = Instantiate(intervieweePrefabs[(int)Mathf.Round(Random.Range(0, waypoints.Length-0.51f))], transform.position, transform.rotation);
        StartCoroutine(MoveInterviewee());
    }

    private IEnumerator MoveInterviewee()
    {
        currentWaypointIndex = 0;
        while (currentWaypointIndex < waypoints.Length)
        {
            MoveStep();
            yield return null;
        }
    }

    private void MoveStep()
    {
        Vector3 currentPosition = currentInterviewee.transform.position;
        Vector3 targetPosition = waypoints[currentWaypointIndex].position;
        currentInterviewee.transform.position = Vector3.MoveTowards(currentPosition, targetPosition, speed * Time.deltaTime);
        if (Vector3.Distance(currentInterviewee.transform.position,targetPosition) < proximityCriterion)
        {
            currentWaypointIndex++;
        }
    }
}