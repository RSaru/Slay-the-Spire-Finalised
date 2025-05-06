using UnityEngine;

[System.Serializable]
public class Genome
{
    public float treasurePriority;
    public float trapAvoidance;
    public float movementAggression;
    public float moveXBias;
    public float moveYBias;
    public float wallAvoidance;
    public float moveSpeedGene;

    public float fitness;

    public Genome() { }

    //initializes genome genes with random values
    public void Initialize()
    {
        treasurePriority = Random.Range(0f, 1f);
        trapAvoidance = Random.Range(0f, 1f);
        movementAggression = Random.Range(0f, 1f);
        moveXBias = Random.Range(-1f, 1f);
        moveYBias = Random.Range(-1f, 1f);
        wallAvoidance = Random.Range(0f, 1f);
        moveSpeedGene = Random.Range(1f, 5f);
    }

    //mutates genome genes based on mutation rate and generation factor
    public void Mutate(float mutationRate = 0.1f, float generationFactor = 1f)
    {
        if (Random.value < mutationRate)
        {
            treasurePriority += Random.Range(-0.5f, 0.5f) * generationFactor;
            treasurePriority = Mathf.Clamp01(treasurePriority);
        }
        if (Random.value < mutationRate)
        {
            trapAvoidance += Random.Range(-0.5f, 0.5f) * generationFactor;
            trapAvoidance = Mathf.Clamp01(trapAvoidance);
        }
        if (Random.value < mutationRate)
        {
            movementAggression += Random.Range(-0.5f, 0.5f) * generationFactor;
            movementAggression = Mathf.Clamp01(movementAggression);
        }
        if (Random.value < mutationRate)
        {
            moveXBias += Random.Range(-1f, 1f) * generationFactor;
            moveXBias = Mathf.Clamp(moveXBias, -1f, 1f);
        }
        if (Random.value < mutationRate)
        {
            moveYBias += Random.Range(-1f, 1f) * generationFactor;
            moveYBias = Mathf.Clamp(moveYBias, -1f, 1f);
        }
        if (Random.value < mutationRate)
        {
            wallAvoidance += Random.Range(-0.5f, 0.5f) * generationFactor;
            wallAvoidance = Mathf.Clamp01(wallAvoidance);
        }
        if (Random.value < mutationRate)
        {
            moveSpeedGene += Random.Range(-1f, 1f) * generationFactor;
            moveSpeedGene = Mathf.Clamp(moveSpeedGene, 0.1f, 10f);
        }
    }

    //creates a child genome by interpolating parent genes
    public static Genome Crossover(Genome p1, Genome p2)
    {
        var child = new Genome();
        float α = Random.value;
        child.treasurePriority = Mathf.Lerp(p1.treasurePriority, p2.treasurePriority, α);
        child.trapAvoidance = Mathf.Lerp(p1.trapAvoidance, p2.trapAvoidance, α);
        child.movementAggression = Mathf.Lerp(p1.movementAggression, p2.movementAggression, α);
        child.moveXBias = Mathf.Lerp(p1.moveXBias, p2.moveXBias, α);
        child.moveYBias = Mathf.Lerp(p1.moveYBias, p2.moveYBias, α);
        child.wallAvoidance = Mathf.Lerp(p1.wallAvoidance, p2.wallAvoidance, α);
        child.moveSpeedGene = Mathf.Lerp(p1.moveSpeedGene, p2.moveSpeedGene, α);
        return child;
    }
}
