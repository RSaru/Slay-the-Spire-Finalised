using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalItem : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject particleEffectPrefab;
    public int scoreValue = 500;
    public GameObject oldObject;
    public GameObject newObject;
    private HashSet<Collider2D> collectedBy = new HashSet<Collider2D>();

    //handles player entering trigger to collect goal and play effects
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (collectedBy.Contains(collision)) return;

        collectedBy.Add(collision);

        geneticAgent agent = collision.GetComponent<geneticAgent>();
        if (agent != null)
        {
            agent.CollectTreasure(scoreValue);
        }

        if (audioSource != null)
        {
            audioSource.Play();
        }

        if (particleEffectPrefab != null)
        {
            Instantiate(particleEffectPrefab, transform.position, Quaternion.identity);
        }

        if (oldObject != null)
        {
            oldObject.SetActive(false);
        }

        if (newObject != null)
        {
            newObject.SetActive(true);
        }
    }

    //resets collected state and restores objects to initial visibility
    public void ResetState()
    {
        collectedBy.Clear();

        if (oldObject != null)
            oldObject.SetActive(true);

        if (newObject != null)
            newObject.SetActive(false);
    }
}
