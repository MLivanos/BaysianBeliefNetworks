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
        bool startedBus = false;
        while(timer < busSlideIn.GetDuration())
        {
            if (!startedBus && timer > 2f)
            {
                audioManager.PlayEffect("AmbientBus");
                startedBus = true;
            }
            foreach(Transform wheel in busWheels)
            {
                wheel.Rotate(Vector3.right * wheelSpeed * Time.deltaTime);
            }
            timer += Time.deltaTime;
            yield return null;
        }
        audioManager.FadeOutSFX(2f);
        yield return new WaitForSeconds(0.75f);
        yield return ViewPanel();
        AnimateText();
        audioManager.PlayEffect("BusDoors");
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