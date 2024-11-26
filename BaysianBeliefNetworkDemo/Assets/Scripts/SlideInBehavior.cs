using System.Collections;
using UnityEngine;

public class SlideInBehavior : MonoBehaviour
{
    [SerializeField] private RectTransform objectTransform;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 endPosition;
    [SerializeField] private float eventDuration;
    [SerializeField] private bool staticX;
    [SerializeField] private bool staticY;
    [SerializeField] private bool staticZ;

    private void Start()
    {
        if (objectTransform == null) objectTransform = GetComponent<RectTransform>();

        startPosition = SetStaticAxes(startPosition);
        endPosition = SetStaticAxes(endPosition);

        objectTransform.position = startPosition;
    }

    private Vector3 SetStaticAxes(Vector3 position)
    {
        return new Vector3(
            staticX ? objectTransform.anchoredPosition3D.x : position.x,
            staticY ? objectTransform.anchoredPosition3D.y : position.y,
            staticZ ? objectTransform.anchoredPosition3D.z : position.z
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

    private IEnumerator Slide(bool slideIn)
    {
        float timer = 0f;
        Vector3 start = slideIn ? startPosition : endPosition;
        Vector3 end = slideIn ? endPosition : startPosition;
        while (timer < eventDuration)
        {
            objectTransform.anchoredPosition3D = Vector3.Lerp(start, end, timer / eventDuration);

            timer += Time.deltaTime;
            yield return null;
        }

        objectTransform.anchoredPosition3D = end;
    }
}