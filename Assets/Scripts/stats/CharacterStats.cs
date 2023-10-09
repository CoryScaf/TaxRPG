using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth { get; set; }

    public int attack = 10;
    public int defense = 5;
    public float moveSpeed = 5f;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }


    public virtual void TakeDamage(int damage)
    {
        int damageTaken = Mathf.Clamp(damage - defense, 0, int.MaxValue); // Defense reduces damage
        currentHealth -= damageTaken;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log(transform.name + " died.");
    }
}
