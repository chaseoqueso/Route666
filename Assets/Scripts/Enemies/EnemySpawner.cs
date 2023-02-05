using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnTableEntry{
    [Tooltip("A specific enemy prefab to generate from this spawner")]
    [SerializeField] private GameObject enemyPrefab;

    [Tooltip("% chance of this enemy spawning (all %s in the list must add up to 100%) - as whole numbers (50% = 50)")]
    [SerializeField] private float spawnChance;

    public GameObject EnemyPrefab(){return enemyPrefab;}
    public float SpawnChance(){return spawnChance;}
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<SpawnTableEntry> spawnTable = new List<SpawnTableEntry>();

    // [Tooltip("OPTIONAL timer - if the value here is > 0, it will spawn with a timer and IGNORE the target enemy population instead")]
    // [SerializeField] private float spawnTimer;

    [Tooltip("If the radius of this spawner goes below this value, it waits until the player's out of range and spawns more")]
    [SerializeField] private int targetActiveEnemies = 10;

    [Tooltip("The radius outside of which enemies can spawn")]
    [SerializeField] private float spawnDistanceFromPlayer = 50f;

    public Transform playerLoc;

    public int totalActiveEnemies {get; private set;}      // All ACTIVE enemies spawned right now

    [Tooltip("This gets deleted on play; just here to make it easier to see in the editor")]
    [SerializeField] private GameObject editorSpatialIndicator;

    void Awake()
    {
        if(editorSpatialIndicator){
            Destroy(editorSpatialIndicator);
        }
    }

    void Start()
    {
        // Check for an input error in the spawn table and throw an error if there is one
        if(spawnTable.Count == 0){
            Debug.LogError("spawner has empty spawn table");
            return;
        }

        float total = 0f;
        foreach(SpawnTableEntry entry in spawnTable){
            total += entry.SpawnChance();
        }
        if((int)total < 100){
            Debug.LogError("total spawn chance is less than 100%");
        }
        else if((int)total > 100){
            Debug.LogError("total spawn chance is greater than 100%");
        }

        totalActiveEnemies = 0;
    }

    void Update()
    {
        // If the player is out of range of the spawner, spawn more enemies
        if( Vector3.Distance( transform.position, playerLoc.position ) >= spawnDistanceFromPlayer ){    // totalActiveEnemies < targetActiveEnemies &&
            SpawnEnemies();
        }
    }

    public void SpawnEnemies()
    {
        while(totalActiveEnemies < targetActiveEnemies){
            // (look at Continuum gear generation)
            // TODO: generate a random number and find the % it matches and spawn that type of enemy

            // TEMP
            GameObject newEnemy = Instantiate( spawnTable[0].EnemyPrefab(), gameObject.transform.position, Quaternion.identity );

            newEnemy.GetComponent<Enemy>().spawnPoint = this;
            newEnemy.GetComponent<Enemy>().playerLoc = playerLoc;

            UpdatePopOnNewSpawn();
        }
    }

    // Called when an enemy dies
    public void UpdatePopOnEnemyDeath()
    {
        totalActiveEnemies--;
    }

    // Called when an EnemySpawner creates a new enemy
    public void UpdatePopOnNewSpawn()
    {
        totalActiveEnemies++;
    }
}
