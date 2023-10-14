using UnityEngine;
using TMPro;

public class StatTrainer : MonoBehaviour
{
    public TextMeshProUGUI maxHealthStatText, attackStatText, defenseStatText, critChanceStatText, regenRateStatText;
    public TextMeshProUGUI maxHealthCostText, attackCostText, defenseCostText, critChanceCostText, regenRateCostText;

    public int maxHealthUpgradeCost = 5;
    public int attackUpgradeCost = 2;
    public int defenseUpgradeCost = 2;
    public int critChanceUpgradeCost = 5;
    public int regenRateUpgradeCost = 8;

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

        UpdateStatDisplay();
        UpdateCostDisplay();
    }

    public void UpdateStatDisplay()
    {
        maxHealthStatText.text = playerStats.maxHealth.ToString();
        attackStatText.text = playerStats.attack.ToString();
        defenseStatText.text = playerStats.defense.ToString();
        critChanceStatText.text = (playerStats.critChance * 100).ToString("F1") + "%";
        regenRateStatText.text = playerStats.regenRate.ToString();
    }

    public void UpdateCostDisplay()
    {
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
            UpdateCostDisplay();
        }
    }

    public void UpgradeAttack()
    {
        if (CanAfford(attackUpgradeCost))
        {
            playerStats.attack += 5;
            DeductGold(attackUpgradeCost);
            UpdateStatDisplay();
            UpdateCostDisplay();
        }
    }

    public void UpgradeDefense()
    {
        if (CanAfford(defenseUpgradeCost))
        {
            playerStats.defense += 5;
            DeductGold(defenseUpgradeCost);
            UpdateStatDisplay();
            UpdateCostDisplay();
        }
    }

    public void UpgradeCritChance()
    {
        if (CanAfford(critChanceUpgradeCost))
        {
            playerStats.critChance += 0.05f;
            DeductGold(critChanceUpgradeCost);
            UpdateStatDisplay();
            UpdateCostDisplay();
        }
    }

    public void UpgradeRegenRate()
    {
        if (CanAfford(regenRateUpgradeCost))
        {
            playerStats.regenRate += 1;
            DeductGold(regenRateUpgradeCost);
            UpdateStatDisplay();
            UpdateCostDisplay();
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
