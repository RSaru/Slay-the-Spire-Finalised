using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticManager : MonoBehaviour
{
    public int populationSize = 10;
    public GameObject agentPrefab;
    public Transform spawnPoint;
    public List<Genome> population;
    public List<geneticAgent> agents;
    public List<float> fitnessScores;
    int generationCount = 0;
    private List<float> maxFitnessHistory = new List<float>();
    private List<float> minFitnessHistory = new List<float>();
    private List<float> standardDeviationHistory = new List<float>();
    private List<float> averageFitnessHistory = new List<float>();
    private List<float> totalFitnessHistory = new List<float>();
    private float cumulativeTrainingScore = 0f;

    public float generationDuration = 60f;
    private float generationTimer;

    // initializes population list, spawns agents, and resets timer
    void Start()
    {
        population = new List<Genome>();
        agents = new List<geneticAgent>();
        fitnessScores = new List<float>();

        InitializePopulation();
        SpawnAgents();

        generationTimer = 0f;
    }

    // updates generation timer and ends generation when duration reached
    void Update()
    {
        generationTimer += Time.deltaTime;

        if (generationTimer >= generationDuration)
        {
            EndGeneration();
            generationTimer = 0f;
        }
    }

    // initializes new random population of genomes
    void InitializePopulation()
    {
        population.Clear();
        for (int i = 0; i < populationSize; i++)
        {
            population.Add(GeneticAlgorithm.GetRandomGenome());
        }
    }

    // instantiates agent prefabs and assigns genomes
    void SpawnAgents()
    {
        foreach (var genome in population)
        {
            GameObject agentObject = Instantiate(agentPrefab, spawnPoint.position, Quaternion.identity);
            geneticAgent agent = agentObject.GetComponent<geneticAgent>();
            if (agent != null)
            {
                agent.genome = genome;
                agent.genome.Initialize();
                agents.Add(agent);
            }
            else
            {
                Debug.LogError("geneticAgent script not found on instantiated prefab.");
            }
        }
    }

    // evaluates fitness, logs data, evolves population, and resets agents
    void EndGeneration()
    {
        EvaluateFitness();
        LogFitness(generationCount);

        GAHeatmap heatmap = FindObjectOfType<GAHeatmap>();
        if (heatmap != null)
        {
            heatmap.ExportGenerationHeatmap(generationCount);
            heatmap.ResetGeneration();
        }

        generationCount++;
        Debug.Log($"Population size: {population.Count}, Fitness Scores count: {fitnessScores.Count}");

        if (fitnessScores.Count != population.Count)
        {
            Debug.LogError("Fitness scores count does not match population size!");
        }

        float mutationRate = 0.01f;
        List<Genome> newGeneration = GeneticAlgorithm.EvolvePopulation(fitnessScores, population, mutationRate);

        ResetAgents(newGeneration);
        SaveFitnessDataToCSV();
    }

    // destroys old agents and spawns new ones with given genomes
    void ResetAgents(List<Genome> newGenomes)
    {
        foreach (var agent in agents)
        {
            if (agent != null)
            {
                Destroy(agent.gameObject);
            }
        }
        agents.Clear();

        for (int i = 0; i < newGenomes.Count; i++)
        {
            GameObject agentObject = Instantiate(agentPrefab, spawnPoint.position, Quaternion.identity);
            geneticAgent agent = agentObject.GetComponent<geneticAgent>();
            agent.genome = newGenomes[i];
            agents.Add(agent);
        }

        fitnessScores.Clear();
    }

    // calculates fitness scores for current agents
    void EvaluateFitness()
    {
        fitnessScores.Clear();
        int nullAgents = 0;

        foreach (var agent in agents)
        {
            if (agent != null)
            {
                fitnessScores.Add(agent.GetFitnessScore());
            }
            else
            {
                fitnessScores.Add(-100f);
                nullAgents++;
            }
        }

        while (fitnessScores.Count < population.Count)
        {
            fitnessScores.Add(-25f);
            nullAgents++;
        }

        if (nullAgents > 0)
        {
            Debug.LogWarning($"{nullAgents} agents were dead or missing during fitness evaluation and were penalized.");
        }

        if (fitnessScores.Count != population.Count)
        {
            Debug.LogError($"Mismatch! FitnessScores: {fitnessScores.Count}, Population: {population.Count}");
        }
    }

    // computes and records fitness statistics for a generation
    void LogFitness(int generationNumber)
    {
        float total = 0f;
        float max = float.MinValue;
        float min = float.MaxValue;

        foreach (float score in fitnessScores)
        {
            total += score;
            if (score > max) max = score;
            if (score < min) min = score;
        }

        float avg = total / fitnessScores.Count;
        float sumSqDiff = 0f;
        foreach (float score in fitnessScores)
        {
            sumSqDiff += Mathf.Pow(score - avg, 2);
        }
        float stdDev = Mathf.Sqrt(sumSqDiff / fitnessScores.Count);

        averageFitnessHistory.Add(avg);
        maxFitnessHistory.Add(max);
        minFitnessHistory.Add(min);
        standardDeviationHistory.Add(stdDev);
        totalFitnessHistory.Add(total);

        cumulativeTrainingScore += total;

        Debug.Log($"Generation {generationNumber} | Avg: {avg}, Max: {max}, Min: {min}, StdDev: {stdDev}, Total: {total}, Cumulative Total: {cumulativeTrainingScore}");
    }

    // writes fitness history to a csv file
    void SaveFitnessDataToCSV()
    {
        string filePath = Application.dataPath + "/FitnessLog.csv";
        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath))
        {
            writer.WriteLine("Generation,AverageFitness,MaxFitness,MinFitness,StandardDeviation,TotalFitness,CumulativeTrainingScore");

            float runningCumulative = 0f;

            for (int i = 0; i < averageFitnessHistory.Count; i++)
            {
                float avg = averageFitnessHistory[i];
                float max = maxFitnessHistory[i];
                float min = minFitnessHistory[i];
                float std = standardDeviationHistory[i];
                float total = totalFitnessHistory[i];
                runningCumulative += total;

                writer.WriteLine($"{i},{avg},{max},{min},{std},{total},{runningCumulative}");
            }

            writer.WriteLine();
            writer.WriteLine($"TotalTrainingScore,{cumulativeTrainingScore}");
        }

        Debug.Log("Fitness data exported to: " + filePath);
    }

    // exports cumulative heatmap when application quits
    void OnApplicationQuit()
    {
        GAHeatmap heatmap = FindObjectOfType<GAHeatmap>();
        if (heatmap != null)
        {
            heatmap.ExportCumulativeHeatmap();
        }
    }
}
