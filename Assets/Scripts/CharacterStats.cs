using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    // Basic stats
    public float maxHealth = 100f;
    public float currentHealth { get; private set; }
    public float attack = 10f;
    public float defense = 5f;
    
    public int goldCount = 0;
    // Advanced/unique stats can be added as needed, examples:
    public float critChance = 0.1f; // 10% chance
    public float critMultiplier = 1.5f; // 150% damage

    // Events for notifying other systems of changes in stats, like taking damage or healing
    public delegate void OnHealthChanged(float currentHealth);
    public event OnHealthChanged onHealthChanged;

    private void Start()
    {
        currentHealth = maxHealth; // Initialize current health
    }

    // Method to handle damage taken
    public void TakeDamage(float damage)
    {
        float damageAfterDefense = damage - defense;
        damageAfterDefense = Mathf.Clamp(damageAfterDefense, 1, maxHealth); // Ensure at least 1 damage is taken

        currentHealth -= damageAfterDefense;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Notify any listeners
        onHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Method to handle healing
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Notify any listeners
        onHealthChanged?.Invoke(currentHealth);
    }

    // Method for character death
    private void Die()
    {
        // Handle character death, e.g., play an animation, restart the game, etc.
        Debug.Log("Character died.");
        // This could be expanded with events or more functionality depending on your game's requirements.
    }

    // Getter methods for other scripts to safely access character stats
    public float GetAttack()
    {
        // Implement logic here to calculate final attack with potential modifiers, buffs, or debuffs.
        // For now, simply returning the base value:
        return attack;
    }

    public float GetDefense()
    {
        return defense;
    }

    public float GetCritChance()
    {
        return critChance;
    }

    public float GetCritMultiplier()
    {
        return critMultiplier;
    }
}
