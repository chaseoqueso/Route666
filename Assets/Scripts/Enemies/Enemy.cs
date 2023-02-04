using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour, IShootable
{
    [SerializeField] protected int maxHealth = 1;
    public int currentHealth {get; private set;}

    [SerializeField] protected int attackValue = 1;
    [SerializeField] protected bool damageOnImpact;

    [Tooltip("The radius outside of which the enemy will return to spawn")]
    [SerializeField] private float leashDistance = 50f;

    [HideInInspector] public EnemySpawner spawnPoint;

    [SerializeField] private NavMeshAgent enemyAgent;
    [HideInInspector] public Transform playerLoc;

    private bool followingPlayer;

    void Start()
    {
        currentHealth = maxHealth;
        playerLoc = GameManager.instance.player.transform;
    }

    void Update()
    {
        // If the player is out of range of the enemy, return to spawn
        if( Vector3.Distance( transform.position, playerLoc.position ) >= leashDistance ){
            followingPlayer = true;
        }
        else{
            followingPlayer = false;
        }

        if(followingPlayer){
            enemyAgent.SetDestination(playerLoc.position);
        }
        else{
            enemyAgent.SetDestination(spawnPoint.transform.position);
        }        
    }

    void OnCollisionEnter(Collision collision)
    {
        if(damageOnImpact && collision.gameObject.tag == "Player"){
            TakeDamage(GameManager.instance.player.motorcycleImpactDamage, KillType.collisionKill);
        }
    }

    public void OnShoot()
    {
        TakeDamage(GameManager.instance.player.gunDamage, KillType.normalGunKill);
    }

    #region Health Stuff
        public int GetMaxHealth()
        {
            return maxHealth;
        }

        public virtual void SetMaxHealth(int newMaxHealth)
        {
            maxHealth = newMaxHealth;
        }

        public virtual void TakeDamage(int damageValue, KillType killType)
        {
            currentHealth -= damageValue;

            if(currentHealth <= 0){
                spawnPoint.UpdatePopOnEnemyDeath();
                GameManager.instance.IncreaseRuckusValue(killType);
                Destroy(gameObject);
            }
        }

        public virtual void Heal(int healValue)
        {
            currentHealth += healValue;

            if(currentHealth > maxHealth){
                currentHealth = maxHealth;
            }
        }
    #endregion
}
