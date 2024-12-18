using UnityEngine;

public class PlanetaryBehavior : MonoBehaviour
{
    [SerializeField] private float rotationalSpeed = 10f;
    [SerializeField] private float orbitalSpeed = 5f;
    [SerializeField] private Transform focalPoint;
    [SerializeField] private float orbitRadius = 10f;
    private float orbitalAngle;
    private bool isStopped;

    void Start()
    {
        if (focalPoint != null)
        {
            // Calculate the initial orbital angle based on the starting position relative to the focal point
            Vector3 offset = transform.position - focalPoint.position;
            orbitalAngle = Mathf.Atan2(offset.z, offset.x);
        }
    }

    void Update()
    {
        if (isStopped) return;
        // Rotate around the planet's own axis
        transform.Rotate(Vector3.up, rotationalSpeed * Time.deltaTime);

        if (focalPoint != null)
        {
            // Increment the angle for orbital movement
            orbitalAngle += orbitalSpeed * Time.deltaTime;

            // Calculate the offset from the focal point
            Vector3 orbitOffset = new Vector3(
                Mathf.Cos(orbitalAngle) * orbitRadius,
                0,
                Mathf.Sin(orbitalAngle) * orbitRadius
            );

            // Update the position based on the focal point's position and the offset
            transform.position = focalPoint.position + orbitOffset;
        }
    }

    public void Stop()
    {
        isStopped = true;
    }
}