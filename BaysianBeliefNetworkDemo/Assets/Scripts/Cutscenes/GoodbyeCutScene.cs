using System.Collections;
using UnityEngine;

public class GoodbyeCutScene : CutsceneBehavior
{
	[SerializeField] private SlideInBehavior busSlideIn;
    [SerializeField] private Transform[] busWheels;
    [SerializeField] private float wheelSpeed;
    [SerializeField] private Transform[] busDoors;
    [SerializeField] private float doorOpenSpeed;

    protected override IEnumerator PlayScene()
    {
    	yield return null; // Wait one frame to let the slideIn script initialize itself
        busSlideIn.BeginSlideIn();
        float timer = 0f;
        while(timer < busSlideIn.GetDuration())
        {
            foreach(Transform wheel in busWheels)
            {
                wheel.Rotate(Vector3.right * wheelSpeed * Time.deltaTime);
            }
            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.75f);
        yield return ViewPanel();
        AnimateText();
        while(busDoors[0].localEulerAngles.y < 90f)
        {
            busDoors[0].Rotate(Vector3.up * doorOpenSpeed * Time.deltaTime);
            busDoors[1].Rotate(-Vector3.up * doorOpenSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public override void Interrupt()
    {
    	return;
    }

    protected override IEnumerator ExitTransition()
    {
    	yield return null;
    }
}