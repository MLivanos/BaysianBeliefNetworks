using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveVehicle : MonoBehaviour
{
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float rotationalSpeed = 180f;
    [SerializeField] private Transform[] wheels;

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        float rotationAmount = rotationalSpeed * Time.deltaTime;

        foreach (Transform wheel in wheels)
        {
            wheel.Rotate(Vector3.right, rotationAmount);
        }
    }
}
