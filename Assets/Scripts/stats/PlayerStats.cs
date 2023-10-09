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

    public float invincibilityDuration = 2f; // Duration of invincibility in seconds
    public float knockbackForce = 5f; // Force of the knockback

    // Reference to the HealthBar slider on the canvas
    public Slider healthBar;

    private Rigidbody2D rb;
    private PlayerCharacter playerCharacter; 

    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody2D>();
        playerCharacter = GetComponent<PlayerCharacter>();
        if (healthBar == null)
        {
            Debug.LogError("HealthBar Slider is not set on " + gameObject.name);
            return;
        }

        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        if (playerCharacter.IsInvincible()) return; // Don't take damage if invincible

        base.TakeDamage(damage);
        healthBar.value = currentHealth;

        // Apply the knockback
        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Force);


        // Start invincibility
        if (playerCharacter)
        {
            playerCharacter.BecomeInvincible(invincibilityDuration);
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
