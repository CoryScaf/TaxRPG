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
    private bool isRolling = false;
    private Vector2 lastMoveDirection;
    public Vector2 currentVelocity;
    private bool shouldClampPosition = false; // Add this

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
        if (!isRolling && lastMoveDirection != Vector2.zero)
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
        yield return new WaitForSeconds(rollDuration);
        rb.velocity = Vector2.zero;
        shouldClampPosition = true; // Mark for clamping after roll ends
        isRolling = false;
    }

    private void FixedUpdate()
    {
        if(shouldClampPosition)
        {
            rb.position = PixelPerfectUtility.ClampToPixelGrid(rb.position);
            shouldClampPosition = false;
        }
    }
}
