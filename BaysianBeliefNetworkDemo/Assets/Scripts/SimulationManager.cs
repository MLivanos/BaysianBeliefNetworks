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
    public GameObject tree;
    public GameObject lights;
    public GameObject cafeLights;
    public GameObject wind;
    public GameObject clouds;
    public GameObject thunder;
    Dictionary<string, int> eventIndices = new Dictionary<string, int>();
    
    private bool[] truthValues;
    private bool sceneSet;

    private void Start()
    {
        graph = GameObject.Find("Graph").GetComponent<Graph>();
        Dictionary<string, int> nodeOrder = graph.GetNodeOrder();
        AssignIndices(nodeOrder);
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
        if (truthValues[eventIndices["Winter"]])
        {
            winter.SetActive(true);
        }
        else if (truthValues[eventIndices["Spring"]])
        {
            spring.SetActive(true);
        }
        else if (truthValues[eventIndices["Summer"]])
        {
            summer.SetActive(true);
        }
        else
        {
            fall.SetActive(true);
        }
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
        if (truthValues[eventIndices["Cloudy"]])
        {
            clouds.SetActive(true);
        }
        if (truthValues[eventIndices["Rain"]] && truthValues[eventIndices["Winter"]])
        {
            snow.SetActive(true);
        }
        else if (truthValues[eventIndices["Rain"]])
        {
            rain.SetActive(true);
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
        if(truthValues[eventIndices["Aliens"]])
        {
            aliens.SetActive(true);
        }
    }

    private void AssignIndices(Dictionary<string, int> nodeOrder)
    {
        eventIndices = new Dictionary<string, int>
        {
            { "Winter", nodeOrder["WinterNode"] },
            { "Spring", nodeOrder["SpringNode"] },
            { "Summer", nodeOrder["SummerNode"] },
            { "Fall", nodeOrder["FallNode"] },
            { "APD", nodeOrder["AtmosphericPressureDropNode"] },
            { "Cloudy", nodeOrder["CloudNode"] },
            { "Rain", nodeOrder["RainNode"] },
            { "Wind", nodeOrder["HighWindNode"] },
            { "Power", nodeOrder["PowerOutageNode"] },
            { "Tree", nodeOrder["TreeNode"] },
            { "Busy", nodeOrder["BusyNode"] },
            { "Thunder", nodeOrder["ThunderNode"] },
            { "Cafe", nodeOrder["CafeNode"] },
            { "Alien", nodeOrder["AlienNode"] },
            { "Dog", nodeOrder["DogNode"] },
            { "Cat", nodeOrder["CatNode"] }
        };
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
        if (truthValues[eventIndices["Tree"]])
        {
            // TODO: Make pretty
            tree.transform.eulerAngles = new Vector3(0f,0f,90f);
        }
    }
}
