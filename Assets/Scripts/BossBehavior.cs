using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    private enum State
    {
        Idle,
        Following,
        Attacking
    }

    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;

    public float attackRange = 3f;
    public float followRange = 6f;
    public float detectionRange = 10f;
    public float idleRange = 6f;
    public float retreatMultiplier = 2.0f;
    public float chargeTime = 2f;
    public float cooldownTime = 2f;
    public float invincibilityDur = 0.2f;
    public float rageLevel1Percent = 0.75f;
    public float rageLevel2Percent = 0.50f;
    public float rageLevel3Percent = 0.25f;
    public float rageLevelCooldownMultiplier = 0.8f;
    public int rageLevel = 0;
    public GameObject circleAttackPrefab;
    public GameObject followAttackPrefab;
    public GameObject fastAttackPrefab;

    public Sprite whiteSprite;
    public Sprite originalSprite;

    private State currentState = State.Idle;
    private Vector2 chargeDirection;
    private float lastAttackTime = -Mathf.Infinity;
    // Public fields to customize knockback force and duration
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.5f;

    // Private fields to handle knockback state
    private Vector2 knockbackDirection;
    private EnemyStats enemyStats;
    private Rigidbody2D rb;
    private bool isInvincible;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyStats = GetComponent<EnemyStats>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
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
                if (distanceToPlayer <= attackRange && Time.time - lastAttackTime >= (cooldownTime * Mathf.Pow(rageLevelCooldownMultiplier,rageLevel)))
                {
                    StartCoroutine(StartCharging());
                }
                else if (distanceToPlayer <= followRange)
                {
                    Vector2 moveDirection = (playerTransform.position - transform.position).normalized;
                    if(distanceToPlayer <= idleRange) moveDirection *= -retreatMultiplier;
                    transform.position += (Vector3)moveDirection * enemyStats.moveSpeed * Time.deltaTime; // Assume enemy has a defined move speed
                }
                break;
            case State.Attacking:
                break;
        }
    }

    private System.Collections.IEnumerator StartCharging()
    {
        currentState = State.Attacking;
        lastAttackTime = Time.time;

        float blinkInterval = 0.1f;
        float elapsedTime = 0f;
        int range = rageLevel+2;
        if(range > 4) range = 4;

        int attack = Random.Range(1,range);
        if(attack == 1) {
            List<GameObject> attacks = new List<GameObject>(10);
            for(int i = 0; i < 10; ++i) {
                spriteRenderer.color = (spriteRenderer.color == Color.red) ? Color.white : Color.red;
                elapsedTime += blinkInterval;
                Vector3 dir = Quaternion.AngleAxis(36*i, Vector3.forward) * new Vector3(1f, 0f, 0f);
                GameObject shot = Instantiate(circleAttackPrefab, transform.position + dir, Quaternion.identity);
                shot.GetComponent<BossBlackShot>().SetDirection(dir, 36*i-90);
                attacks.Add(shot);

                yield return new WaitForSeconds(blinkInterval);
            }
            for(int i = 0; i < 10; ++i) {
                attacks[i].GetComponent<BossBlackShot>().Shoot();
            }
        }
        else if(attack == 2) {
            Vector3 spawnPos = playerTransform.position - transform.position;
            GameObject shot = Instantiate(followAttackPrefab, spawnPos.normalized + transform.position, Quaternion.identity); 
            shot.GetComponent<BossGreenShot>().SetTarget(playerTransform);
            for(int i = 0; i < 10; ++i) {
                spriteRenderer.color = (spriteRenderer.color == Color.red) ? Color.white : Color.red;
                elapsedTime += blinkInterval;

                yield return new WaitForSeconds(blinkInterval);
            }
            shot.GetComponent<BossGreenShot>().Shoot();
        }
        else if(attack == 3) {
            Vector3 spawnPos = playerTransform.position - transform.position;
            GameObject shot = Instantiate(fastAttackPrefab, spawnPos.normalized + transform.position, Quaternion.identity); 
            shot.GetComponent<BossRedShot>().SetTarget(playerTransform, transform);
            for(int i = 0; i < 10; ++i) {
                spriteRenderer.color = (spriteRenderer.color == Color.red) ? Color.white : Color.red;
                elapsedTime += blinkInterval;

                yield return new WaitForSeconds(blinkInterval);
            }
            shot.GetComponent<BossRedShot>().Shoot();
        }

        spriteRenderer.color = Color.white;

        chargeDirection = (playerTransform.position - transform.position).normalized;
        float startTime = Time.time;

        //while (Time.time - startTime < chargeTime)
        //{
        //}

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
                GetComponent<AudioSource>().Play();
   
                PlayerStats playerStats = FindObjectOfType<PlayerStats>();
                // Use TakeDamage method in EnemyStats
                enemyStats.TakeDamage(weaponInfo.damage,playerStats.critChance);

                // Knockback logic can still be handled here
                knockbackDirection = (transform.position - (Vector3)knockbackOrigin).normalized;
                StartCoroutine(StartKnockback(knockbackDirection, knockbackForce, knockbackDuration));
                StartCoroutine(InvincibilityCoroutine(invincibilityDur));
                float percentHealth = enemyStats.currentHealth / (float)enemyStats.maxHealth;
                if(percentHealth < rageLevel3Percent) {
                    rageLevel = 3;
                }
                else if(percentHealth < rageLevel2Percent) {
                    rageLevel = 2;
                }
                else if(percentHealth < rageLevel1Percent) {
                    rageLevel = 1;
                }
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
