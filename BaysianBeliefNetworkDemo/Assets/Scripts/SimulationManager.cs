using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    private Graph graph;
    public GameObject winter;
    public GameObject spring;
    public GameObject summer;
    public GameObject fall;
    public GameObject cafeOpen;
    public GameObject cafeClosed;
    public GameObject busy;
    public GameObject notBusy;
    public GameObject busyCafe;
    public GameObject catHide;
    public GameObject catOut;
    public DogController dog;
    
    public GameObject aliens;
    public GameObject rain;
    public GameObject snow;
    public GameObject lights;
    public GameObject cafeLights;
    public GameObject wind;
    public GameObject clouds;
    public GameObject rainClouds;
    public GameObject thunder;
    public Light directionalLight;
    private Transform tree;
    Dictionary<string, int> eventIndices;
    
    private bool[] truthValues;
    private bool sceneSet;

    private void Start()
    {
        graph = GameObject.Find("Graph").GetComponent<Graph>();
        eventIndices = graph.AssignIndices();
        SetTruthValues(graph.VisualizeSample());
    }

    private void Update()
    {
        if(Input.GetKeyDown("r"))
        {
            SetTruthValues(graph.VisualizeSample());
        }
    }

    public void SetTruthValues(bool[] sample)
    {
        truthValues = sample;
        SetSeason();
        SetCafe();
        SetWeather();
        SetDog();
        SetCat();
        SetAliens();
        SetBusy();
        SetTree();
        SetPowerOutage();
    }

    private void SetSeason()
    {
        GameObject seasonItems;
        if (truthValues[eventIndices["Winter"]])
        {
            seasonItems = winter;
            winter.SetActive(true);
        }
        else if (truthValues[eventIndices["Spring"]])
        {
            seasonItems = spring;
            spring.SetActive(true);
        }
        else if (truthValues[eventIndices["Summer"]])
        {
            seasonItems = summer;
            summer.SetActive(true);
        }
        else
        {
            seasonItems = fall;
            fall.SetActive(true);
        }
        tree = seasonItems.transform.Find("Tree");
    }

    private void SetCafe()
    {
        if (truthValues[eventIndices["Cafe"]])
        {
            if (truthValues[eventIndices["Busy"]])
            {
                busyCafe.SetActive(true);
            }
            cafeOpen.SetActive(true);
        }
        else
        {
            cafeClosed.SetActive(true);
        }
        if(truthValues[eventIndices["Power"]])
        {
            cafeLights.SetActive(true);
        }
    }

    private void SetBusy()
    {
        if (truthValues[eventIndices["Busy"]])
        {
            busy.SetActive(true);
        }
        else
        {
            notBusy.SetActive(true);
        }
    }

    private void SetWeather()
    {
        if (!truthValues[eventIndices["Rain"]] && truthValues[eventIndices["Cloudy"]])
        {
            directionalLight.intensity = 0f;
            clouds.SetActive(true);
        }
        else if (truthValues[eventIndices["Rain"]] && truthValues[eventIndices["Winter"]])
        {
            snow.SetActive(true);
        }
        else if (truthValues[eventIndices["Rain"]])
        {
            rainClouds.SetActive(true);
            rain.SetActive(true);
        }
        else
        {
            lights.SetActive(false);
        }
        if (truthValues[eventIndices["Wind"]])
        {
            wind.SetActive(true);
            rain.transform.eulerAngles = new Vector3(10f,0f,0f);
            snow.transform.eulerAngles = new Vector3(10f,0f,0f);
        }
        if (truthValues[eventIndices["Thunder"]])
        {
            thunder.SetActive(true);
        }
    }

    private void SetCat()
    {
        if (truthValues[eventIndices["Cat"]])
        {
            catHide.SetActive(true);
        }
        else
        {
            catOut.SetActive(true);
        }
    }

    private void SetDog()
    {
        dog.barking = truthValues[eventIndices["Dog"]];
    }

    private void SetAliens()
    {
        if(truthValues[eventIndices["Alien"]])
        {
            aliens.SetActive(true);
        }
    }

    private void SetPowerOutage()
    {
        if (truthValues[eventIndices["Power"]])
        {
            cafeLights.SetActive(false);
            lights.SetActive(false);
        }
    }

    private void SetTree()
    {
        tree.eulerAngles = new Vector3(0f,0f,0f);
        if (truthValues[eventIndices["Tree"]])
        {
            // TODO: Make pretty
            tree.eulerAngles = new Vector3(0f,0f,90f);
        }
    }
}
