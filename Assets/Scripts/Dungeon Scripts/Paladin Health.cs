using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaladinHealthScript : MonoBehaviour
{
    public int maxHealth = 4;
    public int currentHealth;
    public Slider PaladinHealth;

    public float invincibilityDuration = 1f;
    private float invincibilityTimer = 0f;
    private bool isInvincible = false;

    private SpriteRenderer spriteRenderer;

    //initializes health, ui slider, and sprite renderer
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        if (PaladinHealth != null)
        {
            PaladinHealth.maxValue = maxHealth;
            PaladinHealth.value = currentHealth;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //updates invincibility timer and visual effects each frame
    void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0f)
            {
                isInvincible = false;
            }
        }
        if (isInvincible)
        {
            TriggerInvincibilityEffect();
        }
        else
        {
            ResetVisualEffect();
        }
    }

    //applies damage if not invincible and handles death
    public void TakeDamage(int damage)
    {
        if (isInvincible) return;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
        StartInvincibility();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //starts invincibility period
    private void StartInvincibility()
    {
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
    }

    //applies flickering effect during invincibility
    private void TriggerInvincibilityEffect()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, Mathf.PingPong(Time.time * 5f, 1f));
        }
    }

    //resets visual effect to normal
    private void ResetVisualEffect()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }

    //updates the health slider ui
    public void UpdateHealthUI()
    {
        if (PaladinHealth != null)
        {
            PaladinHealth.maxValue = maxHealth;
            PaladinHealth.value = (float)currentHealth / maxHealth;
        }
    }

    //handles character death and destroys non-rlagent objects
    void Die()
    {
        if (GetComponent<RLAgent>() != null) return;
        Destroy(gameObject);
    }
}
