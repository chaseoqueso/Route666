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
    [SerializeField] protected float maxDistanceFromSpawn = 50f;   
    [SerializeField] protected float aggroRadius = 20f;

    /*
        - enemy within maxDistanceFromSpawn + player w/in enemy's aggroRadius:
            > enemy is following player and is going to try to melee you

        - when enemy gets within attack range
            > then the enemy will STOP MOVING, start the attack anim, once attack anim is complete
            it will RESUME moving

        - if the enemy ever passes beyond maxDistanceFromSpawn, it is now "leashed"
            > paths back to its spawn point
            > ignores the player in every way
            > UNTIL enemy is within a certain distance from spawn point ("back home")

        - "in a perfect world, max polish" when player is not nearby:
            > idle
            > randomly moves around (at a WALKING SPEED, not MAX RUNNING SPEED) (picks a nearby
            location and walks there, stays for a bit, then walks some more)
    */

    [HideInInspector] public EnemySpawner spawnPoint;

    [SerializeField] protected NavMeshAgent enemyAgent;
    [HideInInspector] public Transform playerLoc;

    [SerializeField] protected Animator enemyAnimator;

    void Awake()
    {
        currentHealth = maxHealth;
        playerLoc = GameManager.instance.player.transform;

        enemyAgent.SetDestination(playerLoc.position);
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
