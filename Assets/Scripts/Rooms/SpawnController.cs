using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public List<Transform> spawnLocations;
    public TilemapCollider2D tilemapCollider;

    // Enemy Prefabs and Weights
    public GameObject zombiePrefab;
    public float zombieWeight = 1.0f;

    public GameObject batPrefab;
    public float batWeight = 1.0f;

    public GameObject bigZombiePrefab;
    public float bigZombieWeight = 1.0f;

    public GameObject explodingZombiePrefab;
    public float explodingZombieWeight = 1.0f;

    public GameObject daggerMagerPrefab;
    public float daggerMagerWeight = 1.0f;

    public int waveCount = 1;
    public float spawnChance = 0.3f;
    public float chanceIncreasePerWave = 0.1f;
    public float didntSpawnPreviousIncrease = 0.05f;

    public int curWave = 0;
    private List<GameObject> spawnedEnemies;

    void Start()
    {
        spawnedEnemies = new List<GameObject>(spawnLocations.Count);
    }

    void Update()
    {
        if(curWave < waveCount) 
        {
            foreach(GameObject spawned in spawnedEnemies.ToList()) 
            {
                if(spawned != null) return;
                else spawnedEnemies.Remove(spawned);
            }

            GetComponent<AudioSource>().Play();

            for(int i = 0; i < spawnLocations.Count; ++i) 
            {
                float chance = Random.Range(0f, 1f);
                bool shouldSpawn = chance < (spawnChance + chanceIncreasePerWave*curWave);

                if (shouldSpawn)
                {
                    GameObject enemyToSpawn = GetWeightedRandomEnemyPrefab();
                    spawnedEnemies.Add(Instantiate(enemyToSpawn, spawnLocations[i].position, Quaternion.identity));
                }
            }

            curWave++;
        }
        else 
        {
            foreach(GameObject spawned in spawnedEnemies.ToList()) 
            {
                if(spawned != null){
                    if(spawned.GetComponent<EnemyStats>().currentHealth > 0) return;
                }
            }

            FindObjectOfType<GameManager>().EndEncounter(true);
        }
    }

    private GameObject GetWeightedRandomEnemyPrefab()
    {
        float totalWeight = zombieWeight + batWeight + bigZombieWeight + explodingZombieWeight + daggerMagerWeight;
        float randomValue = Random.Range(0, totalWeight);

        if (randomValue < zombieWeight)
            return zombiePrefab;

        randomValue -= zombieWeight;
        if (randomValue < batWeight)
            return batPrefab;

        randomValue -= batWeight;
        if (randomValue < bigZombieWeight)
            return bigZombiePrefab;

        randomValue -= bigZombieWeight;
        if (randomValue < explodingZombieWeight)
            return explodingZombiePrefab;

        return daggerMagerPrefab;
    }
}
