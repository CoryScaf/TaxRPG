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
    public int runCount = 0;
    public float difficultyScaler = 1.2f;
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
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName != "CombatScene1" && 
            currentSceneName != "CombatScene2" && 
            currentSceneName != "CombatScene3" && 
            currentSceneName != "startScene" && 
            currentSceneName != "BossScene")
        {
            StartCoroutine(WaitDestroyText());
        }
    }
        private IEnumerator WaitDestroyText()
    {
        // Wait for the map scene to fully load and all objects to be initialized
        yield return new WaitForSeconds(2f);  // You can adjust this time as necessary
         DestroyChildrenUnderTextTaggedCanvas();
    }

    public void StartGame()
    {
        
        LoadEncounterScene();  // start in an encounter
    }
    public void DestroyChildrenUnderTextTaggedCanvas()
    {
        // Find the canvas in the scene with the tag "text"
        GameObject canvas = GameObject.FindGameObjectWithTag("text");

        // If canvas is found
        if (canvas)
        {
            // Get all children under the canvas
            int childCount = canvas.transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                // Access the child GameObject directly from the Transform component
                GameObject childObject = canvas.transform.GetChild(i).gameObject;
                
                // Destroy the child game object
                Destroy(childObject);
            }
        }
        else
        {
            Debug.LogWarning("Canvas with the tag 'text' not found in the scene.");
        }
    }

    public void EndEncounter(bool victory)
    {
        if (victory)
        {
             isFirstStart = false; // set to false after the initial start
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
        ResetTemporaryStats();
        lives--;
        if(lives < 0){
            lives = 0;
        }
        Debug.Log(lives);
        if (lives == 0)
        {
            SceneManager.LoadScene("GameOver");
        }else{
            SceneManager.LoadScene("lifeLost");
        }
        
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
    public void ResetTemporaryStats()
    {
        // Find the player instance in the scene and get its PlayerStats component
        PlayerCharacter playerInstance = FindObjectOfType<PlayerCharacter>();
        if (!playerInstance)
        {
            Debug.LogError("No Player instance found in the scene!");
            return;
        }
        PlayerStats instanceStats = playerInstance.GetComponent<PlayerStats>();

        // Reset the temporary stats of the instance
        instanceStats.RemoveTemporaryStats();

        // Reset the temporary stats of the GameManager's PlayerStats component
        this.GetComponent<PlayerStats>().RemoveTemporaryStats();

        // Heal the player instance in the scene
        instanceStats.currentHealth = instanceStats.maxHealth;

        // Heal the GameManager's PlayerStats component (assuming it's meant to represent player stats)
        this.GetComponent<PlayerStats>().currentHealth = this.GetComponent<PlayerStats>().maxHealth;
    }

    public void GameOver()
    {
        // Reset all values for a new game.
        lives = 4;
        taxAmount = 100;
        runsUntilTax = 4;
        runCount = 0;
        //needs to reset player stats
        ResetTemporaryStats();
        ResetStats();

        LoadEncounterScene();  // start in an encounter
        // TODO: Load main menu or game over scene.
    }
    public void StartRun()
    {
        PlayerCharacter player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacter>();
        //update stat repository
        this.GetComponent<PlayerStats>().CopyStats(player.playerStats);
        if(isFirstStart == false){
            FindObjectOfType<MapManager>().currentID = "1";
        }
        
        LoadEncounterScene();  // start in an encounter
    }

    public void CompleteRun()
    {

        runCount++;

        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        //check if it's time to tax by run count, if they cant pay they lose 
        if(runCount == runsUntilTax){
            
            if(playerStats.gold >= taxAmount){
                playerStats.gold -= taxAmount;
                this.GetComponent<PlayerStats>().CopyStats(playerStats);
                taxAmount += (int)(Mathf.Pow(runCount,1.1f) + playerStats.gold/2);
                lives++;
                runsUntilTax +=4;
                
                //remove stats bought from the stat shop
                ResetTemporaryStats();
                SceneManager.LoadScene("training");
                //heal player till full
                playerStats.currentHealth = playerStats.maxHealth;
            }else{
                //game over scene into game over
                LoseAllLives();
                return;
                
            }
        }else{
            //remove stats bought from the stat shop
            ResetTemporaryStats();
            SceneManager.LoadScene("training");
            //heal player till full
            playerStats.currentHealth = playerStats.maxHealth;
        }


    }

    void IncreaseDifficulty()
    {
        taxAmount += 50;
        // TODO: Increase enemy stats.
    }

    void LoseAllLives()
    {
        lives = 0;
        SceneManager.LoadScene("GameOver");
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
