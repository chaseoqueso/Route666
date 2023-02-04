using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [Tooltip("Target number of active enemies - when you drop below this value, new enemies start spawning")]
    [SerializeField] private int targetActiveEnemyPopulation = 25;

    public int totalSpawnedEnemies {get; private set;}     // All ALIVE spawned enemies that exist in the scene right now
    public int totalActiveEnemies {get; private set;}      // All ACTIVE enemies spawned right now

    [HideInInspector] public List<EnemySpawner> enemySpawners = new List<EnemySpawner>();

    public Transform playerLoc;
    [Tooltip("The radius within which enemies can spawn")]
    [SerializeField] private float spawnDistanceFromPlayer = 50f;

    void Awake()
    {
        if(!GameManager.instance){
            Debug.LogError("No Game Manager found in scene!");
            return;
        }
        GameManager.instance.spawnManager = this;

        if(!playerLoc){
            Debug.LogError("No player transform assigned to the Spawn Manager! (Assigning it now but you should put it in in the inspector)");
            playerLoc = FindObjectOfType<Player>().transform;
        }

        totalSpawnedEnemies = 0;
        totalActiveEnemies = 0;
    }

    void Start()
    {
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

        Debug.Log("current active enemy population: " + totalActiveEnemies);
    }

    public void SpawnEnemies()
    {
        List<EnemySpawner> activeSpawners = new List<EnemySpawner>();
        foreach(EnemySpawner spawner in enemySpawners){
            
        }

        // if(at least one spawner is in range){
            while(totalActiveEnemies < targetActiveEnemyPopulation){
                // Spawn more enemies (from a random currently active spawner) to get up to the target

                // TODO: Pick a random spawner w/in range of the player

                enemySpawners[0].SpawnEnemy();
            }
        // }
    }
}
