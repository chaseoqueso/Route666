using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState{
    // enemy within maxDistanceFromSpawn + player w/in enemy's aggroRadius
    Aggro,  // enemy is following player and is going to try to melee you

    // when enemy gets within attack range
    Attacking, // enemy will STOP MOVING, start the attack anim, once attack anim is complete RESUME moving

    // if the enemy ever passes beyond maxDistanceFromSpawn:
        // > paths back to its spawn point, ignores the player in every way
        // > UNTIL enemy is within a certain distance from spawn point ("back home")
    Leashed,

    // when player is not nearby:
        // > randomly moves around (at a WALKING SPEED, not MAX RUNNING SPEED) (picks a nearby
        // location and walks there, stays for a bit, then walks some more)
    Idle,   // Not moving
    Wander, // Moving randomly

    enumSize
}

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour, IShootable
{
    protected EnemyState enemyState = EnemyState.Idle;

    [SerializeField] protected int maxHealth = 1;
    public int currentHealth {get; private set;}

    [SerializeField] protected int attackValue = 1;

    [Tooltip("Whether or not this enemy takes damage from being hit by the player's motorcycle")]
    [SerializeField] protected bool damageOnImpact = true;

    [Tooltip("The radius outside of which the enemy will return to spawn")]
    [SerializeField] protected float maxDistanceFromSpawn = 50f;
    [Tooltip("'Back Home' range - when returning to spawn, at what radius out from there does this enemy behave as normal again")]
    [SerializeField] protected float stopRetreatRange = 25f;
    [Tooltip("Radius around this enemy within which it will start following/attacking the player")]
    [SerializeField] protected float aggroRadius = 20f;

    [Tooltip("IGNORE SPEED IN THE NavAgent COMPONENT - use this instead")]
    [SerializeField] protected float walkSpeed = 2.5f;
    [Tooltip("IGNORE SPEED IN THE NavAgent COMPONENT - use this instead")]
    [SerializeField] protected float runSpeed = 3.5f;

    [SerializeField] protected float idleDurationInSeconds = 5;
    protected Coroutine idleRoutine;

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
            if(currentHealth > 0){
                currentHealth -= damageValue;

                if(currentHealth <= 0){
                    spawnPoint.UpdatePopOnEnemyDeath();
                    GameManager.instance.IncreaseRuckusValue(killType);
                    Destroy(gameObject);
                }
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
