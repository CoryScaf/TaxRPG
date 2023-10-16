using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    private enum State
    {
        Idle,
        Following,
        Charging,
    }

    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;

    public float attackRange = 3f;
    public float followRange = 6f;
    public float detectionRange = 10f;

    public float chargeSpeed = 5f;
    public float chargeTime = 2f;
    public float cooldownTime = 2f;
    public float invincibilityDur = 0.2f;

    public Sprite whiteSprite;
    public Sprite originalSprite;

    private State currentState = State.Idle;
    private Vector2 chargeDirection;
    private float lastAttackTime = -Mathf.Infinity;
    // Public fields to customize knockback force and duration
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.5f;
    public AudioSource hitSound;

    // Private fields to handle knockback state
    private Vector2 knockbackDirection;
    public EnemyStats enemyStats;
    private Rigidbody2D rb;
    private bool isInvincible;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyStats = GetComponent<EnemyStats>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        switch (currentState)
        {
            case State.Idle:
                if (distanceToPlayer <= detectionRange)
                {
                    currentState = State.Following;
                }
                break;

            case State.Following:
                if (distanceToPlayer <= attackRange && Time.time - lastAttackTime >= cooldownTime)
                {
                    StartCoroutine(StartCharging());
                }
                else if (distanceToPlayer <= followRange)
                {
                    Vector2 moveDirection = (playerTransform.position - transform.position).normalized;
                    transform.position += (Vector3)moveDirection * Time.deltaTime; // Assume enemy has a defined move speed
                }
                break;

            case State.Charging:
                // Charging behavior is handled in the StartCharging coroutine.
                break;
        }
    }

    private System.Collections.IEnumerator StartCharging()
    {
        currentState = State.Charging;
        lastAttackTime = Time.time;

        float blinkDuration = 1f;
        float blinkInterval = 0.1f;
        float elapsedTime = 0f;

        while (elapsedTime < blinkDuration)
        {
            spriteRenderer.color = (spriteRenderer.color == Color.red) ? Color.white : Color.red;
            elapsedTime += blinkInterval;
            yield return new WaitForSeconds(blinkInterval);
        }

        spriteRenderer.color = Color.white;

        chargeDirection = (playerTransform.position - transform.position).normalized;
        float startTime = Time.time;

        while (Time.time - startTime < chargeTime)
        {
            transform.position += (Vector3)chargeDirection * chargeSpeed * Time.deltaTime;
            yield return null;
        }

        currentState = State.Following;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(isInvincible) return;
        if (other.CompareTag("CharacterWeapon"))
        {
            WeaponInfo weaponInfo = other.GetComponent<WeaponInfo>();
            if (weaponInfo)
            {
                Vector2 knockbackOrigin = playerTransform.position;
                hitSound.Play();
   
                PlayerStats playerStats = FindObjectOfType<PlayerStats>();
                // Use TakeDamage method in EnemyStats
                enemyStats.TakeDamage(weaponInfo.damage,playerStats.critChance);

                // Knockback logic can still be handled here
                knockbackDirection = (transform.position - (Vector3)knockbackOrigin).normalized;
                StartCoroutine(StartKnockback(knockbackDirection, knockbackForce, knockbackDuration));
                StartCoroutine(InvincibilityCoroutine(invincibilityDur));
            }
            else
            {
                Debug.LogError("No WeaponInfo found on the weapon.");
            }
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
    private IEnumerator StartKnockback(Vector2 direction, float force, float duration)
    {
        rb.velocity = direction * force;
        yield return new WaitForSeconds(duration);
        rb.velocity = Vector2.zero;
    }


}
