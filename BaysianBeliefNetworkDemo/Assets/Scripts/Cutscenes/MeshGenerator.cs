using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    [SerializeField] private int xSize = 20; // Width of the terrain
    [SerializeField] private int zSize = 20; // Length of the terrain
    [SerializeField] private float bumpiness = 2f; // Strength of the Perlin noise
    [SerializeField] private float scale = 10f; // Scale of the noise
    [SerializeField] private Material terrainMaterial; // Material for the terrain
    [SerializeField] private float slopeBias=10;

    private Mesh mesh;
    private EnvironmentalInstancer environmentalInstancer;
    private Vector3[] vertices;
    private int[] triangles;

    private void Start()
    {
        GenerateMesh();
        ApplyMaterial();
        PopulateMesh();
    }

    public void GenerateMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        triangles = new int[xSize * zSize * 6];

        // Generate vertices using Perlin noise
        float xScaler = scale / xSize;
        float zScaler = scale / zSize;
        float slopBiasScaler = (xSize + slopeBias);
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * xScaler, z * zScaler) * bumpiness * (1f-(float)((x + slopeBias) / slopBiasScaler));
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        // Create triangles
        int vert = 0;
        int tris = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;

                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        // Apply to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals(); // Recalculate for smooth lighting
    }

    private void ApplyMaterial()
    {
        if (terrainMaterial != null)
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.material = terrainMaterial;
        }
    }

    // For visualization in the Unity Editor
    private void OnDrawGizmos()
    {
        if (vertices == null)
            return;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPosition = transform.TransformPoint(vertices[i]);
            Gizmos.DrawSphere(worldPosition, 0.1f);
        }
    }

    private void PopulateMesh()
    {
        environmentalInstancer = GetComponent<EnvironmentalInstancer>();
        environmentalInstancer.SetTerrainMesh(mesh);
        environmentalInstancer.GenerateAllInstances();
    }

    public void DeleteChunk()
    {
        Destroy(gameObject);
    }
}
