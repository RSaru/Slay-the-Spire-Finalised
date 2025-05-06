using UnityEngine;
using System.IO;

public class GAHeatmap : MonoBehaviour
{
    public int gridWidth = 100;
    public int gridHeight = 100;
    public float worldWidth = 50f;
    public float worldHeight = 50f;
    public int maxCountThreshold = 100;

    private int[,] visitCounts;
    private int[,] cumulativeVisitCounts;

    private Texture2D heatmapTexture;

    public float updateInterval = 0.5f;
    private float updateTimer = 0f;

    //initializes grids and heatmap texture
    void Start()
    {
        visitCounts = new int[gridWidth, gridHeight];
        cumulativeVisitCounts = new int[gridWidth, gridHeight];
        heatmapTexture = new Texture2D(gridWidth, gridHeight, TextureFormat.RGBA32, false);
        heatmapTexture.filterMode = FilterMode.Point;
        UpdateHeatmapTextureFromArray(visitCounts, heatmapTexture);
    }

    //records agent position into heatmap grids
    public void RecordPosition(Vector3 position)
    {
        float halfWidth = worldWidth / 2f;
        float halfHeight = worldHeight / 2f;
        float normX = Mathf.InverseLerp(-halfWidth, halfWidth, position.x);
        float normY = Mathf.InverseLerp(-halfHeight, halfHeight, position.y);
        int gridX = Mathf.Clamp((int)(normX * gridWidth), 0, gridWidth - 1);
        int gridY = Mathf.Clamp((int)(normY * gridHeight), 0, gridHeight - 1);

        visitCounts[gridX, gridY]++;
        cumulativeVisitCounts[gridX, gridY]++;
    }

    //updates timer for optional periodic actions
    void Update()
    {
        updateTimer += Time.deltaTime;
        if (updateTimer >= updateInterval)
        {
            updateTimer = 0f;
        }
    }

    //converts visit count array into texture pixels
    void UpdateHeatmapTextureFromArray(int[,] counts, Texture2D texture)
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                float intensity = Mathf.Clamp01((float)counts[x, y] / maxCountThreshold);
                Color color = Color.Lerp(Color.blue, Color.red, intensity);
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
    }

    //exports current generation heatmap as png file
    public void ExportGenerationHeatmap(int generationNumber)
    {
        UpdateHeatmapTextureFromArray(visitCounts, heatmapTexture);
        byte[] bytes = heatmapTexture.EncodeToPNG();
        string filePath = Path.Combine(Application.persistentDataPath, $"Heatmap_Gen{generationNumber}.png");
        File.WriteAllBytes(filePath, bytes);
        Debug.Log("Exported generation heatmap to: " + filePath);
    }

    //generates texture from cumulative visit data
    public Texture2D GenerateCumulativeHeatmapTexture()
    {
        Texture2D cumulativeTexture = new Texture2D(gridWidth, gridHeight, TextureFormat.RGBA32, false);
        cumulativeTexture.filterMode = FilterMode.Point;
        UpdateHeatmapTextureFromArray(cumulativeVisitCounts, cumulativeTexture);
        return cumulativeTexture;
    }

    //exports cumulative heatmap as png file
    public void ExportCumulativeHeatmap()
    {
        Texture2D cumTex = GenerateCumulativeHeatmapTexture();
        byte[] bytes = cumTex.EncodeToPNG();
        string filePath = Path.Combine(Application.persistentDataPath, "Heatmap_Final.png");
        File.WriteAllBytes(filePath, bytes);
        Debug.Log("Exported cumulative heatmap to: " + filePath);
    }

    //resets current generation visit counts
    public void ResetGeneration()
    {
        visitCounts = new int[gridWidth, gridHeight];
    }
}
