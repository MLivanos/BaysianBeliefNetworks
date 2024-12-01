using UnityEngine;

public class SkyTimestep : CyclicalTimestepBehavior
{
    [SerializeField] private Renderer skyRenderer;
    [SerializeField] private Color[] skyColors;
    [SerializeField] private Light directionalLight;
    [SerializeField] private float[] lightIntensities;
    [SerializeField] private float[] skyboxIntensities;
    [SerializeField] private float[] sunAngles;
    private Material skyMaterial;

    private void Start()
    {
        skyMaterial = skyRenderer.material;
        UpdateSky();
    }

    public override void Step()
    {
        base.Step();
        UpdateSky();
    }

    private void UpdateSky()
    {
        skyMaterial.SetColor("_ColorTop", skyColors[step]);

        RenderSettings.ambientIntensity = skyboxIntensities[step];

        directionalLight.intensity = lightIntensities[step];
        Vector3 lightRotation = directionalLight.transform.eulerAngles;
        lightRotation.x = sunAngles[step];
        directionalLight.transform.eulerAngles = lightRotation;
    }
}
