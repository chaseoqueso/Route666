using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] protected int maxHealth = 3;
    public int currentHealth {get; private set;}
    
    public int gunDamage = 1;
    public int motorcycleImpactDamage = 1;

    void Awake()
    {
        if(!GameManager.instance){
            Debug.LogWarning("No Game Manager found in scene!");
            return;
        }
        GameManager.instance.player = this;
    }

    void Start()
    {
        currentHealth = maxHealth;
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

        public virtual void TakeDamage(int damageValue)
        {
            currentHealth -= damageValue;

            if(currentHealth <= 0){
                // TODO: Do death stuff
                Debug.Log("(you died)");
                return;
            }

            for(int i = 0; i < damageValue; i++){
                GameManager.instance.UIManager.healthUI.RemoveHealth();
            }
        }

        public virtual void Heal(int healValue)
        {
            currentHealth += healValue;

            if(currentHealth > maxHealth){
                currentHealth = maxHealth;
                GameManager.instance.UIManager.healthUI.RefillAllHealth();
                return;
            }

            for(int i = 0; i < healValue; i++){
                GameManager.instance.UIManager.healthUI.AddHealth();
            }
        }
    #endregion
}
