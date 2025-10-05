using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntervieweeSpawner : InterviewEventSystem
{
    [SerializeField] private GameObject[] intervieweePrefabs;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Transform lookAheadNode;
    [SerializeField] private RuntimeAnimatorController animator;
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;

    private GameObject currentInterviewee;
    private Animator intervieweeAnimator;
    private int currentWaypointIndex = 0;
    private float proximityCriterion = 0.05f;

    private Queue<int> _bag;
    private int _lastIdx = -1;

    public void SpawnInterviewee()
    {
        if (intervieweePrefabs == null || intervieweePrefabs.Length == 0)
        {
            Debug.LogWarning("IntervieweeSpawner: No prefabs assigned.");
            return;
        }

        if (_bag == null || _bag.Count == 0)
            RefillBag();

        int idx = _bag.Dequeue();

        currentInterviewee = Instantiate(intervieweePrefabs[idx], transform.position, transform.rotation);

        var anim = currentInterviewee.GetComponent<Animator>();
        if (anim != null)
        {
            anim.runtimeAnimatorController = animator; // no cast needed
            anim.applyRootMotion = false;
            intervieweeAnimator = anim;
        }

        _lastIdx = idx;
        StartCoroutine(SitAtBooth());
    }

    private void RefillBag()
    {
        int n = intervieweePrefabs.Length;

        if (n == 1)
        {
            _bag = new Queue<int>();
            _bag.Enqueue(0);
            return;
        }

        List<int> list = new List<int>(n);
        for (int i = 0; i < n; i++) list.Add(i);
        FisherYates(list);

        if (_lastIdx >= 0 && list[0] == _lastIdx)
        {
            int swapWith = Random.Range(1, list.Count); // [1, n-1]
            (list[0], list[swapWith]) = (list[swapWith], list[0]);
        }

        _bag = new Queue<int>(list);
    }

    private static void FisherYates(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
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
        // Note: this waits until the *current* state matches. If you need to wait for it to finish,
        // also check normalizedTime >= 1.0f.
        while (!animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName))
            yield return null;
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

        if (IsAtTarget(currentInterviewee.transform.position, targetPosition))
            currentWaypointIndex += reverse ? -1 : 1;
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