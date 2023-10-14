using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    public float maxMoveSpeed = 5f;
    public float acceleration = 2f;
    public float deceleration = 3f;
    public float rollSpeed = 8f;
    public float rollDuration = 0.5f;

    private Rigidbody2D rb;
    public bool isRolling = false;
    private float nextRollTime = 0f;  // Time when the next roll is permitted
    public float rollDelay = 1f;      // Delay in seconds between rolls
    private PlayerCharacter playerCharacter;
    public float rollInvincibilityDuration = 0.5f;  // Duration of invincibility during a roll

    private Vector2 lastMoveDirection;
    private Vector2 currentVelocity;
    private bool shouldClampPosition = false; // Add this

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCharacter = GetComponent<PlayerCharacter>();
        if (!playerCharacter)
        {
            Debug.LogError("PlayerCharacter script not found on this GameObject!");
        }
    }

    public void MoveCharacter(Vector2 direction)
    {
        if (!isRolling)
        {
            Walk(direction);
        }
    }

    private void Walk(Vector2 direction)
    {
        currentVelocity = rb.velocity;
        if (direction != Vector2.zero)
        {
            lastMoveDirection = direction;
            currentVelocity = Vector2.ClampMagnitude(Vector2.MoveTowards(currentVelocity, direction + currentVelocity, 100 * Time.deltaTime), maxMoveSpeed);
        }
        else
        {
            currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, deceleration * Time.deltaTime);
        }
        rb.velocity = currentVelocity;
        shouldClampPosition = true; // Mark for clamping
    }

    public void Roll()
    {
        // Check if enough time has passed since the last roll
        if (!isRolling && Time.time > nextRollTime && lastMoveDirection != Vector2.zero)
        {
            StartCoroutine(RollCoroutine());
        }
    }

    private IEnumerator RollCoroutine()
    {
        isRolling = true;
        rb.velocity = Vector2.zero;
        Vector2 rollVelocity = lastMoveDirection.normalized * rollSpeed;
        rb.velocity = rollVelocity;


        if (playerCharacter)
        {
            playerCharacter.BecomeInvincible(rollInvincibilityDuration);
        }
        yield return new WaitForSeconds(rollDuration);
        rb.velocity = Vector2.zero;
        isRolling = false;
        nextRollTime = Time.time + rollDelay;  // Set the time for the next possible roll
    }

    private void FixedUpdate()
    {
        if(shouldClampPosition)
        {
            rb.position = PixelPerfectUtility.ClampToPixelGrid(rb.position);
            shouldClampPosition = false;
        }
    }

    public bool isCurrentlyRolling() {
        return isRolling;
    }
}
