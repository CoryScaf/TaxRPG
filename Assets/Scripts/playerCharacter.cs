using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public float health = 100f;
    public float attackPower = 10f;
    public int goldCount = 0;

    private CharacterMovement movementComponent;

    void Start()
    {
        movementComponent = GetComponent<CharacterMovement>();
    }

    void Update()
    {
        Vector2 direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        movementComponent.MoveCharacter(direction);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            movementComponent.Roll();
        }

    }

    public void CollectGold(int amount)
    {
        goldCount += amount;
    }

    public void PayTax(int amount)
    {
        if (amount <= goldCount)
        {
            goldCount -= amount;
        }
        else
        {
            // Not enough gold to pay tax logic
        }
    }

    public void ModifyHealth(float amount)
    {
        health += amount;
        if (health <= 0)
        {
            health = 0;
            // Player death logic
        }
    }

    public void Attack()
    {
        // Attack logic
    }
}
