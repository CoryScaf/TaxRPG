using System.Collections;
using UnityEngine;

public class BatBehavior : MonoBehaviour
{
    private enum State
    {
        Idle,
        Following
    }

    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;

    public float followRange = 8f; 
    public float detectionRange = 12f;
    public float invincibilityDur = 0.2f;

    public Sprite whiteSprite;
    public Sprite originalSprite;
    public AudioSource hitSound;

    private State currentState = State.Idle;
    private Rigidbody2D rb;
    private bool isInvincible;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
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
                if (distanceToPlayer <= followRange)
                {
                    Vector2 moveDirection = (playerTransform.position - transform.position).normalized;
                    transform.position += (Vector3)moveDirection * 5 * Time.deltaTime;
                }
                else if (distanceToPlayer > detectionRange)
                {
                    currentState = State.Idle;
                }
                break;
        }
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
                
                EnemyStats enemyStats = GetComponent<EnemyStats>();
                PlayerStats playerStats = FindObjectOfType<PlayerStats>();
                // Use TakeDamage method in EnemyStats
                enemyStats.TakeDamage(weaponInfo.damage,playerStats.critChance);


                Vector2 knockbackDirection = (transform.position - (Vector3)knockbackOrigin).normalized;
                StartCoroutine(StartKnockback(knockbackDirection, 10f, 1.5f));
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
            spriteRenderer.sprite = whiteSprite;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.sprite = originalSprite;
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.2f;
        }
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
