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
        UpdateSky(0);
    }

    public override void Step()
    {
        base.Step();
        UpdateSky(step);
    }

    private void UpdateSky(int index)
    {
        skyMaterial.SetColor("_ColorTop", skyColors[index]);
        RenderSettings.ambientIntensity = skyboxIntensities[index];

        directionalLight.intensity = lightIntensities[index];
        Vector3 lightRotation = directionalLight.transform.eulerAngles;
        lightRotation.x = sunAngles[index];
        directionalLight.transform.eulerAngles = lightRotation;
    }
}
