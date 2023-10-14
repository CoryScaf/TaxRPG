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
                // Handle player healing here
                break;
            case EncounterType.Shop:
                SceneManager.LoadScene("ShopScene");
                break;
        }
    }
}
