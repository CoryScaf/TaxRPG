using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth { get; set; }

    public int attack = 10;
    public int defense = 5;
    public float moveSpeed = 5f;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }


    public virtual void TakeDamage(int damage)
    {
        int damageTaken = (int)((damage*100)/(100f+defense));
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
