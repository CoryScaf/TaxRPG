using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombat : MonoBehaviour
{
    // References
    private Animator animator;
    public GameObject sword;
    // Events (for possible interactions with other systems or UI feedback)
    public delegate void OnAttackHandler();
    public event OnAttackHandler OnAttack;

    private bool isThirdSwingActive = false;
    private bool isCooldownActive = false;
    public int comboCounter = 0;  // Track which swing we're on in the combo
    public float comboDelay = .5f; //time after combo completion before you can start another attack
    public float comboResetTime = 1.5f;  // Time after which the combo resets if no next attack is made(MAKE SURE EXIT TIME FOR SWINGS MATCHES THIS VARIABLE)
    private float lastAttackTime;  // Timestamp of the last attack

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (!animator)
        {
            Debug.LogError("Animator not found on the sword GameObject!");
        }
    }

    public void Attack()
    {
        // Do not allow any attacks if the third swing is active or cooldown is active
        if (isThirdSwingActive || isCooldownActive)
        {
            return;
        }

        // Check if we should reset the combo
        if (Time.time - lastAttackTime > comboResetTime)
        {
            comboCounter = 0;
        }

        //vector from player to mouse
        Vector2 lookDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f;  // Subtract 90 degrees to account for the y-axis default orientation


        // Assuming the swordPivot is a reference to the GameObject that the sword is a child of
        sword.GetComponent<Transform>().rotation = Quaternion.Euler(0, 0, angle);

        
        comboCounter++;
        
        animator.SetTrigger("AttackTrigger");
        animator.SetInteger("ComboCount", comboCounter);
        
        // Trigger event for listeners
        OnAttack?.Invoke();
    }
    private IEnumerator ResetComboAfterDelay()
    {
        yield return new WaitForSeconds(comboResetTime);
        comboCounter = 0;
        animator.SetInteger("ComboCount", comboCounter);
    }



    // This method will be called using an Animation Event at the beginning of the Swing3 animation
    public void StartThirdSwing()
    {
        isThirdSwingActive = true;
    }

    // This method will be called using an Animation Event at the end of the Swing3 animation
    public void EndThirdSwing()
    {
        isThirdSwingActive = false;
        ResetComboCounter();
        StartCoroutine(CooldownAfterCombo());
    }
    private IEnumerator CooldownAfterCombo()
    {
        isCooldownActive = true;
        yield return new WaitForSeconds(comboDelay);  
        isCooldownActive = false;
    }
    public void ResetComboCounter()
    {
        comboCounter = 0;
        animator.SetInteger("ComboCount", comboCounter);
    }

}
