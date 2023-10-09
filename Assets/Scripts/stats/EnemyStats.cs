using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class EnemyStats : CharacterStats
{
    protected override void Die()
    {
        base.Die();
        // Enemy-specific death behaviors
        // e.g., Drop loot, give XP to the player, etc.
    }
}

