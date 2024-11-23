using UnityEngine;
using UnityEngine.UI;

public class CoffeeTimestep : CyclicalTimestepBehavior
{
    [SerializeField] private float[] coffeeLevel;
    [SerializeField] private RectTransform coffeeTransform;
    [SerializeField] private ParticleSystem steam;

    private void Update()
    {
        if (Input.GetKeyDown("space")) Step();
    }

    protected override void Step()
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
}