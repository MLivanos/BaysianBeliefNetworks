using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomImpulse : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Vector2 strengthMinMax;
    [SerializeField] private List<Vector3> directions;
    [SerializeField] private Vector2 timeBetweenMinMax;
    [SerializeField] private bool normalize;

    private void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (normalize) NormalizeDirectionVectors();
        BlowObject();
    }

    private void NormalizeDirectionVectors()
    {
        List<Vector3> normalizedDirections = new List<Vector3>();
        foreach(Vector3 direction in directions)
        {
            normalizedDirections.Add(direction.normalized);
        }
        directions = normalizedDirections;
    }

    private void BlowObject()
    {
        Vector3 direction = GetRandomDirection();
        float force = Random.Range(strengthMinMax[0], strengthMinMax[1]);
        rb.AddForce(direction * force, ForceMode.Impulse);
        StartCoroutine(WaitAndBlow());
    }

    private Vector3 GetRandomDirection()
    {
        Vector3 randomDirection = Vector3.zero;
        foreach(Vector3 direction in directions)
        {
            randomDirection += (Random.Range(0f,1f) / directions.Count) * direction;
        }
        return randomDirection.normalized;
    }

    private IEnumerator WaitAndBlow()
    {
        yield return new WaitForSeconds(Random.Range(timeBetweenMinMax[0], timeBetweenMinMax[1]));
        BlowObject();
    }
}