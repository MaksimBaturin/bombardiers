using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
public class TerrainGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    public int segments = 100;
    public float heightScale = 10f;
    public float terrainLength = 20f;
    
    [Header("Noise Settings")]
    public float noiseScale = 0.2f;
    public int octaves = 3;
    public float persistence = 0.5f;
    public float lacunarity = 2f;
    public int seed = 0; // Добавили параметр seed
    public bool randomSeed = true; // Опция для автоматического рандома
    
    [Header("Smoothing")]
    public bool useSmoothing = true;
    public int smoothPasses = 2;
    public float smoothStrength = 0.5f;
    
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    
    
    public void GenerateTerrain()
    {
        // Устанавливаем случайный seed если нужно
        if (randomSeed)
        {
            seed = Random.Range(0, 999999);
        }
        Random.InitState(seed); // Инициализируем генератор случайных чисел
        
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        float segmentLength = terrainLength / segments;
        int vertexCount = segments + 1;
        
        Vector3[] topVertices = new Vector3[vertexCount];
        Vector3[] allVertices = new Vector3[vertexCount * 2];
        
        for (int i = 0; i < vertexCount; i++)
        {
            float x = i * segmentLength;
            float height = CalculateFractalNoise(i, segments) * heightScale;
            
            topVertices[i] = new Vector3(x, height, 0);
            allVertices[i] = topVertices[i];
            allVertices[i + vertexCount] = new Vector3(x, -10f, 0);
        }

        if (useSmoothing)
        {
            SmoothTerrain(ref topVertices);
        }

        // Создаем меш
        List<int> triangles = new List<int>();
        for (int i = 0; i < segments; i++)
        {
            triangles.Add(i);
            triangles.Add(i + vertexCount);
            triangles.Add(i + 1);
            
            triangles.Add(i + 1);
            triangles.Add(i + vertexCount);
            triangles.Add(i + 1 + vertexCount);
        }

        mesh.vertices = allVertices;
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        UpdatePolygonCollider(topVertices);
        
        Debug.Log($"Terrain generated with seed: {seed}");
    }

    void UpdatePolygonCollider(Vector3[] topVertices)
    {
        PolygonCollider2D polyCollider = GetComponent<PolygonCollider2D>();
        Vector2[] points = new Vector2[topVertices.Length + 2];
        
        points[0] = new Vector2(topVertices[0].x, -10f);
        
        for (int i = 0; i < topVertices.Length; i++)
        {
            points[i + 1] = topVertices[i];
        }
        
        points[points.Length - 1] = new Vector2(topVertices[topVertices.Length - 1].x, -10f);
        
        polyCollider.SetPath(0, points);
    }
    
    float CalculateFractalNoise(float x, float maxX)
    {
        float normalizedX = x / maxX;
        float noiseValue = 0f;
        float frequency = 1f;
        float amplitude = 1f;
        
        for (int i = 0; i < octaves; i++)
        {
            // Добавляем seed к координате для уникальности
            float sample = normalizedX * noiseScale * frequency + seed;
            noiseValue += Mathf.PerlinNoise(sample, 0) * amplitude;
            
            amplitude *= persistence;
            frequency *= lacunarity;
        }
        
        return Mathf.Clamp01(noiseValue / octaves);
    }
    
    void SmoothTerrain(ref Vector3[] topVertices)
    {
        for (int pass = 0; pass < smoothPasses; pass++)
        {
            for (int i = 1; i < topVertices.Length - 1; i++)
            {
                float prevY = topVertices[i - 1].y;
                float nextY = topVertices[i + 1].y;
                float currentY = topVertices[i].y;
                
                topVertices[i].y = Mathf.Lerp(currentY, (prevY + nextY) * 0.5f, smoothStrength);
            }
        }
    }
    
    void OnDrawGizmos()
    {
        if (vertices == null || vertices.Length == 0) return;
        
        Gizmos.color = Color.red;
        for (int i = 0; i < vertices.Length / 2; i++)
        {
            Gizmos.DrawSphere(transform.position + vertices[i], 0.1f);
        }
    }
    
    // Добавляем кнопку для генерации нового террейна в инспекторе
    [ContextMenu("Generate New Terrain")]
    void GenerateNewTerrain()
    {
        randomSeed = true;
        GenerateTerrain();
    }
}