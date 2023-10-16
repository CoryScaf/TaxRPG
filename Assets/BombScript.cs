using UnityEngine;
using System.Collections;
public class BombScript : MonoBehaviour
{
 public float explosionRadius = 1.5f; // radius in which the bomb can affect the player
    public float explosionDuration = 3f; // time after which the bomb explodes
    public GameObject explosionEffect; // optional visual effect for explosion
    public float blinkDuration = 0.2f; // How long each blink lasts
    public int numberOfBlinks = 3; // How many times the bomb should blink before exploding

    public Sprite originalSprite; // Original sprite of the bomb
    public Sprite whiteSprite; // White sprite to blink to

    private SpriteRenderer spriteRenderer;
    private EnemyStats enemyStats;
    private Transform childTransform;
    private SpriteRenderer childSpriteRenderer;
    private float originalTransparency;



    private void Start()
    {
        // Assuming the bomb has only one child:
        if (transform.childCount > 0)
        {
            childSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

            if (childSpriteRenderer != null)
            {
                originalTransparency = childSpriteRenderer.color.a;
            }

            childTransform = transform.GetChild(0);
            SetChildScale();
        }
        enemyStats = GetComponent<EnemyStats>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if(!enemyStats)
        {
            Debug.LogError("EnemyStats component missing from the bomb.");
            return;
        }

        // Start blinking before the bomb explodes
        Invoke("StartBlinking", explosionDuration - (numberOfBlinks * blinkDuration * 2));
        
        // Destroy the bomb after explosionDuration
        Invoke("Explode", explosionDuration);
    }

    void SetChildScale()
    {
        if (childTransform != null)
        {
            float newScale = 12f * explosionRadius;
            childTransform.localScale = new Vector3(newScale, newScale, newScale);
        }
    }
void StartBlinking()
{
    StartCoroutine(BlinkBeforeExplosion());
}

System.Collections.IEnumerator BlinkBeforeExplosion()
{
    SpriteRenderer childSpriteRenderer = null;
    if (transform.childCount > 0)
    {
        childSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    float originalChildTransparency = (childSpriteRenderer != null) ? childSpriteRenderer.color.a : 1.0f;

    for (int i = 0; i < numberOfBlinks; i++)
    {
        // Switch to white sprite
        spriteRenderer.sprite = whiteSprite;
        
        // Reduce transparency of child object's sprite
        if (childSpriteRenderer != null)
        {
            Color tempColor = childSpriteRenderer.color;
            tempColor.a = 0;
            childSpriteRenderer.color = tempColor;
        }
        
        yield return new WaitForSeconds(blinkDuration);

        // Switch back to original sprite
        spriteRenderer.sprite = originalSprite;
        
        // Reset transparency of child object's sprite
        if (childSpriteRenderer != null)
        {
            Color tempColor = childSpriteRenderer.color;
            tempColor.a = originalChildTransparency;
            childSpriteRenderer.color = tempColor;
        }
        
        yield return new WaitForSeconds(blinkDuration);
    }
}

    

    void Explode()
    {
        // Create an explosion effect (if you have one)
        if(explosionEffect)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Check for players within explosion radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player")) // or whatever the player tag is
            {
                PlayerCharacter damageController = hit.GetComponent<PlayerCharacter>(); // assuming the DamageCalc method is in this script
                if (damageController)
                {
                    damageController.DamageCalc(this.GetComponent<Collider2D>());
                }
            }
        }
        
        Destroy(gameObject); // destroy the bomb after explosion
    }

    private void Update()
    {
        if(enemyStats.currentHealth <= 0)
        {
            Explode();
        }
    }
}
