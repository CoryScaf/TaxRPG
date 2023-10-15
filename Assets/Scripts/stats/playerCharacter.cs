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

    public Sprite whiteSprite;  // The white version of the character sprite
    public Sprite originalSprite;  // Reference to the original sprite

    private Color originalColor;
    void Start()
    {
        movementComponent = GetComponent<CharacterMovement>();
        cameraController = FindObjectOfType<CameraController>();
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
    }

    void Update()
    {
        // Debugging: End the encounter when the E key is pressed
        if (Input.GetKeyDown(KeyCode.E))
        {
            bool victory = true;  // Assuming victory for now; you can toggle this to false for debugging defeats
            GameManager.instance.EndEncounter(victory);
        }  
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
    public void Heal(int amount)
    {
        if (playerStats)
        {
            playerStats.currentHealth += amount;
           
            if (playerStats.currentHealth > playerStats.maxHealth)
            {
                playerStats.currentHealth = playerStats.maxHealth;
            }
            playerStats.healthBar.value = playerStats.currentHealth;
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
            if(!isInvincible)
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
        spriteRenderer.sprite = whiteSprite;  // Change to the white version
        yield return new WaitForSeconds(0.1f);  // Wait for 0.1 seconds

        spriteRenderer.sprite = originalSprite;  // Change back to the original sprite
        yield return new WaitForSeconds(0.1f);  // Wait for 0.1 seconds

        elapsed += 0.2f;  // 0.1s + 0.1s = 0.2s total for one blink cycle
    }

    // Ensure the sprite is set back to the original at the end
    spriteRenderer.sprite = originalSprite;

    isInvincible = false;
}



    public bool IsInvincible()
    {
        return isInvincible;
    }






}
