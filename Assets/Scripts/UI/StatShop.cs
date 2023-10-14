using UnityEngine;
using TMPro;

public class StatShop : MonoBehaviour
{
    public TextMeshProUGUI stat1Text;
    public TextMeshProUGUI stat2Text;
    public TextMeshProUGUI stat3Text;
    public TextMeshProUGUI playerStatsText;
    
    public float costMultiplier = 1.5f;

    private PlayerStats playerStats;

    private string[] statNames = new string[] { "ATK", "DEF", "HP" };
    private string[] rarities = new string[] { "Normal", "Uncommon", "Rare", "Epic", "Legendary" };
    private Color[] rarityColors = { Color.white, Color.green, Color.blue, Color.magenta, Color.yellow };

    private void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        if (!playerStats)
        {
            Debug.LogError("PlayerStats not found in the scene!");
            return;
        }

        GenerateShopItems();
        DisplayPlayerStats();
    }

    private void GenerateShopItems()
    {
        for (int i = 0; i < 3; i++)
        {
            int randomStatIndex = Random.Range(0, statNames.Length);
            int randomRarityIndex = Random.Range(0, rarities.Length);

            string randomStat = statNames[randomStatIndex];
            string randomRarity = rarities[randomRarityIndex];
            Color textColor = rarityColors[randomRarityIndex];

            int randomBoostAmount = Random.Range(1, 2 + randomRarityIndex * 2);  // Example, scale the boost amount with rarity
            float cost = CalculateCost(randomBoostAmount, randomRarityIndex);

            // Set the text and color for each shop item
            TextMeshProUGUI currentText = (i == 0) ? stat1Text : (i == 1) ? stat2Text : stat3Text;
            currentText.text = $"{randomStat} +{randomBoostAmount} ({randomRarity}) - {cost} gold";
            currentText.color = textColor;
        }
    }

    private float CalculateCost(int boostAmount, int rarityIndex)
    {
        float baseCost = boostAmount * costMultiplier;
        float randomPercent = Random.Range(0.01f, 0.2f);  // Random percentage no greater than 20%
        float rarityFactor = 1f + rarityIndex * 0.25f;  // Example, increases the cost as the rarity goes up

        return Mathf.Floor(baseCost * randomPercent * rarityFactor * playerStats.gold);
    }

    private void DisplayPlayerStats()
    {
        playerStatsText.text = $"ATK: {playerStats.attack}, DEF: {playerStats.defense}, HP: {playerStats.maxHealth}";
    }

    public void BuyStat1()
    {
        BuyStat(stat1Text);
    }

    public void BuyStat2()
    {
        BuyStat(stat2Text);
    }

    public void BuyStat3()
    {
        BuyStat(stat3Text);
    }

 private void BuyStat(TextMeshProUGUI statText)
    {
        // Assuming the text structure is "ATK +10 (Rare) - 100 gold"
        string[] parts = statText.text.Split(' ');
        string statName = parts[0];
        int boostAmount = int.Parse(parts[1].Replace("+", ""));
        int cost = Mathf.FloorToInt(float.Parse(parts[4]));

        if (playerStats.gold >= cost)
        {
            UpdatePlayerStat(statName, boostAmount);
            DeductGold(cost);

            // After buying, refresh the shop with new items
            GenerateShopItems();
            DisplayPlayerStats();
        }
        else
        {
            Debug.Log("Not enough gold!");
        }
    }

    private void UpdatePlayerStat(string statName, int boostAmount)
    {
        switch (statName)
        {
            case "ATK":
                playerStats.attack += boostAmount;
                break;
            case "DEF":
                playerStats.defense += boostAmount;
                break;
            case "HP":
                playerStats.maxHealth += boostAmount;
                break;
        }
    }

    private void DeductGold(int cost)
    {
        playerStats.gold -= cost;
    }
}
