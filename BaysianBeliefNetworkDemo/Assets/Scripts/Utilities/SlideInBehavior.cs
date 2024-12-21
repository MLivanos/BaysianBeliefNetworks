using System.Collections;
using UnityEngine;

public interface IPositionable
{
    Vector3 Position { get; set; }
}

public class RectTransformPositionable : IPositionable
{
    private readonly RectTransform rectTransform;

    public RectTransformPositionable(RectTransform rectTransform)
    {
        this.rectTransform = rectTransform;
    }

    public Vector3 Position
    {
        get => rectTransform.anchoredPosition3D;
        set => rectTransform.anchoredPosition3D = value;
    }
}

public class TransformPositionable : IPositionable
{
    private readonly Transform transform;

    public TransformPositionable(Transform transform)
    {
        this.transform = transform;
    }

    public Vector3 Position
    {
        get => transform.localPosition;
        set => transform.localPosition = value;
    }
}

public class SlideInBehavior : MonoBehaviour
{
    [SerializeField] private Transform objectTransform;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 endPosition;
    [SerializeField] private float eventDuration;
    [SerializeField] private bool staticX;
    [SerializeField] private bool staticY;
    [SerializeField] private bool staticZ;

    private IPositionable positionable;

    private void Start()
    {
        if (objectTransform == null) objectTransform = GetComponent<Transform>();

        if (objectTransform is RectTransform rectTransform)
        {
            positionable = new RectTransformPositionable(rectTransform);
        }
        else
        {
            positionable = new TransformPositionable(objectTransform);
        }

        startPosition = SetStaticAxes(startPosition);
        endPosition = SetStaticAxes(endPosition);
    }

    private Vector3 SetStaticAxes(Vector3 position)
    {
        Vector3 currentPos = positionable.Position;
        return new Vector3(
            staticX ? currentPos.x : position.x,
            staticY ? currentPos.y : position.y,
            staticZ ? currentPos.z : position.z
        );
    }

    public void BeginSlideIn()
    {
        StartCoroutine(Slide(true));
    }

    public void BeginSlideOut()
    {
        StartCoroutine(Slide(false));
    }

    public IEnumerator Slide(bool slideIn)
    {
        float timer = 0f;
        Vector3 start = slideIn ? startPosition : endPosition;
        Vector3 end = slideIn ? endPosition : startPosition;
        while (timer < eventDuration)
        {
            positionable.Position = Vector3.Lerp(start, end, timer / eventDuration);

            timer += Time.deltaTime;
            yield return null;
        }

        positionable.Position = end;
    }

    public void SetAtTerminalPoint(bool begining)
    {
        positionable.Position = begining ? startPosition : endPosition;
    }

    public float GetDuration()
    {
        return eventDuration;
    }
}