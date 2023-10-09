using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{


    private CharacterMovement movementComponent;
    private CameraController cameraController; 

    void Start()
    {
        movementComponent = GetComponent<CharacterMovement>();
      //  cameraController = FindObjectOfType<CameraController>();
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

    public void ModifyHealth(float amount)
    {
       // health += amount;
       // if (health <= 0)
        //{
          //  health = 0;
           // Player death logic
       // }
    }

    public void Attack()
    {
        // Attack logic
    }
}
