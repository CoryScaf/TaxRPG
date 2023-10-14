using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MapNode : MonoBehaviour
{
    public enum EncounterType
    {
        Normal,
        Boss,
        HealSpot,
        Shop
    }

    public EncounterType encounterType;
    public string uniqueID;
    public List<MapNode> children; // Only used for initialization.
    private Button nodeButton;

    private void Awake()
    {
        nodeButton = GetComponent<Button>();
        LockNode();
    }

    public void UnlockNode()
    {
        nodeButton.interactable = true;
    }

    public void LockNode()
    {
        nodeButton.interactable = false;
    }

    public void OnNodeClick()
    {
        HandleEncounter();
        MapManager.instance.NodeCompleted();
    }

    public void HandleEncounter()
    {
        MapManager.instance.SetCurrentID(uniqueID);
        encounterType = this.encounterType;

        switch (encounterType)
        {
            case EncounterType.Normal:
                int randomCombatScene = Random.Range(1, 4);
                SceneManager.LoadScene("CombatScene" + randomCombatScene);
                break;
            case EncounterType.Boss:
                SceneManager.LoadScene("BossScene");
                break;
            case EncounterType.HealSpot:
                PlayerCharacter player = FindObjectOfType<PlayerCharacter>();
                if (player)
                {
                    PlayerStats stats = player.GetComponent<PlayerStats>();
                    if (stats)
                    {
                        int healAmount = Mathf.FloorToInt(stats.currentHealth / 3);
                        player.Heal(healAmount);
                        Debug.Log($"Player healed by {healAmount} points.");
                    }
                    else
                    {
                        Debug.LogError("PlayerCharacter does not have a PlayerStats component attached.");
                    }
                }
                else
                {
                    Debug.LogError("No player character found in the scene.");
                }

                MapManager mapManager = FindObjectOfType<MapManager>();
                if (mapManager)
                {
                    
                    mapManager.NodeCompleted();
                    mapManager. InitializeMap();
                }
                else
                {
                    Debug.LogError("Failed to find MapManager instance after loading map scene.");
                }
                break;
            case EncounterType.Shop:
                SceneManager.LoadScene("ShopScene");
                break;
        }
    }
}
