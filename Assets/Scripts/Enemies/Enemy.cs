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

public enum EnemyID{
    punkMale,
    punkFemale,

    GreezeyHogg
}

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour, IShootable
{
    [SerializeField] private EnemyID enemyID;

    public static List<GameObject> ragdollList;

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
    
    [Tooltip("The ragdoll model of the enemy")]
    [SerializeField] protected GameObject ragdoll;

    [SerializeField] protected float idleDurationInSeconds = 5;
    protected Coroutine idleRoutine;

    [HideInInspector] public EnemySpawner spawnPoint;
    [SerializeField] protected NavMeshAgent enemyAgent;
    [HideInInspector] public Transform playerLoc;

    [SerializeField] protected Animator enemyAnimator;

    public virtual void Start()
    {
        if(ragdollList == null)
            ragdollList = new List<GameObject>();

        currentHealth = maxHealth;
        playerLoc = GameManager.instance.player.transform;

        enemyAgent.SetDestination(playerLoc.position);
    }

    // void OnCollisionEnter(Collision collision)
    // {
    //     if(damageOnImpact && collision.gameObject.tag == "Player"){
    //         PlayerMovement player;
    //         if(collision.gameObject.TryGetComponent<PlayerMovement>(out player) && player.EnemyImpactIsValid(this)){
    //             TakeDamage(GameManager.instance.player.motorcycleImpactDamage, KillType.collisionKill);
    //         }
    //     }
    // }

    public void OnShoot()
    {
        TakeDamage(GameManager.instance.player.gunDamage, KillType.normalGunKill);
    }

    public EnemyID EnemyID()
    {
        return enemyID;
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

                    Destroy(gameObject, 10);

                    ragdoll.SetActive(true);
                    ragdoll.transform.parent = null;
                    ragdoll.GetComponentInChildren<Rigidbody>().velocity = ((transform.position - playerLoc.position).normalized * 15 + Vector3.up * 10);

                    ragdollList.Insert(0, ragdoll);
                    while(ragdollList.Count > 10)
                    {
                        Destroy(ragdollList[10]);
                        ragdollList.RemoveAt(10);
                    }

                    GetComponent<Collider>().enabled = false;
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
