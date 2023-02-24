using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnTableEntry : System.IComparable<SpawnTableEntry>
{
    [Tooltip("A specific enemy prefab to generate from this spawner")]
    [SerializeField] private GameObject enemyPrefab;

    [Tooltip("% chance of this enemy spawning, as whole numbers (50% = 50). all Spawn Chances must add up to exactly 100")]
    [SerializeField] private float spawnChance;

    public GameObject EnemyPrefab(){return enemyPrefab;}
    public float SpawnChance(){return spawnChance;}

    public int CompareTo(SpawnTableEntry otherEntry)
    {
        return this.spawnChance.CompareTo(otherEntry.SpawnChance());
    }
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

        if(!playerLoc){
            playerLoc = GameManager.instance.player.transform;
        }
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
            // Generate a random number and find the % it matches to spawn that type of enemy
            float chanceRolled = Random.Range(0.0f, 100.0f);

            SpawnTableEntry enemyToSpawn = spawnTable[0];
            foreach( SpawnTableEntry entry in spawnTable ){
                // if the next item in the list has a chance that is LESS THAN the chance that you rolled,
                // then subtract that item's chance from chance rolled
                float entryChance = entry.SpawnChance();
                if( entryChance <= chanceRolled ){
                    chanceRolled -= entryChance;
                }
                // ... until you find a chance greater than the chance that you rolled
                else{
                    enemyToSpawn = entry;
                }
            }

            if( enemyToSpawn == null ){
                Debug.LogError("No enemy prefabs found in spawn table");
            }

            GameObject newEnemy = Instantiate( enemyToSpawn.EnemyPrefab(), gameObject.transform.position, Quaternion.identity );

            newEnemy.GetComponent<Enemy>().spawnPoint = this;
            newEnemy.GetComponent<Enemy>().playerLoc = playerLoc;

            RandomlyGeneratePunkEnemyAppearance(newEnemy);

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

    #region Random Punk Enemy Appearance Generation
        private void RandomlyGeneratePunkEnemyAppearance(GameObject newEnemy)
        {
            SkinnedMeshRenderer[] meshRenderers = newEnemy.GetComponentsInChildren<SkinnedMeshRenderer>(true);

            // Pick skin mat out here so that it can be the same for all categories
            Material skinMat = ChooseRandomSkinMat();
            Material vestMat = ChooseRandomClothesMat();
            Material shirtMat = ChooseRandomClothesMat();
            Material shoesMat = ChooseRandomClothesMat();
            Material hairMat = ChooseRandomHairMat();
            Material earringMat = ChooseRandomEarringsMat();
            Material pantsMat = ChooseRandomClothesMat();

            foreach(SkinnedMeshRenderer meshRenderer in meshRenderers){
                string objectName = meshRenderer.gameObject.name;

                // Have to assign the entire array at once
                // so store a local version to make changes to and then replace it on the object at the end
                Material[] localMatArray = meshRenderer.materials;
                
                switch(objectName){
                    case "Punk_Body":
                        localMatArray[0] = vestMat;                     // vest
                        localMatArray[1] = shirtMat;                    // shirt
                        localMatArray[2] = skinMat;                     // skin

                        break;

                    case "Punk_Feet":
                        localMatArray[0] = shoesMat;                    // shoes
                        localMatArray[1] = skinMat;                     // skin

                        break;

                    case "Punk_Head":
                        localMatArray[0] = skinMat;                     // skin
                        localMatArray[1] = hairMat;                     // hair
                        localMatArray[2] = earringMat;                  // earrings
                        localMatArray[3] = hairMat;                     // hair

                        break;

                    case "Punk_Legs":
                        localMatArray[0] = pantsMat;                    // pants
                        localMatArray[1] = skinMat;                     // skin

                        break;

                    default:
                        Debug.LogError("No case found for object name: " + objectName);
                        break;
                }

                meshRenderer.materials = localMatArray;
            }
        }

        private Material ChooseRandomClothesMat()
        {
            List<Material> clothesMats = GameManager.instance.enemyMaterialsData.GetPunkClothesMats();

            if(clothesMats.Count < 1){
                Debug.LogError("No punk enemy clothes materials found");
                return null;
            }
            
            int index = Random.Range(0, clothesMats.Count-1);
            return clothesMats[index];
        }

        private Material ChooseRandomSkinMat()
        {
            List<Material> skinMats = GameManager.instance.enemyMaterialsData.GetPunkSkinMats();

            if(skinMats.Count < 1){
                Debug.LogError("No punk enemy skin materials found");
                return null;
            }
            
            int index = Random.Range(0, skinMats.Count-1);
            return skinMats[index];
        }

        private Material ChooseRandomHairMat()
        {
            List<Material> hairMats = GameManager.instance.enemyMaterialsData.GetPunkHairMats();

            if(hairMats.Count < 1){
                Debug.LogError("No punk enemy hair materials found");
                return null;
            }
            
            int index = Random.Range(0, hairMats.Count-1);
            return hairMats[index];
        }

        private Material ChooseRandomEarringsMat()
        {
            List<Material> earringsMats = GameManager.instance.enemyMaterialsData.GetPunkEarringsMats();

            if(earringsMats.Count < 1){
                Debug.LogError("No punk enemy clothes materials found");
                return null;
            }
            
            int index = Random.Range(0, earringsMats.Count-1);
            return earringsMats[index];
        }
    #endregion
}
