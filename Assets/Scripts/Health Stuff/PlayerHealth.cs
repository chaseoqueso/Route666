using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    public override void SetMaxHealth(int newMaxHealth)
    {
        base.SetMaxHealth(newMaxHealth);
    }

    public override void TakeDamage(int damageValue)
    {
        base.TakeDamage(damageValue);

        GameManager.instance.UIManager.healthUI.RemoveHealth();

        if(currentHealth <= 0){
            // TODO: Do death stuff

            Debug.Log("(you died)");
        }
    }

    public override void Heal(int healValue)
    {
        base.Heal(healValue);

        GameManager.instance.UIManager.healthUI.AddHealth();
    }
}
