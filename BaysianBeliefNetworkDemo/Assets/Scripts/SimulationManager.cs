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
    
    
    private bool[] truthValues;
    private bool sceneSet;

    private void Start()
    {
        graph = GameObject.Find("Graph").GetComponent<Graph>();
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
        /*
        Order:
        0 Winter
        1 Spring
        2 Summer
        3 Fall
        4 Rain
        5 Busy
        6 Alien
        7 Dog
        8 Cat
        9 Cafe
        */
        truthValues = sample;
        SetSeason();
        SetCafe();
        SetRain();
        SetDog();
        SetCat();
        SetAliens();
        SetBusy();
    }

    private void SetSeason()
    {
        if (truthValues[0])
        {
            winter.SetActive(true);
        }
        else if (truthValues[1])
        {
            spring.SetActive(true);
        }
        else if (truthValues[2])
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
        if (truthValues[9])
        {
            if (truthValues[5])
            {
                busyCafe.SetActive(true);
            }
            cafeOpen.SetActive(true);
        }
        else
        {
            cafeClosed.SetActive(true);
        }
    }

    private void SetBusy()
    {
        if (truthValues[5])
        {
            busy.SetActive(true);
        }
        else
        {
            notBusy.SetActive(true);
        }
    }

    private void SetRain()
    {
        if (truthValues[4] && truthValues[0])
        {
            snow.SetActive(true);
        }
        else if (truthValues[4])
        {
            rain.SetActive(true);
        }
    }

    private void SetCat()
    {
        if (truthValues[8])
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
        dog.barking = truthValues[7];
    }

    private void SetAliens()
    {
        if(truthValues[6])
        {
            aliens.SetActive(true);
        }
    }
}
