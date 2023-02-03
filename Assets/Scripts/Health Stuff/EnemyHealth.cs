using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    public override void TakeDamage(int damageValue)
    {
        base.TakeDamage(damageValue);

        if(currentHealth <= 0){
            // TODO: Do death stuff

            Destroy(gameObject);
        }
    }

    public override void Heal(int healValue)
    {
        base.Heal(healValue);
    }
}
