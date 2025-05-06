using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [Header("RL Episode UI")]
    public Text timerText;
    public Text longestEpisodeText;
    public Text fastestGoalText;

    [Header("GA Generation UI")]
    public Text generationText;
    public Text runTimerText;
    public Text fastestGenTimeText;

    [Header("Generation Settings")]
    [Tooltip("Length of each GA generation in seconds")] public float generationDuration = 120f;

    float elapsedTime = 0f;
    bool isRunningRL = false;
    float longestEpisodeTime = 0f;
    float fastestGoalTime = float.MaxValue;

    int generationCount = 1;
    float genTimer = 0f;
    float runTimer = 0f;
    bool isRunTimerRunning = true;
    float fastestGenRunTime = float.MaxValue;

    //initializes ui texts and starts both rl and ga timers
    void Start()
    {
        if (timerText != null) timerText.text = FormatTime(0f);
        if (longestEpisodeText != null) longestEpisodeText.text = "Longest: 00:00:000";
        if (fastestGoalText != null) fastestGoalText.text = "Fastest: 00:00:000";
        if (generationText != null) generationText.text = "Generation: 1";
        if (runTimerText != null) runTimerText.text = "Run Time: 00:00:000";
        if (fastestGenTimeText != null) fastestGenTimeText.text = "Best Gen Time: 00:00:000";
        isRunningRL = true;
        isRunTimerRunning = true;
    }

    //updates rl episode timer and ga generation timers each frame
    void Update()
    {
        float dt = Time.deltaTime;
        if (isRunningRL)
        {
            elapsedTime += dt;
            if (timerText != null) timerText.text = FormatTime(elapsedTime);
        }
        genTimer += dt;
        if (genTimer >= generationDuration)
        {
            generationCount++;
            genTimer -= generationDuration;
            runTimer = 0f;
            isRunTimerRunning = true;
            if (generationText != null) generationText.text = $"Generation: {generationCount}";
        }
        if (isRunTimerRunning)
        {
            runTimer += dt;
            if (runTimerText != null) runTimerText.text = $"Run Time: {FormatTime(runTimer)}";
        }
    }

    //stops rl timer and records the longest episode time
    public void StopTimer()
    {
        if (!isRunningRL) return;
        isRunningRL = false;
        if (elapsedTime > longestEpisodeTime)
        {
            longestEpisodeTime = elapsedTime;
            if (longestEpisodeText != null)
                longestEpisodeText.text = "Longest: " + FormatTime(longestEpisodeTime);
        }
    }

    //resets and starts the rl episode timer
    public void ResetTimer()
    {
        elapsedTime = 0f;
        isRunningRL = true;
        if (timerText != null)
            timerText.text = FormatTime(0f);
    }

    //records a new fastest rl goal time if beaten
    public void RecordGoalTime()
    {
        if (elapsedTime < fastestGoalTime)
        {
            fastestGoalTime = elapsedTime;
            if (fastestGoalText != null)
                fastestGoalText.text = "Fastest: " + FormatTime(fastestGoalTime);
        }
    }

    //records a new fastest ga generation run time if beaten
    public void RecordGenGoalTime()
    {
        if (runTimer < fastestGenRunTime)
        {
            fastestGenRunTime = runTimer;
            if (fastestGenTimeText != null)
                fastestGenTimeText.text = "Best Gen Time: " + FormatTime(fastestGenRunTime);
        }
    }

    //returns the current rl elapsed time
    public float GetElapsedTime() => elapsedTime;

    //formats a time value into mm:ss:msmsms
    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);
        return $"{minutes:00}:{seconds:00}:{milliseconds:000}";
    }
}
