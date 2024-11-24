using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntervieweeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] intervieweePrefabs;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Transform finalWaypoint;
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    GameObject currentInterviewee;
    private int currentWaypointIndex = 0;
    private float proximityCriterion = 0.05f;

    private void Start()
    {
        SpawnInterviewee();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SpawnInterviewee();
        }
    }

    public void SpawnInterviewee()
    {
        currentInterviewee = Instantiate(intervieweePrefabs[(int)Mathf.Round(Random.Range(0, intervieweePrefabs.Length-0.51f))], transform.position, transform.rotation);
        StartCoroutine(SetupInterviewee());
    }

    private IEnumerator SetupInterviewee()
    {
        currentWaypointIndex = 0;
        yield return StartCoroutine(MoveInterviewee(false));
        yield return StartCoroutine(RotateToView());
        currentWaypointIndex -= 2;
        yield return StartCoroutine(MoveInterviewee(true));
    }

    private IEnumerator MoveInterviewee(bool reverse)
    {
        while (currentWaypointIndex < waypoints.Length && currentWaypointIndex >= 0)
        {
            MoveStep(reverse);
            yield return null;
        }
    }

    private IEnumerator RotateToView()
    {
        Vector3 direction = finalWaypoint.position - currentInterviewee.transform.position;
        while (Quaternion.Angle(currentInterviewee.transform.rotation, Quaternion.LookRotation(direction)) > proximityCriterion)
        {
            RotateCharacter(direction);
            yield return null;
        }
    }

    private void MoveStep(bool reverse)
    {
        Vector3 currentPosition = currentInterviewee.transform.position;
        Vector3 targetPosition = waypoints[currentWaypointIndex].position;
        Vector3 direction = (targetPosition - currentInterviewee.transform.position).normalized;
        currentInterviewee.transform.position = Vector3.MoveTowards(
            currentPosition, 
            targetPosition, 
            speed * Time.deltaTime
        );
        RotateCharacter(direction);
        if (Vector3.Distance(currentInterviewee.transform.position,targetPosition) < proximityCriterion)
        {
            currentWaypointIndex += reverse ? -1 : 1;
        }
    }

    private void RotateCharacter(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            currentInterviewee.transform.rotation = Quaternion.RotateTowards(
                currentInterviewee.transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
        }
    }

    void OnDrawGizmos()
    {
        if (waypoints == null) return;

        for (int i = 0; i < waypoints.Length; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(waypoints[i].position, 0.2f);

            if (i < waypoints.Length - 1)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }

        if (finalWaypoint == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(finalWaypoint.position, 0.2f);
    }
}