using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [Tooltip("Target number of active enemies - when you drop below this value, new enemies start spawning")]
    [SerializeField] private int targetActiveEnemyPopulation = 25;

    public int totalSpawnedEnemies {get; private set;}     // All ALIVE spawned enemies that exist in the scene right now
    public int totalActiveEnemies {get; private set;}      // All ACTIVE enemies spawned right now

    [HideInInspector] public Dictionary<EnemySpawner,bool> enemySpawnerStatusDatabase = new Dictionary<EnemySpawner,bool>();

    void Start()
    {
        if(!GameManager.instance.enemySpawnManager)
            GameManager.instance.enemySpawnManager = this;

        totalSpawnedEnemies = 0;
        totalActiveEnemies = 0;

        SpawnEnemies();
    }

    // Called when an enemy dies
    public void UpdatePopOnEnemyDeath()
    {
        totalSpawnedEnemies--;
        totalActiveEnemies--;
        
        SpawnEnemies();
    }

    // Called when an EnemySpawner creates a new enemy
    public void UpdatePopOnNewSpawn()
    {
        totalSpawnedEnemies++;
        totalActiveEnemies++;
    }

    public void SpawnEnemies()
    {
        // while(totalActiveEnemies < targetActiveEnemyPopulation){
            // Spawn more enemies (from a random currently active spawner) to get up to the target

            // TODO: Pick a random spawner from the dictionary

            // TEMP
            foreach( KeyValuePair<EnemySpawner,bool> spawnerStatus in enemySpawnerStatusDatabase ){
                // If this spawner is active, spawn the thing (TEMP)
                if( spawnerStatus.Value ){
                    spawnerStatus.Key.SpawnEnemy();
                    break;
                    // continue;
                }
            }
            // if you get this far, cancel the while loop cuz no spawners are in range? idk
            // return;
        // }
    }
}
