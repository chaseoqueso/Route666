using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    [Tooltip("The range within which the enemy starts TRIGGERING the attack")]
    [SerializeField] private float attackRange = 2;
    [Tooltip("Range of attack (within which the player can actually be hit)")]
    [SerializeField] private float attackHitBoxSize = 4;

    void Start()
    {
        // The frame the animation would theoretically hit the player, do stuff
        GetComponentInChildren<AnimationWatcher>().onAnimationTrigger.AddListener(MeleeAttack);
    }

    public void Update()
    {
        enemyAnimator.SetFloat("Speed", enemyAgent.velocity.magnitude);

        // If the player is within the aggro radius, move toward the player
        if( Vector3.Distance( transform.position, playerLoc.position ) <= aggroRadius ){
            // check if the player is in range (if so, trigger attack)
            if( Vector3.Distance( transform.position, playerLoc.position ) <= attackRange ){
                enemyAnimator.SetTrigger("Attack");
            }
            else{
                enemyAgent.SetDestination(playerLoc.position);
            }
        }

        // If the enemy is out of range of their spawn point, tether them back home
        else if( Vector3.Distance( transform.position, spawnPoint.transform.position ) >= maxDistanceFromSpawn ){
            enemyAgent.SetDestination(spawnPoint.transform.position);
        }
    }
    
    public void MeleeAttack()
    {
        // Return all colliders it hit
        Collider[] collision = Physics.OverlapSphere( transform.position + transform.forward * attackHitBoxSize / 2, attackHitBoxSize/2, LayerMask.GetMask("Player") );

        // There should only be one on the Player layer ever so if length > 0 then it hit
        if(collision.Length > 0){
            GameManager.instance.player.TakeDamage(attackValue);
        }
    }
}
