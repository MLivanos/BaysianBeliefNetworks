using UnityEngine;
using UnityEngine.UI;

public class CoffeeTimestep : CyclicalTimestepBehavior
{
    [SerializeField] private GameObject[] sugars;
    [SerializeField] private GameObject[] rings;
    [SerializeField] private float[] coffeeLevel;
    [SerializeField] private RectTransform coffeeTransform;
    [SerializeField] private ParticleSystem steam;

    public override void Step()
    {
        base.Step();
        if (step < 1)
        {
            steam.Simulate(3f);
            steam.Play();
        }
        else
        {
            steam.Pause();
            steam.Clear();
        }
        Vector3 position = coffeeTransform.anchoredPosition3D;
        position.y = coffeeLevel[step];
        coffeeTransform.anchoredPosition3D = position;
    }

    public override void Cycle()
    {
        if (cycle < sugars.Length) sugars[cycle].SetActive(false);
        if (cycle < rings.Length) rings[cycle].SetActive(true);
        base.Cycle();
    }
}