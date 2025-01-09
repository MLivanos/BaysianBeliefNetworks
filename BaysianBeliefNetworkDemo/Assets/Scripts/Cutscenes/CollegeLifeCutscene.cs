using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class CollegeLifeCutscene : IntroCutscene
{
	[SerializeField] private GameObject solidLine;
	[SerializeField] private GameObject brokenLine;
	[SerializeField] private float timeBeforeText;
    [SerializeField] private GameObject leavingScene;
    [SerializeField] private Camera collegeCamera;
    [SerializeField] private Camera leavingCamera;
    [SerializeField] private RawImage collegeImage;
    [SerializeField] private RawImage leavingImage;
    [SerializeField] private float transitionTime;
    [SerializeField] private GameObject[] directionalLights;
    [SerializeField] private SlideInBehavior cameraSlide;
    [SerializeField] private float slideTime;
    private FadableImage collegeFadable;
    private FadableImage leavingFadable;
    private RenderTexture collegeTexture;
    private RenderTexture leavingTexture;

	protected override IEnumerator PlayScene()
    {
        yield return new WaitForSeconds(1f);
        solidLine.GetComponent<FadableImage>().FadeIn(slideTime);
        cameraSlide.BeginSlideIn();
        yield return new WaitForSeconds(slideTime);
    	yield return new WaitForSeconds(timeBeforeText);
    	solidLine.SetActive(false);
    	brokenLine.SetActive(true);
    	yield return ViewPanel();
        AnimateText();
    }

    public override void Interrupt()
    {
    	return;
    }

    protected override IEnumerator ExitTransition()
    {
        foreach(GameObject directionalLight in directionalLights)
        {
            directionalLight.SetActive(false);
        }
        leavingScene.SetActive(true);
        yield return null;
        SetupRenderTextureCameras();
    	collegeFadable.FadeOut(transitionTime);
        yield return new WaitForSeconds(transitionTime);
    }

    private void SetupRenderTextureCameras()
    {
        collegeCamera.gameObject.SetActive(true);
        leavingCamera.gameObject.SetActive(true);

        collegeFadable = collegeImage.gameObject.GetComponent<FadableImage>();
        leavingFadable = leavingImage.gameObject.GetComponent<FadableImage>();

        collegeFadable.SetAlpha(1);
        leavingFadable.SetAlpha(1);

        collegeTexture = new RenderTexture(Screen.width, Screen.height, 16);
        leavingTexture = new RenderTexture(Screen.width, Screen.height, 16);

        collegeCamera.targetTexture = collegeTexture;
        leavingCamera.targetTexture = leavingTexture;

        collegeImage.texture = collegeTexture;
        leavingImage.texture = leavingTexture;
    }
}