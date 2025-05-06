using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class SquireAgent : Agent
{
    private Rigidbody2D rb;
    private SquireHealthScript healthScript;
    private float moveX, moveY;

    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        healthScript = GetComponent<SquireHealthScript>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observe the current health (normalized)
        sensor.AddObservation((float)healthScript.currentHealth / healthScript.maxHealth);

        // Observe if the agent is currently invincible
        //sensor.AddObservation(healthScript.IsInvincible());

        // Observe nearby traps (you may need a detection system)
        float nearestTrapDistance = GetNearestTrapDistance();
        sensor.AddObservation(nearestTrapDistance);

        // Observe goal position relative to agent
        Vector2 goalDirection = GetGoalDirection();
        sensor.AddObservation(goalDirection.x);
        sensor.AddObservation(goalDirection.y);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Debug.Log("OnActionReceived called.");
        moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        moveY = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        Debug.Log("Move X: " + moveX + ", Move Y: " + moveY);  // Debug log to check the values

        Vector2 movement = new Vector2(moveX, moveY).normalized * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // Reward for moving towards goal
        if (IsMovingTowardGoal(movement))
        {
            AddReward(0.01f);
        }

        // Penalize for taking damage
        if (healthScript.currentHealth < healthScript.maxHealth)
        {
            AddReward(-0.1f);
        }
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private float GetNearestTrapDistance()
    {
        GameObject[] traps = GameObject.FindGameObjectsWithTag("Trap");
        float minDistance = float.MaxValue;

        foreach (GameObject trap in traps)
        {
            float distance = Vector2.Distance(transform.position, trap.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
            }
        }

        return minDistance / 10f; // Normalize distance
    }

    private Vector2 GetGoalDirection()
    {
        GameObject goal = GameObject.FindGameObjectWithTag("Goal");
        if (goal != null)
        {
            return (goal.transform.position - transform.position).normalized;
        }
        return Vector2.zero;
    }

    private bool IsMovingTowardGoal(Vector2 movement)
    {
        Vector2 goalDirection = GetGoalDirection();
        return Vector2.Dot(movement.normalized, goalDirection) > 0.5f;
    }

    public void TakeDamage()
    {
        AddReward(-0.5f);
    }

    public void ReachedGoal()
    {
        AddReward(1.0f);
        EndEpisode();
    }
}
