using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TransitionToInterviews : Transition
{
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject reflection;
    [SerializeField] private RawImage whiteToDot;
    [SerializeField] private GameObject circleToDot;
    [SerializeField] private float minHeight = 2f;
    [SerializeField] private float timeBetweenWhiteAndDot = 0.1f;
    [SerializeField] private float whiteToLineTime = 0.25f;
    [SerializeField] private float lineToDotTime = 0.25f;

    float maxHeight, maxWidth;
    RectTransform rt;

    protected override IEnumerator TransitionToScene()
    {
        rt = whiteToDot.rectTransform;

        // Wait one frame so layout/CanvasScaler has correct sizes
        yield return null;
        maxHeight = rt.rect.height;
        maxWidth  = rt.rect.width;

        ShowObjects();

        yield return BringWhiteToLine();
        yield return new WaitForSeconds(timeBetweenWhiteAndDot);
        yield return BringLineToDot();

        sceneManager.GoToInterviews();
    }

    void ShowObjects()
    {
        background.SetActive(true);
        reflection.SetActive(true);
        whiteToDot.gameObject.SetActive(true);
        circleToDot.SetActive(true);
    }

    IEnumerator BringWhiteToLine()
    {
        float t = 0f;
        while (t < whiteToLineTime)
        {
            float height = Mathf.Lerp(maxHeight, minHeight, t / whiteToLineTime);

            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   height);

            t += Time.deltaTime;
            yield return null;
        }
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, minHeight);
    }

    IEnumerator BringLineToDot()
    {
        float t = 0f;
        while (t < lineToDotTime)
        {
            float k = t / lineToDotTime;

            circleToDot.transform.localScale = Vector3.one * (1f - k);
            float width = Mathf.Lerp(maxWidth, 0f, k);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   minHeight);

            t += Time.deltaTime;
            yield return null;
        }
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
    }
}
