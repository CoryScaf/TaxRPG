using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHitbox : MonoBehaviour
{
    private PlayerCharacter player;

    void Start() {
        player = gameObject.transform.parent.gameObject.GetComponent<PlayerCharacter>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only trigger this logic if the collision is on the main player object and NOT on any child objects (like the sword).

        if (other.CompareTag("DamagingObject") && this.CompareTag("CharacterWeapon") != true)
        {
            player.DamageCalc(other);
        }
    }
}
