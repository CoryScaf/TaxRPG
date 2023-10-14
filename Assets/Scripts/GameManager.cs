using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int lives = 4;
    public int taxAmount = 100;
    public int runsUntilTax = 4;
    public int runCount = 0;

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
        lives--;
        if (lives <= 0)
        {
            GameOver();
        }
        // Else, consider implementing a visual representation of losing a life here.
    }

    void GameOver()
    {
        // Reset all values for a new game.
        lives = 4;
        taxAmount = 100;
        runsUntilTax = 4;
        runCount = 0;

        // TODO: Load main menu or game over scene.
    }

    public void CompleteRun()
    {
        runCount++;
        if (runCount >= runsUntilTax)
        {
            // TODO: Check if player has enough gold to pay tax.
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
