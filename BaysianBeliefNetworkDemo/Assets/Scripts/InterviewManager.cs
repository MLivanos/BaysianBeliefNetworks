using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InterviewManager : MonoBehaviour
{
    [SerializeField] private List<string> greetings;
    [SerializeField] private List<string> winterDescriptions;
    [SerializeField] private List<string> springDescriptions;
    [SerializeField] private List<string> summerDescriptions;
    [SerializeField] private List<string> fallDescriptions;
    [SerializeField] private List<string> apdDescriptions;
    [SerializeField] private List<string> windDescriptions;
    [SerializeField] private List<string> cloudyDescriptions;
    [SerializeField] private List<string> rainDescriptions;
    [SerializeField] private List<string> powerDescriptions;
    [SerializeField] private List<string> treeDescriptions;
    [SerializeField] private List<string> busyDescriptions;
    [SerializeField] private List<string> thunderDescriptions;
    [SerializeField] private List<string> cafeDescriptions;
    [SerializeField] private List<string> dogDescriptions;
    [SerializeField] private List<string> catDescriptions;
    [SerializeField] private List<string> friendlyDescriptions;
    [SerializeField] private List<string> aggressiveDescriptions;
    [SerializeField] private float friednlyBias;
    private (List<Node>, List<List<string>>) seasons;
    private (List<Node>, List<List<string>>) weather;
    private (List<Node>, List<List<string>>) consequences;
    private (List<Node>, List<List<string>>) humanActivity;
    private (List<Node>, List<List<string>>) animalBehavior;
    private List<(List<Node>, List<List<string>>)> nonSeasonEvents = new List<(List<Node>, List<List<string>>)>();
    private LikelihoodWeightingSampler sampler;
    private Graph graph;
    private Dictionary<string, int> eventIndices;
    int seasonIndex;

    private void Start()
    {
        graph = GameObject.Find("Graph").GetComponent<Graph>();
        sampler = graph.gameObject.GetComponent<LikelihoodWeightingSampler>();
        StartCoroutine(InstantiateManager());
    }

    private IEnumerator InstantiateManager()
    {
        yield return null;
        eventIndices = GameObject.Find("Graph").GetComponent<Graph>().AssignIndices();
        List<Node> nodes = GameObject.Find("Graph").GetComponent<Graph>().GetAllNodes();
        AddSeasonNodes(nodes);
        AddWeatherNodes(nodes);
        AddConsequenceNodes(nodes);
        AddHumanActivityNodes(nodes);
        AddAnimalNodes(nodes);
        seasonIndex = nonSeasonEvents.Count;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DrawRandomEvents(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DrawRandomEvents(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            DrawRandomEvents(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            DrawRandomEvents(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            DrawRandomEvents(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            DrawRandomEvents(6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            DrawRandomEvents(7);
        }
    }

    private void AddSeasonNodes(List<Node> nodes)
    {
        List<List<string>> seasonDescriptions = new List<List<string>>
            {winterDescriptions,springDescriptions,summerDescriptions,fallDescriptions};
        List<Node> seasonNodes = new List<Node>
            {nodes[eventIndices["Winter"]], nodes[eventIndices["Spring"]], nodes[eventIndices["Summer"]], nodes[eventIndices["Fall"]]};
        seasons = (seasonNodes, seasonDescriptions);
    }

    private void AddWeatherNodes(List<Node> nodes)
    {
        List<List<string>> weatherDescriptions = new List<List<string>>
            {apdDescriptions, cloudyDescriptions, rainDescriptions, windDescriptions, thunderDescriptions};
        List<Node> weatherNodes = new List<Node>
            {nodes[eventIndices["APD"]], nodes[eventIndices["Cloudy"]], nodes[eventIndices["Rain"]], nodes[eventIndices["Wind"]], nodes[eventIndices["Thunder"]]};
        weather = (weatherNodes, weatherDescriptions);
        nonSeasonEvents.Add(weather);
    }

    private void AddConsequenceNodes(List<Node> nodes)
    {
        List<Node> animalBehaviorNodes = new List<Node> {nodes[eventIndices["Power"]], nodes[eventIndices["Tree"]]};
        List<List<string>> consequenceDescriptions = new List<List<string>> {powerDescriptions, treeDescriptions};
        consequences = (animalBehaviorNodes, consequenceDescriptions);
        nonSeasonEvents.Add(consequences);
    }

    private void AddHumanActivityNodes(List<Node> nodes)
    {
        List<Node> humanNodes = new List<Node> {nodes[eventIndices["Busy"]], nodes[eventIndices["Cafe"]]};
        List<List<string>> humanDescriptions = new List<List<string>> {busyDescriptions, cafeDescriptions};
        humanActivity = (humanNodes, humanDescriptions);
        nonSeasonEvents.Add(humanActivity);
    }

    private void AddAnimalNodes(List<Node> nodes)
    {
        List<Node> animalNodes = new List<Node> {nodes[eventIndices["Dog"]], nodes[eventIndices["Cat"]]};
        List<List<string>> animalDescriptions = new List<List<string>> {dogDescriptions, catDescriptions};
        animalBehavior = (animalNodes, animalDescriptions);
        nonSeasonEvents.Add(animalBehavior);
    }

    private (List<Node>, List<List<string>>) DrawRandomEventType(bool canBeSeason)
    {
        int index = (int)Mathf.Round(Random.Range(0, seasonIndex + 0.49f - (canBeSeason ? 0 : 1)));
        if (index == seasonIndex) return seasons;
        return nonSeasonEvents[index];
    }

    private (Node, string) DrawRandomEvent((List<Node>, List<List<string>>) eventType)
    {
        int index = GetRandomIndex(eventType.Item1);
        int stringIndex = GetRandomIndex(eventType.Item2[index]);
        return (eventType.Item1[index], eventType.Item2[index][stringIndex]);
    }

    private int GetRandomIndex<T>(List<T> l)
    {
        return (int)Mathf.Round(Random.Range(0, l.Count - 0.51f));
    }

    private float CalculateProbability(int numberOfSamples=10000)
    {
        int[] positiveQuery = new int[1] {eventIndices["Alien"]};
        int[] negativeQuery = new int[0];
        for (int i = 0; i < numberOfSamples; i++)
        {
            sampler.Sample(positiveQuery, negativeQuery, graph.GetRootNodes().ToList());
        }
        return sampler.CalculateProbability();
    }

    private float CalculateProbability(float precision, int maxIterations, int windowSize, float z=1.96f)
    {
        List<float> slidingWindow = new List<float>();
        for (int i = 0; i < windowSize - 1; i++)
        {
            slidingWindow.Add(CalculateProbability());
        }
        slidingWindow.Add(0f);
        for(int i = windowSize - 1; i < maxIterations; i++)
        {
            slidingWindow[i % windowSize] = CalculateProbability();
            float mean = slidingWindow.Average();
            float std = Mathf.Sqrt(slidingWindow.Select(p => (p - mean) * (p - mean)).Average());

            float standardError = std / Mathf.Sqrt(windowSize);
            float ciWidth = z * standardError;

            if (ciWidth < slidingWindow[i % windowSize] * (1 - precision))
            {
                return slidingWindow[i % windowSize];
            }
        }
        return slidingWindow[(maxIterations-1)%windowSize];
    }

    private void DrawRandomEvents(int numberOfEvents)
    {
        sampler.ClearEvidence();
        sampler.Reset();
        bool hasSeason = false;
        Node node;
        string description;
        List<Node> positiveEvidence = new List<Node>();
        List<Node> negativeEvidence = new List<Node>();
        string evidence = "";
        for(int i=0; i<numberOfEvents; i++)
        {
            (List<Node>, List<List<string>>) eventType = DrawRandomEventType(!hasSeason);
            if (eventType == seasons) hasSeason = true;
            (node, description) = DrawRandomEvent(eventType);
            positiveEvidence.Add(node);
            sampler.AddToEvidence(node, true);
            evidence += node.GetAbriviation() + ",";
        }
        bool friednly = Random.value <= friednlyBias;
        List<string> relevantList = friednly ? friendlyDescriptions : aggressiveDescriptions;
        int index = GetRandomIndex(relevantList);
        Debug.Log(evidence);
        Debug.Log(friednly);
        Debug.Log(CalculateProbability(0.98f, 15, 3));
    }
}