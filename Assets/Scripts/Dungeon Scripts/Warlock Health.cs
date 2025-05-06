using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarlockHealthScript : MonoBehaviour
{
    public int maxHealth = 2;
    public int currentHealth;
    public Slider WarlockHealth;

    public float invincibilityDuration = 1f;
    private float invincibilityTimer = 0f;
    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;

    //initializes health, ui slider, and sprite renderer
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        if (WarlockHealth != null)
        {
            WarlockHealth.maxValue = maxHealth;
            WarlockHealth.value = currentHealth;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //updates invincibility timer and visual effect each frame
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

    //starts invincibility timer
    private void StartInvincibility()
    {
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
    }

    //applies flicker effect during invincibility
    private void TriggerInvincibilityEffect()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, Mathf.PingPong(Time.time * 5f, 1f));
        }
    }

    //resets visual effect to normal color
    private void ResetVisualEffect()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }

    //updates the health slider ui
    void UpdateHealthUI()
    {
        if (WarlockHealth != null)
        {
            WarlockHealth.maxValue = maxHealth;
            WarlockHealth.value = (float)currentHealth / maxHealth;
        }
    }

    //handles character death and notifies game manager
    void Die()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.PlayerDied();
        }
        Destroy(gameObject);
    }
}
