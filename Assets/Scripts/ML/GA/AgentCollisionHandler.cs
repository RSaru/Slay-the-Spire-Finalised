using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentCollisionHandler : MonoBehaviour
{
    void Start()
    {
        // Ignore collision between agents by setting the collision matrix at runtime
        Collider2D[] allAgents = FindObjectsOfType<Collider2D>();
        foreach (var agentCollider in allAgents)
        {
            if (agentCollider.gameObject != this.gameObject)
            {
                Physics2D.IgnoreCollision(agentCollider, GetComponent<Collider2D>());
            }
        }
    }
}
