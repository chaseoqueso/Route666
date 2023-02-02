using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] protected int maxHealth = 1;
    public int currentHealth {get; private set;}

    void Start()
    {
        currentHealth = maxHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public virtual void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
    }

    public virtual void TakeDamage(int damageValue)
    {
        currentHealth -= damageValue;
    }

    public virtual void Heal(int healValue)
    {
        currentHealth += healValue;

        if(currentHealth > maxHealth){
            currentHealth = maxHealth;
        }
    }
}
