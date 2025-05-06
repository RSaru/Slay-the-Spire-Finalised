using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class geneticAgent : Agent
{
    public Genome genome;
    private float fitnessScore;

    private PaladinHealthScript paladinHealthScript;
    private GAHeatmap heatmapManager;

    private int coinsCollected = 0;
    private bool reachedGoal = false;
    private int damageTaken = 0;
    public float moveSpeed = 3f;
    private float totalReward = 0f;
    public float sensorRadius = 0.5f;
    public LayerMask walkableLayer;
    public LayerMask unwalkableLayer;
    private Vector2 previousPosition;
    private float distanceMoved = 0f;
    private int wallHits = 0;

    private int previousHealth;
    private Rigidbody2D rb;

    private float stuckTimer = 0f;
    private float maxStuckTime = 2f;
    private float stuckMoveThreshold = 0.05f;

    private Vector2 startingPosition;
    private float jitterPenalty = 0f;
    private float jitterThreshold = 0.2f;

    private Vector2 spawnPosition;
    public float spawnBonusMultiplier = 5f;

    //initializes references and positions
    void Start()
    {
        paladinHealthScript = GetComponent<PaladinHealthScript>();
        rb = GetComponent<Rigidbody2D>();

        if (paladinHealthScript != null)
            previousHealth = paladinHealthScript.currentHealth;

        if (rb != null)
        {
            startingPosition = rb.position;
            previousPosition = rb.position;
            spawnPosition = rb.position;
        }

        heatmapManager = FindObjectOfType<GAHeatmap>();
    }

    //updates damageTaken based on health changes
    void Update()
    {
        if (paladinHealthScript != null)
        {
            if (paladinHealthScript.currentHealth < previousHealth)
            {
                damageTaken += previousHealth - paladinHealthScript.currentHealth;
            }
            previousHealth = paladinHealthScript.currentHealth;
        }
    }

    //marks agent as having reached its goal
    public void ReachGoal() => reachedGoal = true;

    //calculates and returns the fitness score of the agent
    public float GetFitnessScore()
    {
        float score = 0f;
        score += genome.treasurePriority * GetTreasureScore();
        score += GetHealth();
        score += coinsCollected * 50;
        score += reachedGoal ? 500f : -25f;
        score -= damageTaken * 2;
        if (paladinHealthScript.currentHealth <= 0) score -= 50f;
        score += distanceMoved * 5f;
        score -= wallHits * 10f;
        score -= jitterPenalty;
        score += GetSpawnDistanceBonus();
        return score;
    }

    //computes normalized treasure proximity score
    private float GetTreasureScore()
    {
        float dist = Vector3.Distance(transform.position, GetTreasurePosition());
        return Mathf.Clamp(1f - (dist / 20f), 0f, 1f);
    }

    //returns normalized health fraction
    private float GetHealth()
    {
        return paladinHealthScript == null
            ? 1f
            : (float)paladinHealthScript.currentHealth / paladinHealthScript.maxHealth;
    }

    //computes bonus based on spawn distance
    private float GetSpawnDistanceBonus()
    {
        float disp = Vector2.Distance(rb.position, spawnPosition);
        return disp * spawnBonusMultiplier;
    }

    //finds and returns the position of the closest treasure
    private Vector3 GetTreasurePosition()
    {
        var treasures = GameObject.FindGameObjectsWithTag("Treasure");
        if (treasures == null || treasures.Length == 0) return Vector3.zero;

        float bestDist = float.MaxValue;
        Vector3 bestPos = Vector3.zero;
        Vector3 myPos = transform.position;

        foreach (var t in treasures)
        {
            float d = Vector3.SqrMagnitude(t.transform.position - myPos);
            if (d < bestDist)
            {
                bestDist = d;
                bestPos = t.transform.position;
            }
        }

        return bestPos;
    }

    //initializes the agent for ml-agents with a random genome
    public override void Initialize()
    {
        Debug.Log("Agent Initialized");
        rb = GetComponent<Rigidbody2D>();
        genome = new Genome();
        genome.Initialize();

        paladinHealthScript = FindObjectOfType<PaladinHealthScript>();
        if (paladinHealthScript == null)
            Debug.LogError("PaladinHealthScript NOT FOUND!");

        if (rb != null)
        {
            startingPosition = rb.position;
            previousPosition = rb.position;
            spawnPosition = rb.position;
        }
    }

    //collects environment observations for the rl agent
    public override void CollectObservations(VectorSensor sensor)
    {
        var relT = GetTreasurePosition() - transform.position;
        sensor.AddObservation(relT.normalized);
        sensor.AddObservation(GetHealth());

        Vector3 trapDir;
        float trapDist;
        if (GetNearestTrap(out trapDir, out trapDist))
        {
            sensor.AddObservation(trapDir.normalized);
            sensor.AddObservation(1f - Mathf.Clamp01(trapDist / 5f));
        }
        else
        {
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(0f);
        }

        Vector3 coinDir;
        float coinDist;
        if (GetNearestCoin(out coinDir, out coinDist))
        {
            sensor.AddObservation(coinDir.normalized);
            sensor.AddObservation(1f - coinDist);
        }
        else
        {
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(0f);
        }

        AddWalkabilityObservations(sensor);
    }

    //finds nearest trap direction and normalized distance
    bool GetNearestTrap(out Vector3 dir, out float normDist)
    {
        float radius = 5f;
        var cols = Physics2D.OverlapCircleAll(transform.position, radius);
        float best = radius + 1f;
        Vector3 bestDir = Vector3.zero;
        bool found = false;

        foreach (var c in cols)
        {
            if (c.CompareTag("Trap"))
            {
                float d = Vector3.Distance(transform.position, c.transform.position);
                if (d < best)
                {
                    best = d;
                    bestDir = c.transform.position - transform.position;
                    found = true;
                }
            }
        }

        dir = bestDir;
        normDist = Mathf.Clamp01(best / radius);
        return found;
    }

    //finds nearest coin direction and normalized distance
    bool GetNearestCoin(out Vector3 dir, out float normDist)
    {
        float maxD = 20f;
        var coins = GameObject.FindGameObjectsWithTag("Coin");
        float best = maxD + 1f;
        Vector3 bestDir = Vector3.zero;
        bool found = false;

        foreach (var coin in coins)
        {
            float d = Vector3.Distance(transform.position, coin.transform.position);
            if (d < best)
            {
                best = d;
                bestDir = coin.transform.position - transform.position;
                found = true;
            }
        }

        dir = bestDir;
        normDist = Mathf.Clamp01(best / maxD);
        return found;
    }

    //adds walkability sensor observations for neighboring cells
    private void AddWalkabilityObservations(VectorSensor sensor)
    {
        for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
            {
                var checkPos = (Vector2)transform.position + new Vector2(x, y) * sensorRadius;
                var hit = Physics2D.OverlapCircle(checkPos, sensorRadius * 0.4f, walkableLayer | unwalkableLayer);
                if (hit == null) sensor.AddObservation(0f);
                else if (((1 << hit.gameObject.layer) & walkableLayer) != 0) sensor.AddObservation(1f);
                else if (((1 << hit.gameObject.layer) & unwalkableLayer) != 0) sensor.AddObservation(-1f);
                else sensor.AddObservation(0f);
            }
    }

    //handles received actions (unused in this implementation)
    public override void OnActionReceived(ActionBuffers actions) { }

    //moves the agent according to action values and genome biases
    void MoveAgent(float moveX, float moveY)
    {
        if (rb == null) return;

        Vector2 baseDir = new Vector2(
            moveX * genome.treasurePriority + genome.moveXBias,
            moveY * genome.treasurePriority + genome.moveYBias
        ).normalized;

        Vector2 finalDir = GetClearDirection(baseDir);
        rb.velocity = finalDir * moveSpeed;
        Debug.DrawRay(transform.position, finalDir, Color.green);
    }

    //checks if a direction is clear of obstacles
    bool IsDirectionClear(Vector2 dir)
    {
        return Physics2D.Raycast(transform.position, dir, 1f, unwalkableLayer).collider == null;
    }

    //returns a clear movement direction by avoiding walls
    Vector2 GetClearDirection(Vector2 baseDir)
    {
        if (IsDirectionClear(baseDir)) return baseDir;
        float maxOff = Mathf.Lerp(15f, 90f, genome.wallAvoidance);
        for (float a = 15f; a <= maxOff; a += 15f)
        {
            var r1 = RotateVector(baseDir, a);
            if (IsDirectionClear(r1)) return r1;
            var r2 = RotateVector(baseDir, -a);
            if (IsDirectionClear(r2)) return r2;
        }
        return -baseDir;
    }

    //rotates a vector by a given angle
    Vector2 RotateVector(Vector2 v, float deg)
    {
        float rad = deg * Mathf.Deg2Rad;
        return new Vector2(
            v.x * Mathf.Cos(rad) - v.y * Mathf.Sin(rad),
            v.x * Mathf.Sin(rad) + v.y * Mathf.Cos(rad)
        ).normalized;
    }

    //handles treasure collection, rewards, and agent deactivation
    public void CollectTreasure(int rewardValue)
    {
        if (hasCollectedTreasure) return;
        hasCollectedTreasure = true;
        coinsCollected += 1;
        AddReward(rewardValue);
        ReachGoal();

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (rb != null) rb.velocity = Vector2.zero;
        rb.simulated = false;

        this.enabled = false;
    }

    //applies movement, exploration logging, and stuck detection each physics step
    void FixedUpdate()
    {
        var dir = (GetTreasurePosition() - transform.position).normalized;
        MoveAgent(dir.x, dir.y);

        float moved = Vector2.Distance(rb.position, previousPosition);
        distanceMoved += moved;

        float netDisp = Vector2.Distance(rb.position, startingPosition);
        float ratio = netDisp / (distanceMoved + 0.001f);
        jitterPenalty = ratio < jitterThreshold
            ? 50f * (jitterThreshold - ratio)
            : 0f;

        if (moved < stuckMoveThreshold)
            stuckTimer += Time.fixedDeltaTime;
        else
            stuckTimer = 0f;

        if (stuckTimer > maxStuckTime)
        {
            rb.velocity = Random.insideUnitCircle.normalized * moveSpeed;
            stuckTimer = 0f;
            wallHits++;
            Debug.Log("Forced reorientation due to being stuck");
        }

        previousPosition = rb.position;

        heatmapManager?.RecordPosition(transform.position);
    }

    //draws sensor gizmos in the editor when selected
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.cyan;
        for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
            {
                var pos = (Vector2)transform.position + new Vector2(x, y) * sensorRadius;
                Gizmos.DrawWireSphere(pos, sensorRadius * 0.4f);
            }
    }

    public bool hasCollectedTreasure = false;
}
