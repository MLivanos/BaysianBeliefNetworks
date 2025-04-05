using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SaveSystem : MonoBehaviour
{
    [System.Serializable]
    public class NodeSaveData
    {
        public string nodeName;
        public float[] jointProbabilityDistribution;
    }

    [System.Serializable]
    public class GameManagerSaveData
    {
        public float timeProgress;
    }

    [System.Serializable]
    public class SaveData
    {
        public List<NodeSaveData> nodeData = new List<NodeSaveData>();
        public GameManagerSaveData gameManagerData;
    }

    private string filePath;

    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "savegame.json");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveGame();
            Debug.Log("Save triggered with S");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGame();
            Debug.Log("Load triggered with L");
        }
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();
        Node[] nodes = FindObjectsOfType<Node>();
        foreach (Node node in nodes)
        {
            NodeSaveData nsd = new NodeSaveData();
            nsd.nodeName = node.gameObject.name;
            nsd.jointProbabilityDistribution = node.JointProbabilityDistribution();
            data.nodeData.Add(nsd);
        }

        GameManager gameManager = GameManager.instance;
        if (gameManager != null)
        {
            GameManagerSaveData gameManagerData = new GameManagerSaveData();
            gameManagerData.timeProgress = gameManager.TimeProgress();
            data.gameManagerData = gameManagerData;
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
    }

    public void LoadGame()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Save file not found at " + filePath);
            return;
        }

        string json = File.ReadAllText(filePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        foreach (NodeSaveData nsd in data.nodeData)
        {
            Node node = FindNodeByName(nsd.nodeName);
            if (node != null)
            {
                node.SetJointProbabilityDistribution(nsd.jointProbabilityDistribution);
            }
            else
            {
                Debug.LogWarning("Node with name " + nsd.nodeName + " not found in scene.");
            }
        }

        GameManager gm = FindObjectOfType<GameManager>();
        Debug.Log(data.gameManagerData.timeProgress);
        if (gm != null && data.gameManagerData != null)
        {
            gm.SetTimeProgress(data.gameManagerData.timeProgress);
        }

        Debug.Log("Game loaded.");
    }

    private Node FindNodeByName(string nodeName)
    {
        Node[] nodes = FindObjectsOfType<Node>();
        foreach (Node node in nodes)
        {
            if (node.gameObject.name == nodeName)
                return node;
        }
        return null;
    }
}