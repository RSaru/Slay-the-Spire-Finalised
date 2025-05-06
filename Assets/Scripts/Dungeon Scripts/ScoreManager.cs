using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [Header("GA UI Texts")]
    [Tooltip("Displays the all-time total score for GA generations")]
    public Text totalScoreText;
    [Tooltip("Displays the score for the current generation (resets every generation)")]
    public Text generationScoreText;
    [Tooltip("Displays the highest score ever hit in any generation")]
    public Text highestGenerationScoreText;

    [Header("RL UI Texts")]
    [Tooltip("All-time cumulative score across RL episodes")]
    public Text overallScoreText;
    [Tooltip("Score for the current RL episode")]
    public Text episodeScoreText;
    [Tooltip("Highest single-episode score recorded")]
    public Text bestEpisodeText;

    [Header("Generation Timing")]
    [Tooltip("Length of each GA generation in seconds")]
    public float generationDuration = 120f;

    private int totalScore = 0;
    private int generationScore = 0;
    private int highestGenerationScore = 0;

    private int overallScore = 0;
    private int currentEpisodeScore = 0;
    private int bestEpisodeScore = 0;

    //initializes singleton instance
    void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }
    }

    //sets up ui and schedules generation score reset
    void Start()
    {
        UpdateUI();
        InvokeRepeating(nameof(ResetGenerationScore), generationDuration, generationDuration);
    }

    //adds score for ga generations and rl episodes then updates ui
    public void AddScore(int amount)
    {
        totalScore += amount;
        generationScore += amount;
        currentEpisodeScore += amount;
        UpdateUI();
    }

    //resets generation score and records highest generation score
    public void ResetGenerationScore()
    {
        if (generationScore > highestGenerationScore)
            highestGenerationScore = generationScore;
        generationScore = 0;
        UpdateUI();
    }

    //banks rl episode score into overall score and resets episode score
    public void ResetAllScores()
    {
        overallScore += currentEpisodeScore;
        if (currentEpisodeScore > bestEpisodeScore)
            bestEpisodeScore = currentEpisodeScore;
        currentEpisodeScore = 0;
        UpdateUI();
    }

    //updates all ui text fields with current score values
    void UpdateUI()
    {
        if (totalScoreText != null)
            totalScoreText.text = "Total Score: " + totalScore;
        if (generationScoreText != null)
            generationScoreText.text = "Generation Score: " + generationScore;
        if (highestGenerationScoreText != null)
            highestGenerationScoreText.text = "Best Gen Score: " + highestGenerationScore;

        if (overallScoreText != null)
            overallScoreText.text = "Total Score: " + totalScore;
        if (episodeScoreText != null)
            episodeScoreText.text = "Episode Score: " + currentEpisodeScore;
        if (bestEpisodeText != null)
            bestEpisodeText.text = "Best Episode: " + bestEpisodeScore;
    }

    public int TotalScore => totalScore;
    public int GenerationScore => generationScore;
    public int HighestGenerationScore => highestGenerationScore;
    public int OverallScore => overallScore;
    public int CurrentEpisodeScore => currentEpisodeScore;
    public int BestEpisodeScore => bestEpisodeScore;
}
