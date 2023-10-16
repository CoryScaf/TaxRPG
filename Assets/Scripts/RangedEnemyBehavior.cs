using System.Collections;
using UnityEngine;

public class RangedEnemyBehavior : MonoBehaviour
{
    private enum State
    {
        Idle,
        MaintainingDistance,
        Shooting,
    }

    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    public float shootingRange = 8f;
    public float minDistanceFromPlayer = 4f; // The minimum distance enemy wants to keep from player
    public float moveSpeed = 3f;
    public float projectileSpeed = 5f;
    public GameObject projectilePrefab;
    private int shotsTaken = 0;  // Add this at the class level to keep track of shots

    private float lastAttackTime;
    public float fireRate = 1f;  // Time between individual shots or bursts
    public int burstFrequency = 5;  // Shoot a burst every nth shot
    public float timeBetweenBursts = 0.3f;  // Time between projectiles in a burst
    public int burstCount = 3;  // Number of projectiles in a burst
    public Sprite whiteSprite;
    public Sprite originalSprite;
    private bool isInvincible;
    public float invincibilityDur = 0.2f;
    private Vector2 knockbackDirection;
    public EnemyStats enemyStats; // Ensure EnemyStats component is attached to the ranged enemy
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.5f;
    public AudioSource hitSound;
    private State currentState = State.Idle;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyStats = GetComponent<EnemyStats>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        switch (currentState)
        {
            case State.Idle:
                if (distanceToPlayer <= shootingRange)
                {
                    currentState = State.MaintainingDistance;
                }
                break;

            case State.MaintainingDistance:
                Vector2 moveDirection;
                if (distanceToPlayer > minDistanceFromPlayer)
                {
                    moveDirection = (playerTransform.position - transform.position).normalized;
                }
                else
                {
                    moveDirection = (transform.position - playerTransform.position).normalized;
                }
                transform.position += (Vector3)moveDirection * moveSpeed * Time.deltaTime;

                // Checking if enemy's health is above 0 before allowing attacks
                if (distanceToPlayer <= shootingRange && Time.time - lastAttackTime >= fireRate && enemyStats.currentHealth > 0)
                {
                    shotsTaken++;

                    if (shotsTaken % burstFrequency == 0)
                    {
                        currentState = State.Shooting;
                        StartCoroutine(ShootBurst());
                    }
                    else
                    {
                        ShootProjectile();
                    }

                    lastAttackTime = Time.time;
                }
                break;



            case State.Shooting:
                // Shooting behavior handled by the coroutine.
                break;
        }
    }

    private IEnumerator ShootBurst()
    {
        for (int i = 0; i < burstCount; i++)
        {
            ShootProjectile();
            if (i < burstCount - 1)  // No delay after the last shot
                yield return new WaitForSeconds(timeBetweenBursts);
        }
        currentState = State.MaintainingDistance;
    }
    private void ShootProjectile()
    {
        Vector2 fireDirection = (playerTransform.position - transform.position).normalized;
        float angle = Vector2.Angle(fireDirection.normalized, Vector3.right)-90;
        if(fireDirection.normalized.y < 0.0f) {
            angle *= -1;
            angle += 180;
        }
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.AngleAxis(angle, Vector3.forward));
        projectile.GetComponent<Rigidbody2D>().velocity = fireDirection * projectileSpeed;

        // Set the state back to maintaining distance after shooting a single projectile
        currentState = State.MaintainingDistance;
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isInvincible) return;

        if (other.CompareTag("CharacterWeapon"))
        {
            WeaponInfo weaponInfo = other.GetComponent<WeaponInfo>();
            if (weaponInfo)
            {
                Vector2 knockbackOrigin = playerTransform.position;
                hitSound.Play();

                PlayerStats playerStats = FindObjectOfType<PlayerStats>();
                enemyStats.TakeDamage(weaponInfo.damage, playerStats.critChance);

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

    private IEnumerator StartKnockback(Vector2 direction, float force, float duration)
    {
        rb.velocity = direction * force;
        yield return new WaitForSeconds(duration);
        rb.velocity = Vector2.zero;
    }

    private IEnumerator InvincibilityCoroutine(float invincibilityDuration)
    {
        isInvincible = true;
        float elapsed = 0f;

        while (elapsed < invincibilityDuration)
        {
            spriteRenderer.sprite = whiteSprite;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.sprite = originalSprite;
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.2f;
        }
        spriteRenderer.sprite = originalSprite;
        isInvincible = false;
    }
}
