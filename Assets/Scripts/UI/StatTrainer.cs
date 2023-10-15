using UnityEngine;
using TMPro;

public class StatTrainer : MonoBehaviour
{
    public TextMeshProUGUI maxHealthStatText, attackStatText, defenseStatText, critChanceStatText, regenRateStatText;
    public TextMeshProUGUI maxHealthCostText, attackCostText, defenseCostText, critChanceCostText, regenRateCostText;
    public TextMeshProUGUI playerStatsText;
    public TextMeshProUGUI taxesOwed;
    public TextMeshProUGUI runsLeft;
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
        playerStats = FindObjectOfType<PlayerStats>();

        if (!playerStats)
        {
            Debug.LogError("PlayerStats not found in the scene!");
        }

        //update tax and runs left text
        GameManager gameManager = FindObjectOfType<GameManager>();

        taxesOwed.text = $"{gameManager.runsUntilTax - gameManager.runCount}";
        runsLeft.text = $"{gameManager.taxAmount}";

        UpdateStatDisplay();
        UpdateCostDisplay();

    }
    void Update()
    {
        DisplayCurrentPlayerStats();
        //update current gold
        PlayerCharacter player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacter>();
        player.playerStats.UpdateGoldText();
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
        maxHealthStatText.text = (playerStats.maxHealth + 10).ToString();
        maxHealthStatText.color = Color.green;
    }

    public void OnHoverAttackUpgrade()
    {
        attackStatText.text = (playerStats.attack + 5).ToString();
        attackStatText.color = Color.green;
    }

    public void OnHoverDefenseUpgrade()
    {
        defenseStatText.text = (playerStats.defense + 5).ToString();
        defenseStatText.color = Color.green;
    }

    public void OnHoverCritChanceUpgrade()
    {
        critChanceStatText.text = ((playerStats.critChance + 0.05f) * 100).ToString("F1") + "%";
        critChanceStatText.color = Color.green;
    }

    public void OnHoverRegenRateUpgrade()
    {
        regenRateStatText.text = (playerStats.regenRate + 1).ToString();
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
            playerStats.maxHealth += 10;
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
            playerStats.attack += 5;
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
            playerStats.defense += 5;
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
            playerStats.critChance += 0.02f;
            playerStats.scaleCritDamage();
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
            playerStats.regenRate += 1;
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
