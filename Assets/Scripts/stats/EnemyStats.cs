using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        // Enemy-specific damage logic, if any
        // Note: The base.TakeDamage already reduces health and checks for death.
        // Destroy the enemy object after handling the above logic.
        if(this.currentHealth <= 0){
            Destroy(gameObject);
        }
    }

}

