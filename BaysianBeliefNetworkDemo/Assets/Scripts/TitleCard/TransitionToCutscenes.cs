using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionToCutscenes : Transition
{
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject cloudParent;
    [SerializeField] private List<TypewriterEffect> objectsToTypeOut;
    [SerializeField] private List<TypewriterEffect> objectsToTypeIn;
    [SerializeField] private List<FadableGameObject> clouds;
    [SerializeField] private List<PlanetaryBehavior> planets;
    [SerializeField] private SlideInBehavior cameraSlide;
    [SerializeField] private FadableImage fadeToWhite;
    [SerializeField] private List<FadableElement> instructions;
    [SerializeField] private float fadeToWhiteTime;
    [SerializeField] private float fadeInstructionsTime;
    [SerializeField] private float minimumInstructionTime;
    [SerializeField] private float hangOnWhite;
    private AsyncOperation asyncLoad;

    private void Start()
    {
        foreach(TypewriterEffect typewriter in objectsToTypeIn)
        {
            typewriter.Clear();
        }
    }

    protected override IEnumerator TransitionToScene()
    {
        TypeOutObjects();
        TypeInObjects();
        StopPlanets();
        cloudParent.SetActive(true);
        yield return new WaitForSeconds(1f); // Wait for glitch effect, typeout
        ZoomToClouds();
        AudioManager.instance.FadeOutMusic(cameraSlide.GetDuration()-fadeToWhiteTime);
        yield return new WaitForSeconds(cameraSlide.GetDuration()-fadeToWhiteTime);
        AudioManager.instance.PlayEffect("CloudWoosh");
        fadeToWhite.gameObject.SetActive(true);
        yield return FadeInstructions(true);
        yield return PreloadScene();
        yield return WaitForSecondsInterruptible(minimumInstructionTime);
        yield return FadeInstructions(false);
        yield return new WaitForSeconds(hangOnWhite);
        yield return LoadScene();
    }

    private void TypeOutObjects()
    {
        foreach(TypewriterEffect entity in objectsToTypeOut)
        {
            entity.TypewriterDelete();
        }
    }

    private void TypeInObjects()
    {
        foreach(TypewriterEffect typewriter in objectsToTypeIn)
        {
            typewriter.UpdateText();
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
        asyncLoad = GetComponent<SceneManagerScript>().PreloadScene("Cutscenes");
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

    private IEnumerator WaitForSecondsInterruptible(float timeToWait)
    {
        float timer = 0f;
        while (timer < timeToWait)
        {
            timer += Time.deltaTime;
            if (Input.GetMouseButtonDown(0)) break;
            yield return null;
        }
    }
}
