using System.Collections;
using UnityEngine;

public class FadableGameObject : FadableElement
{
	private Material material;

	private void Start()
	{
		material = GetComponent<Renderer>().material;
	}

	public override void SetAlpha(float alpha)
	{
		Color currentColor = material.color;
		currentColor.a = alpha;
		material.color = currentColor;
	}
}