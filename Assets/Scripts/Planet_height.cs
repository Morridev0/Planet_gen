using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class PlanetHeight : MonoBehaviour
{
    public float noiseScale = 2f;
    public float heightMultiplier = 0.2f;
    public float seaLevel = 0.3f;

    private Mesh mesh;
    private Vector3[] vertices;
    private Vector3[] normals;

    void Start()
    {
        MeshFilter mf = GetComponent<MeshFilter>();

        // 🔑 CLONAMOS la malla
        mesh = Instantiate(mf.sharedMesh);
        mf.mesh = mesh;

        vertices = mesh.vertices;
        normals = mesh.normals;

        ApplyHeight();

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void ApplyHeight()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 normal = normals[i].normalized;

            float noise = Mathf.PerlinNoise(
                normal.x * noiseScale,
                normal.y * noiseScale
            );

            float height = Mathf.Max(0f, noise - seaLevel);

            vertices[i] += normal * height * heightMultiplier;
        }
    }
}
