using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public int damageAmount = 1;

    //inflicts damage on player characters upon collision
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PaladinHealthScript paladinHealth = other.GetComponent<PaladinHealthScript>();
            if (paladinHealth != null)
            {
                paladinHealth.TakeDamage(damageAmount);
            }

            SquireHealthScript squireHealth = other.GetComponent<SquireHealthScript>();
            if (squireHealth != null)
            {
                squireHealth.TakeDamage(damageAmount);
            }

            RogueHealthScript rogueHealth = other.GetComponent<RogueHealthScript>();
            if (rogueHealth != null)
            {
                rogueHealth.TakeDamage(damageAmount);
            }

            WarlockHealthScript warlockHealth = other.GetComponent<WarlockHealthScript>();
            if (warlockHealth != null)
            {
                warlockHealth.TakeDamage(damageAmount);
            }
        }
    }
}
