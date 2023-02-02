using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected EnemyPathing enemyPathing;

    [SerializeField] protected Health health;
    [SerializeField] protected int attackValue = 1;
    [SerializeField] protected bool damageOnImpact;

    void OnCollisionEnter(Collision collision)
    {
        // TODO: if the collision is the player's gunshot, take that kind of damage
        // health.TakeDamage(GameManager.instance.player.gunAttackDamage);

        if(damageOnImpact && collision.gameObject.tag == "Player"){
            health.TakeDamage(GameManager.instance.player.motorcycleImpactDamage);
        }
    }
}
