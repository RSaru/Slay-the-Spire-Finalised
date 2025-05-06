using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
/*
public class DungeonAgent : Agent
{
    PaladinHealthScript paladinHealth;

    public Genome genome;  // Genome controlling this agent
    private float reward = 0f;
    private Rigidbody2D rb;
    public float moveSpeed = 5f; // Default movement speed

    private float GetTreasureScore()
    {
        float dist = Vector3.Distance(transform.position, GetTreasurePosition());
        return Mathf.Clamp(1f - (dist / 20f), 0f, 1f); // Higher score for closer treasure
    }

    private float GetTrapAvoidanceScore()
    {
        return IsNearTrap() ? -0.5f : 0.2f; // Less harsh penalty, slight bonus for being safe
    }

    private float GetMovementScore()
    {
        return 0.1f; // Reward for movement
    }

    private float GetHealth()
    {
        if (paladinHealth == null)
        {
            Debug.LogError("paladinHealth is NULL! Returning default health.");
            return 1f;  // Default to full health if missing
        }
        return paladinHealth.currentHealth / paladinHealth.maxHealth;
    }

    private bool IsNearTrap()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 3f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Trap"))
                return true;
        }
        return false;
    }

    private Vector3 GetTreasurePosition()
    {
        GameObject treasure = GameObject.FindGameObjectWithTag("Treasure");
        return treasure ? treasure.transform.position : Vector3.zero;
    }

    public override void Initialize()
    {
        Debug.Log("Agent Initialized");

        rb = GetComponent<Rigidbody2D>();
        genome = GeneticAlgorithm.GetRandomGenome();
        paladinHealth = FindObjectOfType<PaladinHealthScript>();

        if (paladinHealth == null)
        {
            Debug.LogError("PaladinHealthScript NOT FOUND! Check if it's assigned.");
        }
        else
        {
            Debug.Log("PaladinHealthScript found.");
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 treasurePos = GetTreasurePosition();
        Vector3 relativeTreasurePos = treasurePos - transform.position;

        sensor.AddObservation(relativeTreasurePos.normalized);
        sensor.AddObservation(GetHealth());
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Debug.Log("OnActionReceived CALLED!");

        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        Debug.Log($"Actions Received: X={moveX}, Y={moveY}");

        MoveAgent(moveX, moveY);
        reward += CalculateReward();
        SetReward(reward);
    }

    void Update()
    {
        if (Time.frameCount % 5 == 0)
        {
            Debug.Log("Requesting Decision...");
            RequestDecision();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
        Debug.Log($"Heuristic Controls: X={continuousActions[0]}, Y={continuousActions[1]}");
    }

    private float CalculateReward()
    {
        float fitness = 0f;
        fitness += genome.treasurePriority * GetTreasureScore();
        fitness += genome.trapAvoidance * GetTrapAvoidanceScore();
        fitness += genome.movementAggression * GetMovementScore();
        return fitness;
    }

    private Vector3 spawnPoint = new Vector3(-19.82f, 8.33f, 0.019f); // Change to the exact coordinates of the white circle

    public override void OnEpisodeBegin()
    {
        Debug.Log("New Episode Started");

        // Reset health
        if (paladinHealth != null)
        {
            paladinHealth.currentHealth = paladinHealth.maxHealth;
            paladinHealth.UpdateHealthUI();
        }

        // Reset agent position to the fixed spawn point
        transform.position = spawnPoint;
    }



    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.2f); // Show agent position

        // Draw line to treasure
        Vector3 treasurePos = GetTreasurePosition();
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, treasurePos);
    }


    private void MoveAgent(float x, float y)
    {
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D is missing! Can't move.");
            return;
        }

        Vector2 move = new Vector2(x, y) * moveSpeed;
        rb.velocity = move;
    }
}
*/