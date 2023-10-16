using UnityEngine;
using TMPro;

public class StatTrainer : MonoBehaviour
{
    public TextMeshProUGUI maxHealthStatText, attackStatText, defenseStatText, critChanceStatText, regenRateStatText;
    public TextMeshProUGUI maxHealthCostText, attackCostText, defenseCostText, critChanceCostText, regenRateCostText;
    public TextMeshProUGUI playerStatsText;
    public TextMeshProUGUI taxesOwed;
    public TextMeshProUGUI runsLeft;
    public TextMeshProUGUI lives;
    public int maxHealthIncrement = 10;
    public int attackIncrement = 5;
    public int defenseIncrement = 5;
    public float critChanceIncrement = 0.02f;
    public int regenRateIncrement = 1;

    public int maxHealthUpgradeCost = 5;
    public int attackUpgradeCost = 2;
    public int defenseUpgradeCost = 2;
    public int critChanceUpgradeCost = 5;
    public int regenRateUpgradeCost = 8;
    public int maxHealthBaseCost = 5;
    public float maxHealthScaleFactor = 0.1f;

    public int attackBaseCost = 2;
    public float attackScaleFactor = 0.2f;

    public int defenseBaseCost = 2;
    public float defenseScaleFactor = 0.2f;

    public int critChanceBaseCost = 5;
    public float critChanceScaleFactor = 20.0f;

    public int regenRateBaseCost = 8;
    public float regenRateScaleFactor = 0.5f;


    private Color defaultColor;
    private PlayerStats playerStats;

    private void Start()
    {
        defaultColor = maxHealthStatText.color;
        playerStats = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerStats>();

        if (!playerStats)
        {
            Debug.LogError("PlayerStats not found in the scene!");
        }

        //update tax and runs left text
        GameManager gameManager = FindObjectOfType<GameManager>();

        taxesOwed.text = $"{gameManager.runsUntilTax - gameManager.runCount}";
        runsLeft.text = $"{gameManager.taxAmount}";
        lives.text = $"{gameManager.lives}";

        UpdateStatDisplay();
        UpdateCostDisplay();

    }
    void Update()
    {
        DisplayCurrentPlayerStats();
    }
    public void StartRun()
    {
        FindObjectOfType<GameManager>().StartRun();
    }
    private void DisplayCurrentPlayerStats()
    {
        playerStatsText.text = $"HLT: {playerStats.maxHealth} ATK: {playerStats.attack} DEF: {playerStats.defense} CRT: {playerStats.critChance} REG: {playerStats.regenRate}";
    }


    public void UpdateStatDisplay()
    {
        maxHealthStatText.text = playerStats.maxHealth.ToString();
        attackStatText.text = playerStats.attack.ToString();
        defenseStatText.text = playerStats.defense.ToString();
        critChanceStatText.text = (playerStats.critChance * 100).ToString("F1") + "%";
        regenRateStatText.text = playerStats.regenRate.ToString();
    }

    private void UpdateCostDisplay()
    {
        maxHealthUpgradeCost = maxHealthBaseCost + Mathf.CeilToInt(maxHealthScaleFactor * playerStats.maxHealth);
        attackUpgradeCost = attackBaseCost + Mathf.CeilToInt(attackScaleFactor * playerStats.attack);
        defenseUpgradeCost = defenseBaseCost + Mathf.CeilToInt(defenseScaleFactor * playerStats.defense);
        critChanceUpgradeCost = critChanceBaseCost + Mathf.CeilToInt(critChanceScaleFactor * playerStats.critChance);
        regenRateUpgradeCost = regenRateBaseCost + Mathf.CeilToInt(regenRateScaleFactor * playerStats.regenRate);

        maxHealthCostText.text = maxHealthUpgradeCost.ToString();
        attackCostText.text = attackUpgradeCost.ToString();
        defenseCostText.text = defenseUpgradeCost.ToString();
        critChanceCostText.text = critChanceUpgradeCost.ToString();
        regenRateCostText.text = regenRateUpgradeCost.ToString();
    }



    public void OnHoverMaxHealthUpgrade()
    {
        maxHealthStatText.text = (playerStats.maxHealth + maxHealthIncrement).ToString();
        maxHealthStatText.color = Color.green;
    }

    public void OnHoverAttackUpgrade()
    {
        attackStatText.text = (playerStats.attack + attackIncrement).ToString();
        attackStatText.color = Color.green;
    }

    public void OnHoverDefenseUpgrade()
    {
        defenseStatText.text = (playerStats.defense + defenseIncrement).ToString();
        defenseStatText.color = Color.green;
    }

    public void OnHoverCritChanceUpgrade()
    {
        critChanceStatText.text = ((playerStats.critChance + critChanceIncrement) * 100).ToString("F1") + "%";
        critChanceStatText.color = Color.green;
    }

    public void OnHoverRegenRateUpgrade()
    {
        regenRateStatText.text = (playerStats.regenRate + regenRateIncrement).ToString();
        regenRateStatText.color = Color.green;
    }


    public void OnExitHoverMaxHealthUpgrade()
    {
        UpdateStatDisplay();
        maxHealthStatText.color = defaultColor;
    }

    public void OnExitHoverAttackUpgrade()
    {
        UpdateStatDisplay();
        attackStatText.color = defaultColor;
    }

    public void OnExitHoverDefenseUpgrade()
    {
        UpdateStatDisplay();
        defenseStatText.color = defaultColor;
    }

    public void OnExitHoverCritChanceUpgrade()
    {
        UpdateStatDisplay();
        critChanceStatText.color = defaultColor;
    }

    public void OnExitHoverRegenRateUpgrade()
    {
        UpdateStatDisplay();
        regenRateStatText.color = defaultColor;
    }

    public void UpgradeMaxHealth()
    {
        if (CanAfford(maxHealthUpgradeCost))
        {
            playerStats.maxHealth += maxHealthIncrement;
            DeductGold(maxHealthUpgradeCost);
            UpdateStatDisplay();
            maxHealthUpgradeCost = maxHealthBaseCost + Mathf.CeilToInt(maxHealthScaleFactor * playerStats.maxHealth);
            UpdateCostDisplay();
            DisplayCurrentPlayerStats();
        }
    }

    public void UpgradeAttack()
    {
        if (CanAfford(attackUpgradeCost))
        {
            playerStats.attack += attackIncrement;
            DeductGold(attackUpgradeCost);
            UpdateStatDisplay();
            attackUpgradeCost = attackBaseCost + Mathf.CeilToInt(attackScaleFactor * playerStats.attack);
            UpdateCostDisplay();
            DisplayCurrentPlayerStats();
        }
    }

    public void UpgradeDefense()
    {
        if (CanAfford(defenseUpgradeCost))
        {
            playerStats.defense += defenseIncrement;
            DeductGold(defenseUpgradeCost);
            UpdateStatDisplay();
            defenseUpgradeCost = defenseBaseCost + Mathf.CeilToInt(defenseScaleFactor * playerStats.defense);
            UpdateCostDisplay();
            DisplayCurrentPlayerStats();
        }
    }

    public void UpgradeCritChance()
    {
        if (CanAfford(critChanceUpgradeCost))
        {
            playerStats.critChance += critChanceIncrement;
            playerStats.scaleCritDamage();  // Assuming this method exists to scale critical damage
            DeductGold(critChanceUpgradeCost);
            UpdateStatDisplay();
            critChanceUpgradeCost = critChanceBaseCost + Mathf.CeilToInt(critChanceScaleFactor * playerStats.critChance);
            UpdateCostDisplay();
            DisplayCurrentPlayerStats();
        }
    }

    public void UpgradeRegenRate()
    {
        if (CanAfford(regenRateUpgradeCost))
        {
            playerStats.regenRate += regenRateIncrement;
            DeductGold(regenRateUpgradeCost);
            UpdateStatDisplay();
            regenRateUpgradeCost = regenRateBaseCost + Mathf.CeilToInt(regenRateScaleFactor * playerStats.regenRate);
            UpdateCostDisplay();
            DisplayCurrentPlayerStats();
        }
    }



    private bool CanAfford(int cost)
    {
        return playerStats.gold >= cost;
    }

    private void DeductGold(int cost)
    {
        playerStats.gold -= cost;
    }
}
