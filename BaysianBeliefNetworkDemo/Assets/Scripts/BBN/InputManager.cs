using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private float maxX;
    [SerializeField] private float maxY;
    [SerializeField] private float panSpeed = 2f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZ = -25f;
    [SerializeField] private float maxZ = -5f;
    private Vector3 lastMousePosition;

    private void Update()
    {
        ZoomCamera();
        PanCamera();
    }

    private void PanCamera()
    {
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 delta = (Input.mousePosition - lastMousePosition);
            delta += panSpeed*(Vector3.left*delta.x+Vector3.down*delta.y);
            Vector3 newPosition = cameraObject.transform.position + delta;
            newPosition.x = Mathf.Clamp(newPosition.x, -maxX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, -maxY, maxY);
            cameraObject.transform.position = newPosition;
            lastMousePosition = Input.mousePosition;
        }
    }

    private void ZoomCamera()
    {
        float delta = Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime;
        Vector3 newPosition = cameraObject.transform.position + Vector3.forward*delta;
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);
        cameraObject.transform.position = newPosition;
    }

}