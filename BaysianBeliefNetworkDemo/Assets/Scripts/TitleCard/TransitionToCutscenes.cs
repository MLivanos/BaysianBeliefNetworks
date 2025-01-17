using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionToCutscenes : Transition
{
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject cloudParent;
    [SerializeField] private List<GameObject> objectsToHide;
    [SerializeField] private List<FadableGameObject> clouds;
    [SerializeField] private List<PlanetaryBehavior> planets;
    [SerializeField] private SlideInBehavior cameraSlide;
    [SerializeField] private FadableImage fadeToWhite;
    [SerializeField] private List<FadableElement> instructions;
    [SerializeField] private float fadeToWhiteTime;
    [SerializeField] private float fadeInstructionsTime;
    [SerializeField] private float minimumInstructionTime;
    private AsyncOperation asyncLoad;

    protected override IEnumerator TransitionToScene()
    {
        HideObjects();
        StopPlanets();
        cloudParent.SetActive(true);
        yield return null; // Allow one frame to pass to let clouds initialize
        ZoomToClouds();
        yield return new WaitForSeconds(cameraSlide.GetDuration()-fadeToWhiteTime);
        fadeToWhite.gameObject.SetActive(true);
        yield return FadeInstructions(true);
        yield return PreloadScene();
        yield return WaitMinimumInstructionTime();
        yield return FadeInstructions(false);
        yield return LoadScene();
    }

    private void HideObjects()
    {
        playButton.GetComponent<Image>().enabled = false;
        foreach(GameObject go in objectsToHide)
        {
            go.SetActive(false);
        }
    }

    private void ZoomToClouds()
    {
        cameraSlide.BeginSlideIn();
        foreach(FadableGameObject cloud in clouds)
        {
            cloud.FadeIn(cameraSlide.GetDuration());
        }
    }

    private void StopPlanets()
    {
        foreach(PlanetaryBehavior planet in planets)
        {
            planet.Stop();
        }
    }

    private IEnumerator PreloadScene()
    {
        asyncLoad = sceneManager.PreloadScene("Cutscenes");
        asyncLoad.allowSceneActivation = false;
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }
    }

    private IEnumerator LoadScene()
    {
        asyncLoad.allowSceneActivation = true;
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator FadeInstructions(bool fadeIn)
    {
        if (fadeIn) yield return fadeToWhite.Fade(fadeToWhiteTime, fadeIn);
        foreach(FadableElement instruction in instructions)
        {
            if (fadeIn) instruction.FadeIn(fadeInstructionsTime);
            else instruction.FadeOut(fadeInstructionsTime);
        }
        yield return new WaitForSeconds(fadeInstructionsTime);
    }

    private IEnumerator WaitMinimumInstructionTime()
    {
        float timer = 0f;
        while (timer < minimumInstructionTime)
        {
            timer += Time.deltaTime;
            if (Input.GetMouseButtonDown(1)) break;
            yield return null;
        }
    }
}
