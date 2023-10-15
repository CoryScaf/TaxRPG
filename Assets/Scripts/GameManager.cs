using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject playerPrefab;
    public int lives = 4;
    public int taxAmount = 100;
    public int runsUntilTax = 4;
    public int runCount = 1;
    public float difficultyScaler = 1.1f;
    private bool isFirstStart = true;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance exists.
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Prevent destruction when changing scenes.
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "startScene" && isFirstStart)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        isFirstStart = false; // set to false after the initial start
        LoadEncounterScene();  // start in an encounter
    }

    public void EndEncounter(bool victory)
    {
        
        if (victory)
        {
            // heal the player by their regen rate here
            PlayerCharacter player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacter>();
            player.Heal(player.playerStats.regenRate);
            //update stat repository
            this.GetComponent<PlayerStats>().CopyStats(player.playerStats);
            LoadMapScene();
            StartCoroutine(WaitAndNotifyNodeCompletion());
        }
        else
        {
            LoseLife();
        }
    }

    private IEnumerator WaitAndNotifyNodeCompletion()
    {
        // Wait for the map scene to fully load and all objects to be initialized
        yield return new WaitForSeconds(0.5f);  // You can adjust this time as necessary

        MapManager mapManager = FindObjectOfType<MapManager>();
        if (mapManager)
        {
            mapManager.NodeCompleted();
        }
        else
        {
            Debug.LogError("Failed to find MapManager instance after loading map scene.");
        }
    }

    void LoseLife()
    {
        ResetStats();
        lives--;
        if (lives <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }
        SceneManager.LoadScene("lifeLost");
    }
    public void ResetStats()
    {
        // Get the PlayerStats component from the prefab
        PlayerStats prefabStats = playerPrefab.GetComponent<PlayerStats>();

        if (!prefabStats)
        {
            Debug.LogError("No PlayerStats component found on the Player prefab!");
            return;
        }

        // Find the player instance in the scene and get its PlayerStats component
        PlayerCharacter playerInstance = FindObjectOfType<PlayerCharacter>();
        if (!playerInstance)
        {
            Debug.LogError("No Player instance found in the scene!");
            return;
        }
        PlayerStats instanceStats = playerInstance.GetComponent<PlayerStats>();

        // Copy stats from prefab to the instance
        instanceStats.CopyStats(prefabStats);
        this.GetComponent<PlayerStats>().CopyStats(prefabStats);

        //heal players
        instanceStats.currentHealth = instanceStats.maxHealth;
        this.GetComponent<PlayerStats>().currentHealth = this.GetComponent<PlayerStats>().maxHealth;
    }
    void GameOver()
    {
        // Reset all values for a new game.
        runCount = 1;

        //needs to reset player stats

        LoadEncounterScene();  // start in an encounter
        // TODO: Load main menu or game over scene.
    }
    public void StartRun(){
        FindObjectOfType<MapManager>().currentID = "1";
        LoadEncounterScene();  // start in an encounter
    }

    public void CompleteRun()
    {
        runCount++;

            PlayerStats playerStats = FindObjectOfType<PlayerStats>();
            //check if it's time to tax by run count, if they cant pay they lose 
            // if(runCount == runsUntilTax){
            //     
            //     if(playerStats.gold >= taxAmount){
            //         playerStats.gold -= taxAmount;
            //     }else{
            //         //game over scene into game over
            //         //LoadSceneGameOver();
            //         GameOver();
            //     }
            // }
            
            SceneManager.LoadScene("training");
            //heal player till full
            playerStats.currentHealth = playerStats.maxHealth;
 
    }

    void IncreaseDifficulty()
    {
        taxAmount += 50;
        // TODO: Increase enemy stats.
    }

    void LoseAllLives()
    {
        lives = 0;
        GameOver();
    }

    public void LoadEncounterScene()
    {
        SceneManager.LoadScene("CombatScene1");
    }

    public void LoadMapScene()
    {
        SceneManager.LoadScene("map");
    }

}
