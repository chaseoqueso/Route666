using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player"){
            MeleeAttack();
        }
    }

    public void MeleeAttack()
    {
        // TODO: Play attack animation!

        // TODO: If it hits, do the following
        GameManager.instance.player.TakeDamage(attackValue);
    }
}
