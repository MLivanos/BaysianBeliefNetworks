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
        public string queryHistory;
    }

    private string filePath;

    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "savegame.json");
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("ShouldLoad", 0) == 1)
        {
            LoadGame();
            PlayerPrefs.SetInt("ShouldLoad", 0);
            PlayerPrefs.Save();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveGame();
            Debug.Log("💾 Save triggered with S");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGame();
            Debug.Log("📂 Load triggered with L");
        }
    }

    public void SaveGame()
    {
        SaveData data = CollectSaveData();
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
        ApplySaveData(data);
    }

    public void SetShouldLoadFlag()
    {
        PlayerPrefs.SetInt("ShouldLoad", 1);
        PlayerPrefs.Save();
    }

    // ---------- Internal Abstractions Below ----------

    private SaveData CollectSaveData()
    {
        SaveData data = new SaveData();

        Node[] nodes = FindObjectsOfType<Node>();
        foreach (Node node in nodes)
        {
            NodeSaveData nsd = new NodeSaveData
            {
                nodeName = node.gameObject.name,
                jointProbabilityDistribution = node.JointProbabilityDistribution()
            };
            data.nodeData.Add(nsd);
        }

        GameManager gm = GameManager.instance;
        if (gm != null)
        {
            data.gameManagerData = new GameManagerSaveData
            {
                timeProgress = gm.TimeProgress()
            };
        }

        SamplingHistory history = FindObjectOfType<SamplingHistory>();
        if (history != null)
        {
            data.queryHistory = history.GetHistoryText();
        }

        return data;
    }

    private void ApplySaveData(SaveData data)
    {
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
        if (gm != null && data.gameManagerData != null)
        {
            gm.SetTimeProgress(data.gameManagerData.timeProgress);
        }

        SamplingHistory history = FindObjectOfType<SamplingHistory>();
        if (history != null && !string.IsNullOrEmpty(data.queryHistory))
        {
            history.SetHistoryText(data.queryHistory);
        }
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