using System.Collections;
using UnityEngine;

public class DropdownList : MonoBehaviour
{
    [SerializeField] private Transform panelTransform;
    [SerializeField] private Vector3 endPosition;
    [SerializeField] private float animationSpeed = 5f;

    private Vector3 startPosition;
    private Coroutine currentCoroutine;

    private void Start()
    {
        startPosition = panelTransform.localPosition;
    }

    public void MoveUp()
    {
        StartMovePanelCoroutine(false);
    }

    public void MoveDown()
    {
        Debug.Log("movingDown");
        StartMovePanelCoroutine(true);
    }

    private void StartMovePanelCoroutine(bool movingDown)
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(MovePanel(movingDown));
    }

    private IEnumerator MovePanel(bool movingDown)
    {
        Vector3 initialPosition = panelTransform.localPosition;
        Vector3 targetPosition = movingDown ? endPosition : startPosition;

        float totalDistance = Vector3.Distance(initialPosition, targetPosition);
        float totalTime = totalDistance / animationSpeed;

        Debug.Log(initialPosition);
        Debug.Log(targetPosition);
        Debug.Log(totalTime);

        float timer = 0f;
        while (timer < totalTime)
        {
            Debug.Log(targetPosition);
            panelTransform.localPosition = Vector3.Lerp(initialPosition, targetPosition, timer / totalTime);
            timer += Time.deltaTime;
            yield return null;
        }

        panelTransform.localPosition = targetPosition;
        currentCoroutine = null;
    }

    private IEnumerator PeepDropdown()
    {
        MoveDown();
        yield return new WaitForSeconds(3f);
        MoveUp();
    }

    public void Peep()
    {
        StartCoroutine(PeepDropdown());
    }
}
