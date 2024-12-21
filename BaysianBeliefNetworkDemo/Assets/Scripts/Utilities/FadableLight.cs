using System.Collections;
using UnityEngine;

public class FadableLight : FadableElement
{
	private Light light;
	private bool interuptFlag = false;

	private void Start()
	{
		light = GetComponent<Light>();
	}

	public override void SetAlpha(float alpha)
	{
		light.intensity = alpha;
	}

	public IEnumerator BlinkForever(float blinkFrequency, bool start)
	{
		interuptFlag = false;
		while(true)
		{
			if(interuptFlag) break;
			yield return Blink(blinkFrequency, start);
		}
		interuptFlag = false;
	}

	public IEnumerator BlinkKTimes(float blinkFrequency, int k, bool start)
	{
		for(int i=0; i<k; i++)
		{
			yield return Blink(blinkFrequency, start);
		}
	}

	public IEnumerator Blink(float blinkFrequency, bool start)
	{
		yield return Fade(1f/((blinkFrequency+0.001f)/2f), start);
		yield return Fade(1f/((blinkFrequency+0.001f)/2f), !start);
	}
}