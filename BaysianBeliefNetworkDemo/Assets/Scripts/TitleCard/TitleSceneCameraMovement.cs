using UnityEngine;

public class TitleSceneCameraMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private float maxLeftRight = 2f;
    [SerializeField] private float maxUpDown = 1f;
    [SerializeField] private float deadZoneSize = 0.2f;
    [SerializeField] private Transform cameraTransform;
    private bool disable;

    private Vector3 startPos;

    private void Start()
    {
        startPos = cameraTransform.position;
    }

    private void Update()
    {
        if (disable) return;
        
        Vector2 mousePos = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);

        float xNormalized = (mousePos.x - 0.5f) * 2; 
        float yNormalized = (mousePos.y - 0.5f) * 2;

        xNormalized = Mathf.Abs(xNormalized) > deadZoneSize ? xNormalized : 0f;
        yNormalized = Mathf.Abs(yNormalized) > deadZoneSize ? yNormalized : 0f;

        float xMove = Mathf.Clamp(xNormalized * maxLeftRight, -maxLeftRight, maxLeftRight);
        float yMove = Mathf.Clamp(yNormalized * maxUpDown, -maxUpDown, maxUpDown);

        Vector3 targetPos = startPos + new Vector3(xMove, yMove, 0);
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPos, Time.deltaTime * movementSpeed);
    }

    public void Disable()
    {
        disable = true;
    }
}
