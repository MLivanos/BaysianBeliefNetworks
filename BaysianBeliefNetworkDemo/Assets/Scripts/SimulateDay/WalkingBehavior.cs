using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingBehavior : MonoBehaviour
{
    public float minTurnAtX = -19.5f;
    public float maxTurnAtX = -16.5f;
    private float turnAtX;
    public float minSpeed = 0.35f;
    public float maxSpeed = 0.6f;
    private float speed;
    private bool turned;

    private void Start()
    {
        turnAtX = Random.Range(minTurnAtX, maxTurnAtX);
        speed = Random.Range(minSpeed, maxSpeed);
    }

    void Update()
    {
        transform.Translate(Vector3.forward*Time.deltaTime*speed);
        if (!turned && transform.position.x < turnAtX)
        {
            int direction;
            bool goLeft = Random.Range(0,2) == 1;
            direction = goLeft ? 1 : -1;
            transform.Rotate(Vector3.up*90*direction);
            turned = true;
        }
    }
}
