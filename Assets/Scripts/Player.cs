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
    public float invulnerabilityTimer = 2;
    public bool canTakeDamage = true;

    void Awake()
    {
        if(!GameManager.instance){
            Debug.LogError("No Game Manager found in scene!");
            return;
        }
        GameManager.instance.player = this;

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
            if(canTakeDamage)
            {
                currentHealth -= damageValue;

                for(int i = 0; i < damageValue; i++){
                    GameManager.instance.UIManager.healthUI.RemoveHealth();
                }

                if(currentHealth <= 0){
                    GameManager.instance.UIManager.deathUI.ActivateDeathUI();
                }

                canTakeDamage = false;
                IEnumerator hitTimer() {
                    yield return new WaitForSeconds(invulnerabilityTimer);
                    canTakeDamage = true;
                }
                StartCoroutine(hitTimer());
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
