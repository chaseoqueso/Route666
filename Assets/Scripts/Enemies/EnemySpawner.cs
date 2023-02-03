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

    void Awake()
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

        // TODO: what should starting status be??? how deal with that???
        // TEMP
        GameManager.instance.enemySpawnManager.enemySpawnerStatusDatabase[this] = true;
    }

    public void SpawnEnemy()
    {
        Debug.Log("spawning enemy");

        // (look at Continuum gear generation)
        // TODO: generate a random number and find the % it matches and spawn that type of enemy

        // TEMP
        Instantiate( spawnTable[0].EnemyPrefab(), new Vector3(0,0,0), Quaternion.identity );

        GameManager.instance.enemySpawnManager.UpdatePopOnNewSpawn();
    }
}
