using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    [Tooltip("The range within which the enemy starts TRIGGERING the attack")]
    [SerializeField] protected float meleeAttackRange = 2;
    [Tooltip("Range of attack (within which the player can actually be hit)")]
    [SerializeField] protected float meleeAttackHitBoxSize = 4;

    void Start()
    {
        // The frame the animation would theoretically hit the player, do stuff
        GetComponentInChildren<AnimationWatcher>().onAnimationTrigger.AddListener(MeleeAttack);
    }

    public void Update()
    {
        enemyAnimator.SetFloat("Speed", enemyAgent.velocity.magnitude);

        switch(enemyState){
            case EnemyState.Aggro:
                // Enemy is currently within maxDistanceFromSpawn + player is w/in enemy's aggroRadius, trying to attack when possible

                // If the enemy is out of range of their spawn point, tether them back home
                if( EnemyIsOutOfRangeOfEntity(spawnPoint.transform.position, maxDistanceFromSpawn) ){
                    TransitionState(EnemyState.Leashed);
                }
                // Check if the player is in range (if so, trigger attack)
                else if( EntityIsWithinRadiusOfEnemy(playerLoc.position, meleeAttackRange) ){
                    TransitionState(EnemyState.Attacking);
                }
                // If none of that applies, just continue following the player
                else{
                    enemyAgent.SetDestination(playerLoc.position);
                }

                break;

            case EnemyState.Attacking:
                // Attack state transition back to aggro handled at the end of the attack
                break;
            
            case EnemyState.Leashed:
                // If enemy is close enough to spawn to become a normal enemy again, transition to that state
                if( EntityIsWithinRadiusOfEnemy(spawnPoint.transform.position, stopRetreatRange) ){
                    // TODO:
                    // TransitionState(EnemyState.Wander);

                    // TEMP
                    TransitionState(EnemyState.Idle);
                }
                
                break;
            
            case EnemyState.Idle:
                // If player is in aggroRadius, switch to that state
                if(CheckForPlayerInAggroRadius()){
                    StopCoroutine(IdleCountdownRoutine());
                    idleRoutine = null;
                }
                break;

            case EnemyState.Wander:
                // If player is in aggroRadius, switch to that state now and break from this case
                if(CheckForPlayerInAggroRadius()){
                    break;
                }

                // Otherwise... If they reached their Wander destination, go Idle
                if(transform.position == enemyAgent.destination){   // DOES THIS WORK???
                    TransitionState(EnemyState.Idle);
                }

                break;
        }
    }

    private void TransitionState( EnemyState newState )
    {
        switch(newState){
            case EnemyState.Aggro:
                enemyAgent.isStopped = false;   // DOES THIS WORK??? if not, set destination to current pos?
                enemyAgent.speed = runSpeed;
                break;

            case EnemyState.Attacking:
                enemyAgent.isStopped = true;
                enemyAnimator.SetTrigger("Attack");
                break;

            case EnemyState.Leashed:
                enemyAgent.isStopped = false;
                enemyAgent.speed = runSpeed;    // Maybe?
                enemyAgent.SetDestination(spawnPoint.transform.position);   // Does it work to just put this here? (not update?)
                break;
            
            case EnemyState.Idle:
                enemyAgent.isStopped = true;
                idleRoutine = StartCoroutine(IdleCountdownRoutine());
                break;

            case EnemyState.Wander:
                enemyAgent.isStopped = false;
                enemyAgent.speed = walkSpeed;

                // TODO: Pick a nearby random location as the destination
                // enemyAgent.SetDestination()  // Does it work to just put this here? (not update?)

                break;
        }

        enemyState = newState;
    }

    private IEnumerator IdleCountdownRoutine()
    {
        yield return new WaitForSecondsRealtime(idleDurationInSeconds);
        idleRoutine = null;

        // TODO: Switch to wander
        // TransitionState(EnemyState.Wander);
    }

    private bool CheckForPlayerInAggroRadius()
    {
        // If the player is within the aggro radius, move toward the player
        if( EntityIsWithinRadiusOfEnemy(playerLoc.position, aggroRadius) ){
            TransitionState(EnemyState.Aggro);
            return true;
        }
        return false;
    }

    private bool EntityIsWithinRadiusOfEnemy( Vector3 otherEntityPos, float radius )
    {
        return Vector3.Distance( transform.position, otherEntityPos ) <= radius;
    }

    private bool EnemyIsOutOfRangeOfEntity( Vector3 otherEntityPos, float radius )
    {
        return Vector3.Distance( transform.position, otherEntityPos ) >= radius;
    }
    
    public void MeleeAttack()
    {
        // Return all colliders it hit
        Collider[] collision = Physics.OverlapSphere( transform.position + transform.forward * meleeAttackHitBoxSize / 2, meleeAttackHitBoxSize/2, LayerMask.GetMask("Player") );

        // There should only be one on the Player layer ever so if length > 0 then it hit
        if(collision.Length > 0){
            GameManager.instance.player.TakeDamage(attackValue);
        }

        TransitionState(EnemyState.Aggro);
    }
}
