using UnityEngine;
using System.IO;

public class RLHeatmap : MonoBehaviour
{
    [Header("Heatmap Settings")]
    public int gridWidth = 100;
    public int gridHeight = 100;
    public float worldWidth = 50f;
    public float worldHeight = 50f;
    public int maxCountThreshold = 100;

    private int[,] visitCounts;
    private Texture2D heatmapTexture;

    //initializes grid and heatmap texture
    void Awake()
    {
        visitCounts = new int[gridWidth, gridHeight];
        heatmapTexture = new Texture2D(gridWidth, gridHeight, TextureFormat.RGBA32, false);
        heatmapTexture.filterMode = FilterMode.Point;
    }

    //records the agent's world position into visitCounts
    public void RecordPosition(Vector3 position)
    {
        float halfW = worldWidth / 2f;
        float halfH = worldHeight / 2f;
        float normX = Mathf.InverseLerp(-halfW, halfW, position.x);
        float normY = Mathf.InverseLerp(-halfH, halfH, position.y);
        int x = Mathf.Clamp(Mathf.FloorToInt(normX * gridWidth), 0, gridWidth - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt(normY * gridHeight), 0, gridHeight - 1);
        visitCounts[x, y]++;
    }

    //resets visitCounts for a new episode
    public void ResetHeatmap()
    {
        for (int i = 0; i < gridWidth; i++)
            for (int j = 0; j < gridHeight; j++)
                visitCounts[i, j] = 0;
    }

    //generates heatmap texture from visitCounts and exports it as png
    public void ExportEpisodeHeatmap(int episodeNumber)
    {
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                float intensity = Mathf.Clamp01(visitCounts[i, j] / (float)maxCountThreshold);
                Color c = Color.Lerp(Color.blue, Color.red, intensity);
                heatmapTexture.SetPixel(i, j, c);
            }
        }
        heatmapTexture.Apply();

        byte[] bytes = heatmapTexture.EncodeToPNG();
        string fileName = $"Heatmap_Ep{episodeNumber:000}.png";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllBytes(path, bytes);
        Debug.Log($"Exported {fileName} to {path}");
    }
}
