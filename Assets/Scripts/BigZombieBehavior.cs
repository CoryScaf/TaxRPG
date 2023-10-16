using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigZombieBehavior : MonoBehaviour
{
    private Animator animator;
    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Vector2 knockbackDirection;
    public EnemyStats enemyStats;

    public float abilityRange = 5f;
    private float lastAbilityTime = 0f;
    public float abilityCooldown = 5f;
    public float moveSpeed = 1.5f;

    private bool isInvincible;
    public Sprite whiteSprite;
    public Sprite originalSprite;
    public float invincibilityDur = 0.2f;
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.5f;
    public AudioSource hitSound;
    public AudioSource slamSound;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        enemyStats = GetComponent<EnemyStats>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (enemyStats.currentHealth <= 0) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        Vector2 moveDirection = (playerTransform.position - transform.position).normalized;
        transform.position += (Vector3)moveDirection * moveSpeed * Time.deltaTime;
        Debug.Log(distanceToPlayer <= abilityRange);
        Debug.Log(Time.time - lastAbilityTime >= abilityCooldown);
        if (distanceToPlayer <= abilityRange && Time.time - lastAbilityTime >= abilityCooldown)
        {
            UseAbility();
            lastAbilityTime = Time.time;
        }
    }

    void UseAbility()
    {
        Debug.Log("attack");
        animator.SetTrigger("Attack");
        slamSound.PlayDelayed(0.2f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isInvincible || enemyStats.currentHealth <= 0) return;

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
