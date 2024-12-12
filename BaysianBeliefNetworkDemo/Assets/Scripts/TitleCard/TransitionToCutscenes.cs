using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionToCutscenes : Transition
{
    [SerializeField] private GameObject cloudParent;
    [SerializeField] private List<GameObject> objectsToHide;
    [SerializeField] private List<FadableGameObject> clouds;
    [SerializeField] private List<PlanetaryBehavior> planets;
    [SerializeField] private SlideInBehavior cameraSlide;
    [SerializeField] private FadableImage fadeToWhite;
    [SerializeField] private float fadeToWhiteTime;

    protected override IEnumerator TransitionToScene()
    {
        HideObjects();
        StopPlanets();
        cloudParent.SetActive(true);
        yield return null; // Allow one frame to pass to let clouds initialize
        ZoomToClouds();
        yield return new WaitForSeconds(cameraSlide.GetDuration()-fadeToWhiteTime);
        fadeToWhite.gameObject.SetActive(true);
        yield return fadeToWhite.Fade(fadeToWhiteTime, true);
        sceneManager.GoToCutscenes();
    }

    private void HideObjects()
    {
        GetComponent<Image>().enabled = false;
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
}
