using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    public float maxMoveSpeed = 5f;
    public float acceleration = 2f;
    public float deceleration = 3f;
    public float rollSpeed = 8f; // Independent roll speed
    public float rollDuration = 0.5f;

    private Rigidbody2D rb;
    private bool isRolling = false;
    private Vector2 lastMoveDirection;
    public Vector2 currentVelocity;

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
        currentVelocity = rb.velocity;// makes sure currentvelocity doesn't get disconnected from the players actual velocity
        if (direction != Vector2.zero)
        {
            lastMoveDirection = direction;// update for roll
            currentVelocity = Vector2.ClampMagnitude(Vector2.MoveTowards(currentVelocity, direction+currentVelocity, 100 * Time.deltaTime),maxMoveSpeed);
        }
        else
        {
            Debug.Log(currentVelocity);
           currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, deceleration * Time.deltaTime);
           Debug.Log(currentVelocity);
        }
        rb.velocity = currentVelocity;
    }

    public void Roll()
    {
        if (!isRolling && lastMoveDirection != Vector2.zero)
        {
            StartCoroutine(RollCoroutine());
        }
    }

    private System.Collections.IEnumerator RollCoroutine()
    {
        isRolling = true;
        rb.velocity = Vector2.zero;//remove any speed you currently have
        Vector2 rollVelocity = lastMoveDirection.normalized * rollSpeed;
        rb.velocity = rollVelocity;
        yield return new WaitForSeconds(rollDuration);
        rb.velocity = Vector2.zero;//stops player after a roll
        isRolling = false;
    }
}