using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    [SerializeField] private float attackRange = 4;

    void Start()
    {
        // The frame the animation would theoretically hit the player, do stuff
        GetComponentInChildren<AnimationWatcher>().onAnimationTrigger.AddListener(MeleeAttack);
    }

    public override void Update()
    {
        base.Update();

        // check if the player is in range (if so, trigger attack)
        if( Vector3.Distance( transform.position, playerLoc.position ) <= attackRange ){
            enemyAnimator.SetTrigger("Attack");
        }

        // TODO
        // set the speed parameter for the walking and running animation to match up with what the 
        // enemy is currently doing
    }
    
    public void MeleeAttack()
    {
        // Return all colliders it hit
        Collider[] collision = Physics.OverlapSphere( transform.position + transform.forward * attackRange / 2, attackRange/2, LayerMask.GetMask("Player") );

        // There should only be one on the Player layer ever so if length > 0 then it hit
        if(collision.Length > 0){
            GameManager.instance.player.TakeDamage(attackValue);
        }
    }
}
