using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{


    private CharacterMovement movementComponent;
    private CameraController cameraController; 
    private PlayerStats playerStats; // Reference to the PlayerStats script
    void Start()
    {
        movementComponent = GetComponent<CharacterMovement>();
      //  cameraController = FindObjectOfType<CameraController>();
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats component missing on " + gameObject.name);
        }
    }

    void Update()
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
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider belongs to a DamagingObject
        if (other.CompareTag("DamagingObject"))
        {
            // Get the attack stat from the DamagingObject
            CharacterStats attackerStats = other.GetComponent<CharacterStats>();
            if (attackerStats)
            {
                int attackValue = attackerStats.attack;

                // Call TakeDamage method from PlayerStats
                playerStats.TakeDamage(attackValue);

                // Print the current health after the hit
                Debug.Log("Current Health: " + playerStats.currentHealth);
            }
            else
            {
                Debug.LogError("DamagingObject missing CharacterStats component.");
            }
        }
    }
    public void CollectGold(int amount)
    {
       // goldCount += amount;
    }

    public void PayTax(int amount)
    {
       // if (amount <= goldCount)
      //  {
         //   goldCount -= amount;
       // }
       // else
       // {
            // Not enough gold to pay tax logic
       // }
    }



}
