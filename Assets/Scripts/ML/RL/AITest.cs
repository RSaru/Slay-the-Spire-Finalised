using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class PaladinAgent : Agent
{
    public Transform target; // Example target (treasure, exit, enemy)
    public float moveSpeed = 2f;

    public override void Initialize()
    {
        // Initialization logic (reset agent, set environment, etc.)
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(target.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        transform.position += new Vector3(moveX, 0, moveZ) * moveSpeed * Time.deltaTime;

        // Reward structure (encourage survival & progress)
        float distance = Vector3.Distance(transform.position, target.position);
        AddReward(-distance * 0.01f); // Encourage moving towards the target
    }

    public override void OnEpisodeBegin()
    {

    }
}

