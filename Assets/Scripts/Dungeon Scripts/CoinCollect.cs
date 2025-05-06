using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject particleEffectPrefab;
    public int scoreValue = 50;
    private HashSet<int> agentsThatCollected;

    public static event System.Action OnCoinCollected;

    //initializes the set of agent IDs that have collected this coin
    private void Start()
    {
        agentsThatCollected = new HashSet<int>();
    }

    //handles player collision and awards score if not already collected
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            int agentID = collision.gameObject.GetInstanceID();
            if (!agentsThatCollected.Contains(agentID))
            {
                agentsThatCollected.Add(agentID);
                ScoreManager.instance.AddScore(scoreValue);
                if (audioSource != null)
                {
                    audioSource.Play();
                }
                if (particleEffectPrefab != null)
                {
                    Instantiate(particleEffectPrefab, transform.position, Quaternion.identity);
                }
                OnCoinCollected?.Invoke();
            }
        }
    }
}
