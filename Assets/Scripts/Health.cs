using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 1;
    public int currentHealth {get; private set;}

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void SetCurrentHealth(int newCurrentHealth)
    {
        currentHealth = newCurrentHealth;
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public void TakeDamage(int damageValue)
    {
        currentHealth -= damageValue;

        if(currentHealth <= 0){
            // Do death stuff
        }
    }

    public void Heal(int healValue)
    {
        currentHealth += healValue;

        if(currentHealth > maxHealth){
            currentHealth = maxHealth;
        }
    }
}
