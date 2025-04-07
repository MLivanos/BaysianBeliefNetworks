using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InstanceSettings
{
    public Mesh mesh; // The object's mesh
    public Material primaryMaterial; // The primary material
    public Material secondaryMaterial; // Optional secondary material
    public float density; // Instances per unit
    public float minDistance; // Minimum distance between instances
    public float minX; // Minimum value of x for generation
    public bool conformToSlope; // Whether to conform to the slope normal
}

public class EnvironmentalInstancer : MonoBehaviour
{
    [Header("Instance Settings")]
    [SerializeField] private InstanceSettings[] instanceSettings; // Array of instance settings

    private List<Vector3> usedPositions = new List<Vector3>();
    private List<Matrix4x4> instanceMatrices = new List<Matrix4x4>();
    private Mesh terrainMesh;
    private List<int> meshIndices = new List<int>(); // Indices of meshes used for each instance
    private bool instantiated = false;
    private bool firstPass = true;

    public void SetTerrainMesh(Mesh terrainMesh_)
    {
        terrainMesh = terrainMesh_;
    }

    public void InitializePositions(List<Vector3> positions)
    {
        usedPositions.AddRange(positions);
    }

    private void Update()
    {
        if (!instantiated)
        {
            return;
        }
        // Render the instances each frame
        RenderInstances();
    }

    public void GenerateAllInstances()
    {
        int meshIndex = 0;
        // Generate instances based on the new instanceSettings array
        foreach (InstanceSettings settings in instanceSettings)
        {
            instanceMatrices.AddRange(GenerateInstances(settings,meshIndex));
            meshIndex++;
        }
        instantiated = true;
    }

    private List<Matrix4x4> GenerateInstances(InstanceSettings settings, int meshIndex)
    {
        Bounds bounds = terrainMesh.bounds;
        int instanceCount = (int)(((bounds.max.x - bounds.min.x) * (bounds.max.z - bounds.min.z)) * settings.density);
        List<Matrix4x4> matrices = new List<Matrix4x4>();

        for (int i = 0; i < instanceCount; i++)
        {
            for (int attempts = 0; attempts < 10; attempts++)
            {
                //float x = Random.Range(Mathf.Max(bounds.min.x, bounds.min.x + settings.minX), bounds.max.x);
                float x = Random.Range(bounds.min.x + settings.minX, bounds.max.x);
                float z = Random.Range(bounds.min.z, bounds.max.z);
                Vector3 position = transform.TransformPoint(new Vector3(x, FindHeightAtPosition(terrainMesh.vertices, x, z), z));

                if (IsValidPosition(position, settings.minDistance))
                {
                    Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    float scale = Random.Range(0.8f, 1.2f);
                    Vector3 scaling = new Vector3(scale, scale, scale);

                    // Optional: conform to slope
                    if (settings.conformToSlope)
                    {
                        rotation = Quaternion.LookRotation(Vector3.Cross(Vector3.up, scaling), scaling);
                    }

                    Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, scaling);
                    matrices.Add(matrix);

                    // Add the mesh index for tracking
                    meshIndices.Add(meshIndex);
                    usedPositions.Add(position);
                    break;
                }
            }
        }

        return matrices;
    }

    // Helper function to get the normal vector at a specific terrain position
    private Vector3 GetNormalAtPosition(Vector3[] vertices, int[] triangles, float x, float z)
    {
        Vector3 closestPoint = Vector3.zero;
        float closestDistanceSqr = Mathf.Infinity;

        // Find the closest triangle vertex to determine the normal
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = vertices[triangles[i]];
            Vector3 v1 = vertices[triangles[i + 1]];
            Vector3 v2 = vertices[triangles[i + 2]];

            Vector3 triangleCenter = (v0 + v1 + v2) / 3;
            float distanceSqr = (triangleCenter.x - x) * (triangleCenter.x - x) + (triangleCenter.z - z) * (triangleCenter.z - z);
            if (distanceSqr < closestDistanceSqr)
            {
                closestPoint = triangleCenter;
                closestDistanceSqr = distanceSqr;
            }
        }

        // Compute normal using cross product between edges of the triangle
        Vector3 normal = Vector3.Cross(vertices[triangles[1]] - vertices[triangles[0]], vertices[triangles[2]] - vertices[triangles[0]]).normalized;
        return normal;
    }


    private bool IsValidPosition(Vector3 position, float minDistance)
    {
        if (position.y < 0)
        {
            return false;
        }
        foreach (Vector3 usedPosition in usedPositions)
        {
            if (Vector3.Distance(position, usedPosition) < minDistance)
            {
                return false;
            }
        }
        return true;
    }

    private void RenderInstances()
    {
        // Group instance matrices by their setting index.
        Dictionary<int, List<Matrix4x4>> groupedMatrices = new Dictionary<int, List<Matrix4x4>>();
        for (int i = 0; i < instanceMatrices.Count; i++)
        {
            int settingIndex = meshIndices[i];
            if (!groupedMatrices.ContainsKey(settingIndex))
            {
                groupedMatrices[settingIndex] = new List<Matrix4x4>();
            }
            groupedMatrices[settingIndex].Add(instanceMatrices[i]);
        }

        // Render each group in batches of 1023.
        foreach (var kvp in groupedMatrices)
        {
            int settingIndex = kvp.Key;
            InstanceSettings settings = instanceSettings[settingIndex];
            Material material = settings.primaryMaterial;
            List<Matrix4x4> matrices = kvp.Value;

            for (int i = 0; i < matrices.Count; i += 1023)
            {
                int count = Mathf.Min(1023, matrices.Count - i);
                Matrix4x4[] batchMatrices = matrices.GetRange(i, count).ToArray();

                Graphics.RenderMeshInstanced(
                    new RenderParams(material),
                    settings.mesh,
                    0,
                    batchMatrices
                );
            }
        }
    }

    /*private void RenderInstances()
    {
        for (int i = 0; i < instanceMatrices.Count; i += 1023)
        {
            int count = Mathf.Min(1023, instanceMatrices.Count - i);

            // Create a subarray of matrices for the current batch
            Matrix4x4[] batchMatrices = instanceMatrices.GetRange(i, count).ToArray();

            // Render each instance
            for (int j = 0; j < count; j++)
            {
                int matrixIndex = i + j;

                InstanceSettings settings = instanceSettings[meshIndices[matrixIndex]];

                // Check if it's the first pass and we need to handle multi-material meshes
                if (firstPass && settings.secondaryMaterial != null)
                {
                    GameObject tempObject = new GameObject("InstanceObject");
                    MeshRenderer meshRenderer = tempObject.AddComponent<MeshRenderer>();
                    meshRenderer.materials = new Material[] { 
                        settings.primaryMaterial, 
                        settings.secondaryMaterial 
                    };

                    MeshFilter meshFilter = tempObject.AddComponent<MeshFilter>();
                    meshFilter.mesh = settings.mesh;
                    tempObject.transform.position = batchMatrices[j].GetColumn(3);
                    tempObject.transform.parent = transform;
                }
                else
                {
                    // Use GPU instancing for single-material meshes
                    Material material = settings.primaryMaterial; 
                    Graphics.RenderMeshInstanced(
                        new RenderParams(material),
                        settings.mesh,
                        0,
                        new Matrix4x4[] { batchMatrices[j] } // Render individual matrix from the batch
                    );
                }
            }
        }
        firstPass = false;
    }*/


    private float FindHeightAtPosition(Vector3[] vertices, float x, float z)
    {
        float closestDistanceSqr = Mathf.Infinity;
        float closestHeight = 0;

        foreach (var vertex in vertices)
        {
            // Calculate the squared distance to avoid using the costly square root
            float distanceSqr = (vertex.x - x) * (vertex.x - x) + (vertex.z - z) * (vertex.z - z);
            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closestHeight = vertex.y;
            }
        }

        return closestHeight;
    }
}