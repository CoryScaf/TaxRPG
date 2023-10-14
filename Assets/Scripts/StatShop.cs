using UnityEngine;
using TMPro;

public class StatShop : MonoBehaviour
{
    public float costMultiplier = 1.5f;

    public PlayerStats playerStats;

    private string[] statNames = new string[] { "ATK", "DEF", "HLT", "CRIT", "REG" };
    private string[] rarities = new string[] { "Normal", "Uncommon", "Rare", "Epic", "Legendary" };
    private Color[] rarityColors = { Color.white, Color.green, Color.blue, Color.magenta, Color.yellow };
    private int[] boostAmounts = new int[3]; // Assuming 3 is the size of your other arrays like statTexts
    private int[] costs = new int[3];


    public TextMeshProUGUI[] statTexts; // Array of TextMeshProUGUI for stat types (e.g., ATK, DEF, HP).
    public TextMeshProUGUI[] boostTexts; // Array for stat increase amounts.
    public TextMeshProUGUI[] costTexts; // Array for costs.
    public TextMeshProUGUI[] rarityTexts; // Array for rarities.
    public TextMeshProUGUI playerStatsText;
    private void Start()
    {

        // playerStats = FindObjectOfType<PlayerStats>();
        // if (!playerStats)
        // {
        //     Debug.LogError("PlayerStats not found in the scene!");
        //     return;
        // }
        GenerateShopItems();
        DisplayPlayerStats();
    }

    public void GenerateShopItems()
    {
        for (int i = 0; i < statTexts.Length; i++)
        {
            int randomStatIndex = Random.Range(0, statNames.Length);

            boostAmounts[i] = GenerateBoostAmount(out string rarity, out Color rarityColor);
            costs[i] = CalculateCost(boostAmounts[i], rarity);

            statTexts[i].text = statNames[randomStatIndex];
            boostTexts[i].text = "+" + boostAmounts[i];
            costTexts[i].text = costs[i].ToString();
            rarityTexts[i].color = rarityColor;
            rarityTexts[i].text = rarity;

        }
    }

    private int GenerateBoostAmount(out string rarity, out Color rarityColor)
    {
        int rarityIndex = Random.Range(0, 5);
        string[] rarities = { "Normal", "Uncommon", "Rare", "Epic", "Legendary" };
        int[] minMaxBoosts = { 1, 5, 10, 15, 20 };
        rarity = rarities[rarityIndex];
        rarityColor = rarityColors[rarityIndex];
        return Random.Range(minMaxBoosts[rarityIndex], minMaxBoosts[rarityIndex] + 5);
    }

    private int CalculateCost(int boostAmount, string rarity)
    {
        float baseCost = boostAmount * 1.5f;
        float rarityMultiplier = 1f;
        switch (rarity)
        {
            case "Normal": rarityMultiplier = 0.5f; break;
            case "Uncommon": rarityMultiplier = 0.8f; break;
            case "Rare": rarityMultiplier = 1f; break;
            case "Epic": rarityMultiplier = 1.5f; break;
            case "Legendary": rarityMultiplier = 2f; break;
        }
        float randomPercent = Random.Range(0.01f, 0.2f);
        int calculatedCost = Mathf.FloorToInt(baseCost * rarityMultiplier * randomPercent * playerStats.gold);
        return Mathf.Max(calculatedCost, (int)(10*rarityMultiplier)*0); // Ensure minimum cost is 1
    }

    private void DisplayPlayerStats()
    {
        playerStatsText.text = $"HLT: {playerStats.maxHealth} ATK: {playerStats.attack} DEF: {playerStats.defense} CRT: {playerStats.critChance} REG: {playerStats.regenRate}";
    }

    public void BuyStat1()
    {
        BuyStat(0);
    }

    public void BuyStat2()
    {
        BuyStat(1);
    }

    public void BuyStat3()
    {
        BuyStat(2);
    }


    public void BuyStat(int index)
    {
        if (playerStats.gold >= costs[index])
        {
           UpdatePlayerStat(statTexts[index].text, boostAmounts[index]);
           DeductGold(costs[index]);

            //After buying, refresh the shop with new items
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
                playerStats.tempAttack += boostAmount;
                break;
            case "DEF":
                playerStats.defense += boostAmount;
                playerStats.tempDefense += boostAmount;
                break;
            case "HLT":
                playerStats.maxHealth += boostAmount;
                playerStats.tempMaxHealth += boostAmount;
                break;
            case "CRIT":
                playerStats.critChance += boostAmount;
                playerStats.tempCritChance += boostAmount;
                break;
            case "REG":
                playerStats.regenRate += boostAmount;
                playerStats.tempRegenRate += boostAmount;
                break;
        }
    }
    public void DeductGold(int cost){
        playerStats.gold -= cost;
    }



}
