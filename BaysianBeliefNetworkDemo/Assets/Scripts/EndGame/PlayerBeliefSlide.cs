using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerBeliefSlide : EndGameSlide
{
	[SerializeField] private List<FadableElement> magnifyingEffect;
	[SerializeField] private List<SlideInBehavior> pages;
	[SerializeField] private List<RectTransform> highlighters;
	[SerializeField] private float timeBetweenPages;
	[SerializeField] private float magnificationTime;
	[SerializeField] private float beforeMagnificationTime;
	[SerializeField] private float highLightTime;
	private List<float> highlighterWidth = new List<float>();

	private void Start()
	{
		StartCoroutine(PlayScene());
	}

	protected override IEnumerator PlayScene()
	{
		HideMagnification();
		HideHighlighters();
		yield return SlideOutPages();
		yield return new WaitForSeconds(beforeMagnificationTime);
		yield return Magnify();
		yield return HighlightWords();
	}

	protected override IEnumerator ExitTransition()
	{
		foreach(SlideInBehavior page in pages)
		{
			page.BeginSlideOut();
		}
		yield return null;
	}

	private void HideMagnification()
	{
		foreach(FadableElement effect in magnifyingEffect)
		{
			effect.SetAlpha(0f);
		}
	}

	private void HideHighlighters()
	{
		for(int i=0; i<highlighters.Count; i++)
		{
		    Vector2 size = highlighters[i].sizeDelta;
		    highlighterWidth.Add(size.x);
		    size.x = 0f;
		    highlighters[i].sizeDelta = size;
		}
	}

	private IEnumerator SlideOutPages()
	{
		foreach(SlideInBehavior page in pages)
		{
			page.BeginSlideIn();
			yield return new WaitForSeconds(timeBetweenPages);
		}
	}

	private IEnumerator Magnify()
	{
		foreach(FadableElement effect in magnifyingEffect)
		{
			effect.FadeIn(magnificationTime);
		}
		yield return null;
	}

	private IEnumerator HighlightWords()
	{
		for(int i=0; i<highlighters.Count; i++)
		{
			yield return HighLight(highlighters[i], highlighterWidth[i]);
		}
	}

	private IEnumerator HighLight(RectTransform highlighter, float targetWidth)
	{
	    float timer = 0f;
	    Vector2 initialSize = highlighter.sizeDelta;

	    while (timer < highLightTime)
	    {
	        float newWidth = targetWidth * timer / highLightTime;
	        highlighter.sizeDelta = new Vector2(newWidth, initialSize.y);
	        timer += Time.deltaTime;
	        yield return null;
	    }
	    highlighter.sizeDelta = new Vector2(targetWidth, initialSize.y);
	}
}