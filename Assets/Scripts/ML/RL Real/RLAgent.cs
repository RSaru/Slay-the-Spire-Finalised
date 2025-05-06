using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.IO;
using System.Collections.Generic;

public class RLAgent : Agent
{
    [Header("spawn settings")]
    public Transform spawnPoint;

    [Header("heatmap (assign in inspector)")]
    public RLHeatmap heatmap;

    private static readonly string _csvDir = Application.dataPath + "/RLCSVs";
    private static readonly string _csvPath = _csvDir + "/RLMetrics.csv";
    private static bool _headerWritten = false;
    private static int _episodeCounter = 0;

    [Header("exploration settings")]
    public float minMoveThreshold = 0.05f;
    public float idlePenalty = -0.005f;
    public float moveRewardScale = 0.02f;

    [Header("grid-visit reward")]
    public float cellVisitReward = 0.3f;
    public float cellSize = 1f;
    private HashSet<Vector2Int> visitedCells = new HashSet<Vector2Int>();

    [Header("lack-of-progress settings")]
    public int maxIdleSteps = 200;
    private int idleStepCount;
    private float prevGoalDist;

    private GameObject[] _allCoins;

    [Header("refs")]
    public Timer timer;
    public RLStatsLogger statsLogger;

    private PaladinHealthScript healthScript;
    private Rigidbody2D rb;
    private Vector2 prevPos;
    private List<Vector2> _path = new List<Vector2>();

    //initializes components, csv directories, and warns if heatmap not assigned
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        healthScript = GetComponent<PaladinHealthScript>();
        prevPos = rb.position;

        _allCoins = GameObject.FindGameObjectsWithTag("Coin");

        if (!Directory.Exists(_csvDir))
            Directory.CreateDirectory(_csvDir);

        if (heatmap == null)
            Debug.LogWarning("RLHeatmap not assigned on RLAgent.");
    }

    //resets environment, heatmap, stats and lack-of-progress tracker at episode start
    public override void OnEpisodeBegin()
    {
        ScoreManager.instance?.ResetAllScores();
        timer?.ResetTimer();

        transform.position = spawnPoint.position;
        rb.velocity = Vector2.zero;
        healthScript.currentHealth = healthScript.maxHealth;

        foreach (var coin in _allCoins)
            if (coin != null) coin.SetActive(true);

        prevPos = rb.position;
        visitedCells.Clear();
        _path.Clear();

        var goal = GameObject.FindWithTag("Goal")?.GetComponent<GoalItem>();
        goal?.ResetState();

        heatmap?.ResetHeatmap();

        idleStepCount = 0;
        var treasure = GameObject.FindGameObjectWithTag("Treasure");
        prevGoalDist = treasure != null
            ? Vector2.Distance(transform.position, treasure.transform.position)
            : float.MaxValue;
    }

    //collects observations: treasure direction, health, and walkability grid
    public override void CollectObservations(VectorSensor sensor)
    {
        var treasure = GameObject.FindGameObjectWithTag("Treasure");
        Vector3 toT = treasure != null
            ? (treasure.transform.position - transform.position).normalized
            : Vector3.zero;
        sensor.AddObservation(toT);

        sensor.AddObservation((float)healthScript.currentHealth / healthScript.maxHealth);

        const float r = 0.5f;
        LayerMask walkable = LayerMask.GetMask("Walkable");
        LayerMask unwalkable = LayerMask.GetMask("Unwalkable");
        for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
            {
                Vector2 c = (Vector2)transform.position + new Vector2(x, y) * r;
                var hit = Physics2D.OverlapCircle(c, r * 0.4f, walkable | unwalkable);
                if (hit == null) sensor.AddObservation(0f);
                else if (((1 << hit.gameObject.layer) & walkable) != 0) sensor.AddObservation(1f);
                else sensor.AddObservation(-1f);
            }
    }

    //processes actions, applies movement, rewards, heatmap recording, and lack-of-progress check
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (healthScript.currentHealth <= 0)
        {
            timer?.StopTimer();
            timer?.RecordGoalTime();

            float t = timer != null ? timer.GetElapsedTime() : 0f;
            float rew = GetCumulativeReward();
            float overall = ScoreManager.instance?.OverallScore ?? 0f;

            if (!_headerWritten)
            {
                File.WriteAllText(_csvPath, "Episode,Time,Reward,OverallScore\n");
                _headerWritten = true;
            }

            _episodeCounter++;
            File.AppendAllText(_csvPath,
                $"{_episodeCounter},{t:F2},{rew:F2},{overall:F2}\n");

            var pathCsv = Path.Combine(_csvDir, $"Path_{_episodeCounter}.csv");
            using (var sw = new StreamWriter(pathCsv, false))
            {
                sw.WriteLine("x,y");
                foreach (var p in _path) sw.WriteLine($"{p.x:F2},{p.y:F2}");
            }
            _path.Clear();

            heatmap?.ExportEpisodeHeatmap(_episodeCounter);

            AddReward(-3f);
            EndEpisode();
            return;
        }

        Vector2 oldPos = rb.position;
        Vector2 dir = new Vector2(
            actions.ContinuousActions[0],
            actions.ContinuousActions[1]
        ).normalized;
        rb.velocity = dir * 3f;
        Vector2 newPos = rb.position;

        heatmap?.RecordPosition(transform.position);
        _path.Add(newPos);

        float moved = Vector2.Distance(oldPos, newPos);
        if (moved < minMoveThreshold) AddReward(idlePenalty);
        else AddReward(moveRewardScale * moved);

        prevPos = newPos;

        var cell = new Vector2Int(
            Mathf.FloorToInt(newPos.x / cellSize),
            Mathf.FloorToInt(newPos.y / cellSize)
        );
        if (visitedCells.Add(cell)) AddReward(cellVisitReward);

        var treasureObj = GameObject.FindGameObjectWithTag("Treasure");
        if (treasureObj != null)
        {
            float distToGoal = Vector2.Distance(transform.position, treasureObj.transform.position);
            if (distToGoal < prevGoalDist)
            {
                idleStepCount = 0;
                prevGoalDist = distToGoal;
            }
            else
            {
                idleStepCount++;
                if (idleStepCount > maxIdleSteps)
                {
                    AddReward(-0.1f);
                    EndEpisode();
                }
            }
        }
    }

    //provides heuristic controls for manual testing
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var c = actionsOut.ContinuousActions;
        c[0] = Input.GetAxis("Horizontal");
        c[1] = Input.GetAxis("Vertical");
    }

    //handles collisions with coins, treasure, and traps
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Coin"))
        {
            AddReward(+1f);
            ScoreManager.instance?.AddScore(0);
            col.gameObject.SetActive(false);
        }
        else if (col.CompareTag("Treasure"))
        {
            AddReward(+5f);
            ScoreManager.instance?.AddScore(500);
            timer?.StopTimer();
            timer?.RecordGoalTime();

            heatmap?.ExportEpisodeHeatmap(_episodeCounter);

            EndEpisode();
        }
        else if (col.CompareTag("Trap"))
        {
            healthScript.TakeDamage(1);
            AddReward(-1.5f);
        }
    }
}
