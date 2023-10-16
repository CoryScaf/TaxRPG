using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class EnemyStats : CharacterStats
{
    public GameObject damageTextPrefab;  // Assign your prefab in the Inspector
    public GameObject critTextPrefab; // Assign your critical hit text prefab in the Inspector
    public float floatSpeed = 1f;  // Speed of floating upwards
    public float fadeDuration = 1f;  // Duration of fading out
    public Vector3 offset = new Vector3(0, -1f, 0f); // Offset from the enemy's position where the text starts
    public int goldValue = 2;
    public AudioSource deathSound;
    public Slider healthbar;
    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
        if(!canvas)
        {
            Debug.LogError("No Canvas child found on the GameObject!");
        }
    }
    protected override void Start()
    {

        if(GameManager.instance != null) // Make sure GameManager exists
        {
            int runs = GameManager.instance.runCount ; // Subtract 1 as you mentioned
            float scalar = GameManager.instance.difficultyScaler;
            scaleStats(runs, scalar);
        }
        currentHealth = maxHealth;
        if(healthbar != null) {
            healthbar.maxValue = maxHealth;
            healthbar.value = currentHealth;
        }
    }

    public void TakeDamage(int damage, float critChance)
    {
        bool isCrit = Random.Range(0f, 1f) < critChance;

        // Retrieve the critMultiplier from the player's PlayerStats
        float critMultiplier = 2f;  // Default to 2 as before
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if (playerStats)
            {
                critMultiplier = playerStats.critMultiplier;
            }
        }

        if (isCrit)
        {
            damage = Mathf.RoundToInt(damage * critMultiplier);
        }

        base.TakeDamage(damage);
        if(healthbar != null) {
            healthbar.value = currentHealth;
        }

        if (isCrit)
        {
            ShowCritText(damage);
        }
        else
        {
            ShowDamageText(damage);
        }

        if (this.currentHealth <= 0)
        {
            deathSound.Play();
            player.GetComponent<PlayerStats>().gold += goldValue;
            player.GetComponent<PlayerStats>().UpdateGoldText();

            // Disable sprite renderer and collider
            GetComponent<SpriteRenderer>().enabled = false;
            Collider2D[] colliders = GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                collider.enabled = false;
            }

            if (SceneManager.GetActiveScene().name.Equals("BossScene"))
            {
                StartCoroutine(killedBoss());
            }
            else
            {
                Destroy(gameObject, fadeDuration + 1); // Add a slight buffer to the fade duration to ensure the text fades completely
            }
        }
    }

    private IEnumerator killedBoss() {
        while(deathSound.isPlaying) {
            yield return null;
        }
        FindObjectOfType<GameManager>().EndEncounter(true);
    }


    void ShowCritText(int damage)
    {
        GameObject canvasObject = GameObject.FindGameObjectWithTag("text");
        if (!canvasObject)
        {
            Debug.LogError("Canvas with tag 'TextCanvas' not found!");
            return;
        }

        GameObject textObject = Instantiate(critTextPrefab, transform.position + offset, Quaternion.identity, canvasObject.transform);
        TMP_Text tmpText = textObject.GetComponent<TMP_Text>();
        tmpText.text = damage.ToString();
        StartCoroutine(FloatingAndFade(tmpText));
    }

    void ShowDamageText(int damage)
    {
        GameObject canvasObject = GameObject.FindGameObjectWithTag("text");
        if (!canvasObject)
        {
            Debug.LogError("Canvas with tag 'TextCanvas' not found!");
            return;
        }

        GameObject textObject = Instantiate(damageTextPrefab, transform.position + offset, Quaternion.identity, canvasObject.transform);
        TMP_Text tmpText = textObject.GetComponent<TMP_Text>();
        tmpText.text = damage.ToString();
        StartCoroutine(FloatingAndFade(tmpText));
    }


    IEnumerator FloatingAndFade(TMP_Text tmpText)
    {
        float elapsed = 0f;
        Color originalColor = tmpText.color;

        while (elapsed < fadeDuration)
        {
            tmpText.transform.position += Vector3.up * floatSpeed * Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            tmpText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(tmpText.gameObject); // Destroys the text object after the fade finishes.
    }

    public void scaleStats(int runs, float scalar)
    {
        int delta = (int)Mathf.Pow(runs,scalar);
        this.maxHealth += delta;
        this.attack += delta;
        this.defense +=  delta;
        this.goldValue += delta;
        // ... (repeat for any other stats)
    }

}

