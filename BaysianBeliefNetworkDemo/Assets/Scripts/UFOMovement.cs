using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOMovement : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;

    private void Update()
    {
        transform.Translate(Vector3.forward*Time.deltaTime*speed,Space.World);
        transform.Rotate(Vector3.up*Time.deltaTime*rotationSpeed,Space.World);
        if (transform.position.z > 100)
        {
            Destroy(gameObject);
        }
    }
}
