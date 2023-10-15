using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class die : MonoBehaviour
{
    public TextMeshProUGUI currentLife;
    public TextMeshProUGUI newLife;
    private GameManager gameManager;  // <-- Missing semicolon

    void Start()  // <-- Capitalize and use parentheses
    {
        gameManager = FindObjectOfType<GameManager>();
        currentLife.text = $"{gameManager.lives + 1}";  // <-- It should be "lives", not "life"
        newLife.text = $"{gameManager.lives}";
    }

    public void dies()
    {
        gameManager.CompleteRun();
    }
}
