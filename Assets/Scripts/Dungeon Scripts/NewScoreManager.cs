using UnityEngine;
using UnityEngine.UI;

public class NewScoreManager : MonoBehaviour
{
    public static NewScoreManager Instance { get; private set; }

    [Header("UI")]
    [Tooltip("Drag the UI Text that displays the score here")]
    [SerializeField] private Text scoreText;

    [Header("Format")]
    [Tooltip("Prefix shown before the numeric score")]
    [SerializeField] private string prefix = "Score: ";

    private int currentScore = 0;

    //initializes singleton instance and checks for assigned ui text
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (scoreText == null)
            Debug.LogError($"[NewScoreManager] No Text assigned!", this);
    }

    //updates ui text at start
    void Start()
    {
        UpdateUIText();
    }

    //adds amount to current score and updates ui
    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateUIText();
    }

    //sets current score to value and updates ui
    public void SetScore(int value)
    {
        currentScore = value;
        UpdateUIText();
    }

    //resets current score to zero and updates ui
    public void ResetScore()
    {
        currentScore = 0;
        UpdateUIText();
    }

    //updates the displayed ui text with current score
    private void UpdateUIText()
    {
        if (scoreText != null)
            scoreText.text = $"{prefix}{currentScore}";
    }
}
