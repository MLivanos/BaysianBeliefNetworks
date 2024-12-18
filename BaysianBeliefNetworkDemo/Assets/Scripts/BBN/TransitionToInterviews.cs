using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionToInterviews : Transition
{
	[SerializeField] private GameObject background;
	[SerializeField] private RawImage whiteToDot;
	[SerializeField] private GameObject circleToDot;
	[SerializeField] private float minHeight;
	[SerializeField] private float timeBetweenWhiteAndDot;
	[SerializeField] private float whiteToLineTime;
	[SerializeField] private float lineToDotTime;
	private float maxHeight;
	private float maxWidth;

	protected override IEnumerator TransitionToScene()
    {
    	maxHeight = whiteToDot.rectTransform.sizeDelta.y;
    	maxWidth = whiteToDot.rectTransform.sizeDelta.x;
    	ShowObjects();
    	yield return BringWhiteToLine();
    	yield return new WaitForSeconds(timeBetweenWhiteAndDot);
    	yield return BringLineToDot();
    	sceneManager.GoToInterviews();
    }

    private void ShowObjects()
    {
    	background.SetActive(true);
    	whiteToDot.gameObject.SetActive(true);
    	circleToDot.SetActive(true);
    }

    private IEnumerator BringWhiteToLine()
    {
    	float timer = 0f;
    	float height = 0f;
    	while(timer < whiteToLineTime)
    	{
    		height = (maxHeight - minHeight) * (1f-timer / whiteToLineTime) + minHeight;
    		whiteToDot.rectTransform.sizeDelta = new Vector2(maxWidth, height);
    		timer += Time.deltaTime;
    		yield return null;
    	}
    }

    private IEnumerator BringLineToDot()
    {
    	float timer = 0f;
    	float width = 0f;
    	while(timer < lineToDotTime)
    	{
    		circleToDot.transform.localScale = Vector3.one*(1f-timer / lineToDotTime);
    		width = maxWidth * (1f-timer / lineToDotTime);
    		whiteToDot.rectTransform.sizeDelta = new Vector2(width, minHeight);
    		timer += Time.deltaTime;
    		yield return null;
    	}
    }
}