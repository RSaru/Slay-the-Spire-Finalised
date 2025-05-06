using UnityEngine;

public class RLSpawnerScript : MonoBehaviour
{
    [Header("RL Settings")]
    public GameObject rlAgentPrefab;
    public Transform spawnPoint;

    //spawns an rl agent and assigns its spawn point
    void Start()
    {
        var go = Instantiate(rlAgentPrefab, spawnPoint.position, Quaternion.identity);
        var agent = go.GetComponent<RLAgent>();
        agent.spawnPoint = spawnPoint;
    }
}
