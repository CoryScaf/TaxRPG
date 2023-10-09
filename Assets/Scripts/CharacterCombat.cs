using UnityEngine;
using System.Collections;
public class CharacterCombat : MonoBehaviour
{
    // References
    private Animator animator;

    // Events (for possible interactions with other systems or UI feedback)
    public delegate void OnAttackHandler();
    public event OnAttackHandler OnAttack;

    public GameObject sword;  // Reference to the sword GameObject

    public int comboCounter = 0;  // Track which swing we're on in the combo
    public float comboResetTime = 1.5f;  // Time after which the combo resets if no next attack is made
    public float comboDelay = 0.5f; // time after combo completion before you can start another attack

    private bool canAttack = true;  // Controls whether a new attack can be initiated
    private bool isThirdSwingActive = false;  // Tracks if the third swing is currently playing
    private bool isCooldownActive = false;  // Tracks if the character is in the cooldown period after completing a combo

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (!animator)
        {
            Debug.LogError("Animator not found on the GameObject!");
        }
    }

    public void Attack()
    {
        if (!canAttack || isThirdSwingActive || isCooldownActive)
        {
            return;
        }

        comboCounter++;
        animator.SetTrigger("AttackTrigger");
        animator.SetInteger("ComboCount", comboCounter);

        // If player is initiating the first swing, we stop any ongoing ComboResetDelay coroutines
        if (comboCounter == 1)
        {
            StopAllCoroutines();  // Stops any ongoing ComboResetDelay
        }
        // Start a new ComboResetDelay coroutine
        StartCoroutine(ComboResetDelay());

        if (comboCounter == 3)
        {
            StartCoroutine(CooldownAfterCombo());
        }

        // Trigger event for listeners
        OnAttack?.Invoke();
    }

    private IEnumerator ComboResetDelay()
    {
        yield return new WaitForSeconds(comboResetTime);
        ResetComboCounter();
    }

    private IEnumerator CooldownAfterCombo()
    {
        isCooldownActive = true;
        yield return new WaitForSeconds(comboDelay);
        isCooldownActive = false;
        ResetComboCounter();
    }

    private void ResetComboCounter()
    {
        comboCounter = 0;
        animator.SetInteger("ComboCount", comboCounter);
    }

    public void StartThirdSwing()
    {
        isThirdSwingActive = true;
    }

    // This method will be called using an Animation Event at the end of the Swing3 animation
    public void EndThirdSwing()
    {
        isThirdSwingActive = false;
    }

    public void RotateSwordTowardsMouse()
    {
        Vector2 lookDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f; // Subtract 90 degrees to account for the y-axis default orientation

        // Assuming the swordPivot is a reference to the GameObject that the sword is a child of
        sword.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Update()
    {
        RotateSwordTowardsMouse();
    }
}
