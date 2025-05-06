using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RLStatsLogger : MonoBehaviour
{
    private string _filePath;
    private bool _headerWritten = false;
    private int _episodeCount = 0;

    //initializes file path for rl stats csv
    void Awake()
    {
        _filePath = Application.dataPath + "/RLStats.csv";
    }

    //logs episode data to csv and refreshes editor assets
    public void LogEpisode(float reward, float duration, int score)
    {
        if (!_headerWritten)
        {
            File.WriteAllText(_filePath, "Episode,TotalReward,Duration,Score\n");
            _headerWritten = true;
        }
        _episodeCount++;
        var line = $"{_episodeCount},{reward:F2},{duration:F2},{score}\n";
        File.AppendAllText(_filePath, line);

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }
}
