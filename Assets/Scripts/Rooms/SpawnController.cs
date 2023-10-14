using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public List<Transform> spawnLocations;
    public GameObject spawn;
    public int waveCount = 1;
    public float spawnChance = 0.3f;
    public float chanceIncreasePerWave = 0.1f;
    public float didntSpawnPreviousIncrease = 0.05f;
    
    public int curWave = 0;
    private List<GameObject> spawnedEnemies;
    // Start is called before the first frame update
    void Start()
    {
        spawnedEnemies = new List<GameObject>(spawnLocations.Count);
    }

    // Update is called once per frame
    void Update()
    {
        if(curWave < waveCount) {
            foreach(GameObject spawned in spawnedEnemies.ToList()) {
                if(spawned != null) return;
                else spawnedEnemies.Remove(spawned);
            }

            for(int i = 0; i < spawnLocations.Count; ++i) {
                float chance = Random.Range(1, 100) / 100.0f;
                bool shouldSpawn = chance < (spawnChance + chanceIncreasePerWave*curWave);
                if(shouldSpawn) {
                    spawnedEnemies.Add(Instantiate(spawn, spawnLocations[i].position, Quaternion.identity));
                }
            }
            
            curWave++;
        }
        else {
            foreach(GameObject spawned in spawnedEnemies.ToList()) {
                if(spawned != null) return;
            }

            // Move back to map
        }
    }
}
