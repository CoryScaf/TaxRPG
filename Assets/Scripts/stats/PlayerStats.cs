using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : CharacterStats
{
    // New properties for critical hits and health regeneration
    public float critChance = 0.1f; // 10% chance by default
    public float critMultiplier = 2f; // Double damage on crits by default
    public int regenRate = 1; // Amount of health regenerated per second

    private float regenTimer = 0f; // A timer to track health regen intervals

    // Reference to the HealthBar slider on the canvas
    public Slider healthBar;

    protected override void Awake()
    {
        base.Awake();

        if (healthBar == null)
        {
            Debug.LogError("HealthBar Slider is not set on " + gameObject.name);
            return;
        }

        // Initialize the health bar's max value and current value
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }
    void Update()
    {
        HandleHealthRegen();
    }

    // Override the TakeDamage method to update the health bar
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);  // Call the original TakeDamage functionality

        // Update the health bar value
        healthBar.value = currentHealth;
    }
    // New method for handling health regeneration
    private void HandleHealthRegen()
    {
        if (currentHealth < maxHealth)
        {
            regenTimer += Time.deltaTime;
            if (regenTimer >= 1f)
            {
                currentHealth += regenRate;
                currentHealth = Mathf.Min(currentHealth, maxHealth); // Ensure we don't exceed max health
                regenTimer = 0f;
            }
        }
    }

    // Overriding the TakeDamage method to incorporate critical hits
    public void Attack(CharacterStats target)
    {
        int damageToDeal = attack;
        // Determine if the attack is a critical hit
        if (Random.value < critChance)
        {
            damageToDeal = Mathf.RoundToInt(damageToDeal * critMultiplier);
            Debug.Log("Critical hit!");
        }
        target.TakeDamage(damageToDeal);
    }

    protected override void Die()
    {
        base.Die();
        // Player-specific death behaviors
        // e.g., Respawn, show game over screen, etc.
    }
}
