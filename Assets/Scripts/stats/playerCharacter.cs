using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    private CharacterMovement movementComponent;
    private CameraController cameraController;
    private PlayerStats playerStats; // Reference to the PlayerStats script
    private bool isInvincible = false;

    [HideInInspector]
    public bool isInKnockback = false;  // Indicates if the player is currently undergoing knockback
    public float knockbackDuration = 0.5f;  // Duration of the knockback effect

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    void Start()
    {
        movementComponent = GetComponent<CharacterMovement>();
        cameraController = FindObjectOfType<CameraController>();
        if (cameraController == null)
        {
            Debug.LogError("CameraController component missing on " + gameObject.name);
        }
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats component missing on " + gameObject.name);
        }
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on PlayerCharacter or its children.");
            return;
        }
        originalColor = spriteRenderer.color;
    }

    void Update()
    {
        if (isInKnockback == false)
        {
            Vector2 direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            movementComponent.MoveCharacter(direction);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                movementComponent.Roll();
            }
            // Attack on mouse click (or any other input binding)
            if (Input.GetMouseButtonDown(0))  // 0 is the left mouse button
            {
                GetComponent<CharacterCombat>().Attack();
            }

        }

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only trigger this logic if the collision is on the main player object and NOT on any child objects (like the sword).

        if (other.CompareTag("DamagingObject") && this.CompareTag("CharacterWeapon") != true)
        {
            DamageCalc(other);
        }
    }


    public void DamageCalc(Collider2D other)
    {
        // Retrieve the CharacterStats of the damaging object (like the enemy)
        CharacterStats attackerStats = other.GetComponent<CharacterStats>();
        if (attackerStats)
        {
            int damage = attackerStats.attack;
            // Calculate knockback direction based on attacker's position
            Vector2 knockbackDirection = (transform.position - other.transform.position).normalized;
            // Handle player taking damage, knockback, etc. here

            playerStats.TakeDamage(damage, knockbackDirection);
            //shake camera
            cameraController.TriggerShake();
            // Start the knockback effect
            StartKnockbackEffect();
        }
        else
        {
            Debug.LogError("CharacterStats component missing from the attacking object.");
        }
    }


    public void StartKnockbackEffect()
    {
        isInKnockback = true;

        // This will call the ResetKnockback() method after the duration specified in knockbackDuration
        Invoke("ResetKnockback", knockbackDuration);
    }

    private void ResetKnockback()
    {
        isInKnockback = false;
    }
    public void BecomeInvincible(float invincibilityDuration)
    {
        if (!isInvincible)
        {
            StartCoroutine(InvincibilityCoroutine(invincibilityDuration));
        }
    }

    private IEnumerator InvincibilityCoroutine(float invincibilityDuration)
    {
        isInvincible = true;

        float elapsed = 0f;
        while (elapsed < invincibilityDuration)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0f);  // Make sprite transparent
            yield return new WaitForSeconds(0.1f);

            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);  // Return sprite to original visibility
            yield return new WaitForSeconds(0.1f);

            elapsed += 0.2f;
        }

        spriteRenderer.color = originalColor;
        isInvincible = false;
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }


    public void PayTax(int amount)
    {
        // if (amount <= goldCount)
        //  {
        //   goldCount -= amount;
        // }
        // else
        // {
        // Not enough gold to pay tax logic
        // }
    }



}
