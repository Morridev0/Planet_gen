using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlanetGenerator : MonoBehaviour
{
    [Header("Shape")]
    public float noiseScale = 2f;
    public int octaves = 4;
    public float persistence = 0.5f;
    public float lacunarity = 2f;

    public float heightMultiplier = 0.3f;
    public float seaLevel = 0.35f;

    [Header("Color")]
    public Color oceanColor = new Color(0f, 0.2f, 0.6f);
    public Color beachColor = new Color(0.9f, 0.85f, 0.6f);
    public Color landColor = new Color(0.15f, 0.6f, 0.25f);
    public Color mountainColor = Color.gray;
    public Color snowColor = Color.white;

    [Header("Runtime Update")]
    public bool autoUpdate = true;

    private Mesh mesh;
    private Vector3[] vertices;
    private Vector3[] normals;
    private Color[] colors;

    // Variables para detectar cambios
    private float lastNoiseScale;
    private float lastHeightMultiplier;
    private float lastSeaLevel;
    private int lastOctaves;
    private float lastPersistence;
    private float lastLacunarity;

    void Start()
    {
        InitializeMesh();
        GeneratePlanet();
        ApplyMesh();
        CacheParameters();
    }

    void Update()
    {
        if (!autoUpdate) return;

        if (ParametersChanged())
        {
            GeneratePlanet();
            ApplyMesh();
            CacheParameters();
        }
    }

    void InitializeMesh()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        mesh = Instantiate(mf.sharedMesh);
        mf.mesh = mesh;

        vertices = mesh.vertices;
        normals = mesh.normals;
        colors = new Color[vertices.Length];
    }

    void ApplyMesh()
    {
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    bool ParametersChanged()
    {
        return noiseScale != lastNoiseScale
            || heightMultiplier != lastHeightMultiplier
            || seaLevel != lastSeaLevel
            || octaves != lastOctaves
            || persistence != lastPersistence
            || lacunarity != lastLacunarity;
    }

    void CacheParameters()
    {
        lastNoiseScale = noiseScale;
        lastHeightMultiplier = heightMultiplier;
        lastSeaLevel = seaLevel;
        lastOctaves = octaves;
        lastPersistence = persistence;
        lastLacunarity = lacunarity;
    }

    void GeneratePlanet()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 normal = normals[i].normalized;

            float height01 = FractalNoise(normal);

            // --- RELIEVE ---
            float elevation = Mathf.Max(0f, height01 - seaLevel);
            vertices[i] = normal + normal * elevation * heightMultiplier;

            // --- COLOR ---
            colors[i] = GetBiomeColor(height01, normal.y);
        }
    }

    float FractalNoise(Vector3 p)
    {
        float value = 0f;
        float amplitude = 1f;
        float frequency = noiseScale;
        float maxValue = 0f;

        for (int i = 0; i < octaves; i++)
        {
            // Ajuste simple para distribuir mejor el ruido en la esfera
            float n = Mathf.PerlinNoise(
                p.x * frequency + 100f * i,
                p.y * frequency + 100f * i
            );

            value += n * amplitude;
            maxValue += amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return value / maxValue; // normalizado 0–1
    }

    Color GetBiomeColor(float height, float latitude)
    {
        if (height < seaLevel)
            return oceanColor;

        if (height < seaLevel + 0.05f)
            return beachColor;

        // nieve por altura o polos
        if (height > 0.8f || Mathf.Abs(latitude) > 0.75f)
            return snowColor;

        if (height > 0.6f)
            return mountainColor;

        return landColor;
    }
}
