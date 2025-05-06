using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public AudioSource audioSource;
    public float speed = 10f;
    public int damage = 10;
    public float lifetime = 5f;

    //destroys projectile after its lifetime
    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    //moves projectile forward at constant speed
    private void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    //handles collisions, applies damage to players, and destroys projectile
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            WarlockHealthScript warlockHealth = collision.GetComponent<WarlockHealthScript>();
            if (warlockHealth != null)
            {
                warlockHealth.TakeDamage(damage);
            }

            SquireHealthScript squireHealth = collision.GetComponent<SquireHealthScript>();
            if (squireHealth != null)
            {
                squireHealth.TakeDamage(damage);
            }

            PaladinHealthScript paladinHealth = collision.GetComponent<PaladinHealthScript>();
            if (paladinHealth != null)
            {
                paladinHealth.TakeDamage(damage);
            }

            RogueHealthScript rogueHealth = collision.GetComponent<RogueHealthScript>();
            if (rogueHealth != null)
            {
                rogueHealth.TakeDamage(damage);
            }
        }

        Destroy(gameObject);

        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
