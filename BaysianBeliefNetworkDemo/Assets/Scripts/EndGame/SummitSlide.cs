using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SummitSlide : EndGameCutscene
{
	[SerializeField] private SlideInBehavior zoomIn;
	[SerializeField] private SlideInBehavior transitionSlide;
	[SerializeField] private Transform paperView;
	[SerializeField] private Transform panelView;
	[SerializeField] private float waitBeforeZoomIn;
	[SerializeField] private float waitBeforeRotateUp;
	[SerializeField] private float rotationSpeed;

	protected override IEnumerator PlayScene()
	{
		yield return new WaitForSeconds(waitBeforeZoomIn);
		yield return ZoomInToPaper();
		yield return ViewPanel();
		AnimateText();
		yield return new WaitForSeconds(waitBeforeRotateUp);
		yield return RotateUntilMatch(cameraTransform, panelView);

	}

	protected override IEnumerator ExitTransition()
	{
		SnapCameraToPosition(panelView);
		transitionSlide.BeginSlideIn();
		yield return RotateUntilMatch(cameraTransform, paperView);
	}

	private IEnumerator ZoomInToPaper()
	{
		StartCoroutine(zoomIn.Slide(true));
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

	public override void Interrupt()
	{
		SnapCameraToPosition(panelView);
	}

	private void SnapCameraToPosition(Transform referenceTransform)
	{
		StopAllCoroutines();
		cameraTransform.position = referenceTransform.position;
		cameraTransform.rotation = referenceTransform.rotation;
	}
}