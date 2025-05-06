using System.Collections.Generic;
using UnityEngine;

public class MultiAgentManager : MonoBehaviour
{
    [Header("GA Settings")]
    public GameObject geneticAgentPrefab;
    public int gaPopulation = 10;
    public Transform gaSpawnPoint;

    [Header("RL Settings")]
    public GameObject rlAgentPrefab;
    public int rlPopulation = 10;
    public Transform rlSpawnPoint;

    private List<geneticAgent> gaAgents = new List<geneticAgent>();
    private List<RLAgent> rlAgents = new List<RLAgent>();

    //spawns GA and RL agents at their respective spawn points
    void Start()
    {
        for (int i = 0; i < gaPopulation; i++)
        {
            var go = Instantiate(geneticAgentPrefab, gaSpawnPoint.position, Quaternion.identity);
            var ag = go.GetComponent<geneticAgent>();
            ag.genome.Initialize();
            gaAgents.Add(ag);
        }

        for (int i = 0; i < rlPopulation; i++)
        {
            var go = Instantiate(rlAgentPrefab, rlSpawnPoint.position, Quaternion.identity);
            var ag = go.GetComponent<RLAgent>();
            ag.spawnPoint = rlSpawnPoint;
            rlAgents.Add(ag);
        }
    }

    //updates each frame to handle GA evolution and RL agent runtime
    void Update()
    {
    }
}
