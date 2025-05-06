using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SquireHealthScript : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;
    public Slider SquireHealth;
    public float invincibilityDuration = 1f;
    private float invincibilityTimer = 0f;
    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;

    //initializes health values, ui slider, and sprite renderer
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        if (SquireHealth != null)
        {
            SquireHealth.maxValue = maxHealth;
            SquireHealth.value = currentHealth;
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
                isInvincible = false;
        }

        if (isInvincible)
            TriggerInvincibilityEffect();
        else
            ResetVisualEffect();
    }

    //applies damage if not invincible and handles death condition
    public void TakeDamage(int damage)
    {
        if (isInvincible) return;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
        StartInvincibility();
        if (currentHealth <= 0)
            Die();
    }

    //starts the invincibility period
    private void StartInvincibility()
    {
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
    }

    //applies flicker effect while invincible
    private void TriggerInvincibilityEffect()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(1f, 1f, 1f, Mathf.PingPong(Time.time * 5f, 1f));
    }

    //resets sprite color after invincibility ends
    private void ResetVisualEffect()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;
    }

    //updates the health slider ui
    void UpdateHealthUI()
    {
        if (SquireHealth != null)
        {
            SquireHealth.maxValue = maxHealth;
            SquireHealth.value = (float)currentHealth / maxHealth;
        }
    }

    //handles character death, notifies game manager, and ends rl episode
    void Die()
    {
        if (GameManager.instance != null)
            GameManager.instance.PlayerDied();

        SquireAgent agent = GetComponent<SquireAgent>();
        if (agent != null)
        {
            agent.AddReward(-1.0f);
            agent.EndEpisode();
        }
    }
}
