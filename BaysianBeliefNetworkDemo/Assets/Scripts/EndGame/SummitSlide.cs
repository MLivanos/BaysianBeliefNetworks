using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SummitSlide : EndGameSlide
{
	[SerializeField] private SlideInBehavior zoomIn;
	[SerializeField] private Transform paperView;
	[SerializeField] private Transform panelView;
	[SerializeField] private float waitBeforeZoomIn;
	[SerializeField] private float waitBeforeRotateUp;
	[SerializeField] private float rotationSpeed;

	private void Start()
	{
		cameraTransform = GameObject.Find("Main Camera").transform;
		StartCoroutine(PlayScene());
	}

	protected override IEnumerator PlayScene()
	{
		yield return new WaitForSeconds(waitBeforeZoomIn);
		yield return ZoomInToPaper();
		yield return new WaitForSeconds(waitBeforeRotateUp);
		yield return RotateUntilMatch(cameraTransform, panelView);

	}

	protected override IEnumerator ExitTransition()
	{
		yield return RotateUntilMatch(cameraTransform, paperView);
		yield return null;
	}

	private IEnumerator ZoomInToPaper()
	{
		zoomIn.BeginSlideIn();
		yield return new WaitForSeconds(zoomIn.GetDuration());
	}

	private IEnumerator RotateUntilMatch(Transform objectToRotate, Transform targetObject)
	{
	    while (Quaternion.Angle(objectToRotate.rotation, targetObject.rotation) > 0.01f)
	    {
	        objectToRotate.rotation = Quaternion.RotateTowards(
	            objectToRotate.rotation,
	            targetObject.rotation,
	            rotationSpeed * Time.deltaTime
	        );

	        yield return null;
	    }

	    objectToRotate.rotation = targetObject.rotation;
	}
}