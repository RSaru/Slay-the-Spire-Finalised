using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections.Generic;

/*
public static class GeneticAlgorithm2
{




    public static List<Genome> population = new List<Genome>();
    private static int populationSize = 10;
    private static float mutationRate = 0.1f;

    public static void InitializePopulation()
    {
        population.Clear();
        for (int i = 0; i < populationSize; i++)
        {
            population.Add(GetRandomGenome());
        }
    }

    public static Genome GetRandomGenome()
    {
        return new Genome
        {
            movementAggression = Random.Range(0f, 1f),
            trapAvoidance = Random.Range(0f, 1f),
            //pathfindingWeight = Random.Range(0f, 1f),
            treasurePriority = Random.Range(0f, 1f),
        };
    }

    public static List<Genome> EvolvePopulation(List<float> fitnessScores)
    {
        List<Genome> newGeneration = new List<Genome>();

        for (int i = 0; i < populationSize; i++)
        {
            Genome parent1 = TournamentSelection(fitnessScores);
            Genome parent2 = TournamentSelection(fitnessScores);

            //Genome child = Crossover(parent1, parent2);
            //Mutate(ref child);

            //newGeneration.Add(child);
        }

        return newGeneration;
    }

    private static Genome TournamentSelection(List<float> fitnessScores)
    {
        int bestIndex = Random.Range(0, population.Count);
        for (int i = 1; i < 3; i++)
        {
            int index = Random.Range(0, population.Count);
            if (fitnessScores[index] > fitnessScores[bestIndex])
            {
                bestIndex = index;
            }
        }
        return population[bestIndex];
    }


    private static void Mutate(ref Genome genome)
    {
        if (Random.value < mutationRate) genome.movementAggression += Random.Range(-0.1f, 0.1f);
        if (Random.value < mutationRate) genome.trapAvoidance += Random.Range(-0.1f, 0.1f);
        //if (Random.value < mutationRate) genome.pathfindingWeight += Random.Range(-0.1f, 0.1f);
        if (Random.value < mutationRate) genome.treasurePriority += Random.Range(-0.1f, 0.1f);
    }
}

*/