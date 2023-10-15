using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class EnemyStats : CharacterStats
{
 public GameObject damageTextPrefab;  // Assign your prefab in the Inspector
    public float floatSpeed = 1f;  // Speed of floating upwards
    public float fadeDuration = 1f;  // Duration of fading out
    public Vector3 offset = new Vector3(0, -1f, 0f); // Offset from the enemy's position where the text starts

    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
        if(!canvas)
        {
            Debug.LogError("No Canvas child found on the GameObject!");
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        ShowDamageText(damage);

        if(this.currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    void ShowDamageText(int damage)
    {
        GameObject textObject = Instantiate(damageTextPrefab, transform.position + offset, Quaternion.identity, canvas.transform);

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

        Destroy(tmpText.gameObject);
    }

}

