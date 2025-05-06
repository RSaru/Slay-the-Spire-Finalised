using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm
{
    //selects parent genomes based on fitness-proportionate selection
    public static List<Genome> SelectParents(List<Genome> population, List<float> fitnessScores)
    {
        var selected = new List<Genome>();
        float totalFitness = 0f;
        foreach (var f in fitnessScores) totalFitness += f;

        for (int i = 0; i < population.Count; i++)
        {
            float pick = Random.Range(0f, totalFitness);
            float running = 0f;
            foreach (var genome in population)
            {
                running += fitnessScores[population.IndexOf(genome)];
                if (running >= pick)
                {
                    selected.Add(genome);
                    break;
                }
            }
        }
        return selected;
    }

    //performs crossover between two parent genomes
    public static Genome Crossover(Genome p1, Genome p2)
    {
        return Genome.Crossover(p1, p2);
    }

    //mutates genome using its instance mutation method
    public static void Mutate(ref Genome genome, float mutationRate = 0.05f)
    {
        genome.Mutate(mutationRate, 1.0f);
    }

    //generates a new random genome
    public static Genome GetRandomGenome() => new Genome();

    //evolves a new population through elitism, selection, crossover, and mutation
    public static List<Genome> EvolvePopulation(List<float> fitnessScores, List<Genome> population, float mutationRate = 0.05f, int eliteCount = 3)
    {
        var newGen = new List<Genome>();
        var scored = new List<(Genome g, float f)>();
        for (int i = 0; i < population.Count; i++)
            scored.Add((population[i], fitnessScores[i]));

        scored.Sort((a, b) => b.f.CompareTo(a.f));

        for (int i = 0; i < eliteCount && i < scored.Count; i++)
            newGen.Add(scored[i].g);

        var parents = SelectParents(population, fitnessScores);
        while (newGen.Count < population.Count)
        {
            var p1 = parents[Random.Range(0, parents.Count)];
            var p2 = parents[Random.Range(0, parents.Count)];
            var child = Crossover(p1, p2);
            Mutate(ref child, mutationRate);
            newGen.Add(child);
        }
        return newGen;
    }
}
