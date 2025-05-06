using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections.Generic;

/*
public class GeneticGameManager : MonoBehaviour
{
    public List<DungeonAgent> agents;
    private List<float> fitnessScores = new List<float>();

    void Start()
    {
        GeneticAlgorithm.InitializePopulation();
        AssignGenomesToAgents();
    }

    void AssignGenomesToAgents()
    {
        for (int i = 0; i < agents.Count; i++)
        {
            agents[i].genome = GeneticAlgorithm.population[i];
        }
    }

    public void EndGeneration()
    {
        // Collect fitness scores
        fitnessScores.Clear();
        foreach (var agent in agents)
        {
            fitnessScores.Add(agent.GetCumulativeReward());
        }

        // Evolve the population
        GeneticAlgorithm.population = GeneticAlgorithm.EvolvePopulation(fitnessScores);

        // Reset agents with new genomes
        AssignGenomesToAgents();

        // Restart training
        foreach (var agent in agents)
        {
            agent.EndEpisode();
        }
    }
}
*/
