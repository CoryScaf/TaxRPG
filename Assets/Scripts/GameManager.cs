using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton pattern
    public int lives = 4;
    public int taxAmount = 100; // This is just a starting value. Adjust as needed.
    public int runsUntilTax = 4;
    public int runCount = 0;

    private void Awake()
    {
        // Singleton pattern to ensure only one GameManager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGame()
    {
        LoadEncounterScene(); // Immediately start the encounter
    }

    public void EndEncounter(bool victory)
    {
        if (victory)
        {
            LoadMapScene();
        }
        else
        {
            LoseLife();
        }
    }

    public void LoseLife()
    {
        lives--;
        if (lives <= 0)
        {
            // End game or show some "game over" scene
            GameOver();
        }
        else
        {
            // Show a scene where the life count slides down
            ShowLifeLostScene();
        }
    }

    public void GameOver()
    {
        // Reset values for a new game
        lives = 4;
        gold = 0;
        taxAmount = 100; 
        runsUntilTax = 4;
        runCount = 0;

        // Load main menu or game over scene
        LoadMainMenu();
    }

    public void CompleteRun()
    {
        runCount++;
        if (runCount >= runsUntilTax)
        {
            if (gold < taxAmount)
            {
                LoseAllLives();
            }
            else
            {
                // Pay the tax
                gold -= taxAmount;

                // Increase tax and enemy difficulty for the next loop
                IncreaseDifficulty();

                // Reset the run count
                runCount = 0;
            }
        }

        // Load training screen after run is complete
        LoadTrainingScreen();
    }

    public void IncreaseDifficulty()
    {
        taxAmount += 50; // Increase tax by some amount
        // Increase enemy stats here too
    }

    public void LoseAllLives()
    {
        lives = 0;
        GameOver();
    }

    // Add methods to load different scenes like LoadEncounterScene(), LoadMapScene(), etc.
}
