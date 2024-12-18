using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class IntervieweeSpawner : InterviewEventSystem
{
    [SerializeField] private GameObject[] intervieweePrefabs;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Transform lookAheadNode;
    [SerializeField] private AnimatorController animator;
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    private GameObject currentInterviewee;
    private Animator intervieweeAnimator;
    private int currentWaypointIndex = 0;
    private float proximityCriterion = 0.05f;

    public void SpawnInterviewee()
    {
        currentInterviewee = Instantiate(intervieweePrefabs[(int)Mathf.Round(Random.Range(0, intervieweePrefabs.Length-0.51f))], transform.position, transform.rotation);
        currentInterviewee.GetComponent<Animator>().runtimeAnimatorController = animator as RuntimeAnimatorController;
        currentInterviewee.GetComponent<Animator>().applyRootMotion = false;
        intervieweeAnimator = currentInterviewee.GetComponent<Animator>();
        StartCoroutine(SitAtBooth());
    }

    public void DespawnInterviewee()
    {
        StartCoroutine(LeaveBooth());
    }

    private IEnumerator SitAtBooth()
    {
        intervieweeAnimator.SetBool("isWalking", true);
        currentWaypointIndex = 0;
        yield return StartCoroutine(MoveInterviewee(false));
        intervieweeAnimator.SetBool("isWalking", false);

        yield return StartCoroutine(RotateToView(lookAheadNode.position));

        yield return WaitForAnimationState(intervieweeAnimator, "idleSit");
        intervieweeAnimator.SetBool("isSitting", true);
        interviewManager.Advance();
    }

    private IEnumerator LeaveBooth()
    {
        intervieweeAnimator.SetBool("isSitting", false);
        currentWaypointIndex -= 2;
        yield return WaitForAnimationState(intervieweeAnimator, "endSit");
        yield return StartCoroutine(RotateToView(waypoints[currentWaypointIndex].position));

        intervieweeAnimator.SetBool("isWalking", true);
        yield return WaitForAnimationState(intervieweeAnimator, "walking");
        yield return StartCoroutine(MoveInterviewee(true));
        Destroy(currentInterviewee);
        interviewManager.Advance();
    }

    private IEnumerator WaitForAnimationState(Animator animator, string stateName, int layer = 0)
    {
        while (!animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName))
        {
            yield return null;
        }
    }


    private IEnumerator MoveInterviewee(bool reverse)
    {
        while (currentWaypointIndex < waypoints.Length && currentWaypointIndex >= 0)
        {
            MoveStep(reverse);
            yield return null;
        }
    }

    private IEnumerator RotateToView(Vector3 position)
    {
        Vector3 direction = position - currentInterviewee.transform.position;
        while (Quaternion.Angle(currentInterviewee.transform.rotation, Quaternion.LookRotation(direction)) > proximityCriterion)
        {
            RotateCharacter(direction);
            yield return null;
        }
        currentInterviewee.transform.rotation = Quaternion.LookRotation(direction);
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
        if (IsAtTarget(currentInterviewee.transform.position,targetPosition))
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

        if (lookAheadNode == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(lookAheadNode.position, 0.2f);
    }

    private bool IsAtTarget(Vector3 currentPosition, Vector3 targetPosition)
    {
        float adjustedProximity = proximityCriterion + speed * Time.deltaTime;
        bool atTarget = Vector3.Distance(currentPosition, targetPosition) < adjustedProximity;
        if (atTarget) currentInterviewee.transform.position = targetPosition;
        return atTarget;
    }
}