using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IShootable
{
    [SerializeField] protected EnemyPathing enemyPathing;

    [SerializeField] protected int maxHealth = 1;
    public int currentHealth {get; private set;}

    [SerializeField] protected int attackValue = 1;
    [SerializeField] protected bool damageOnImpact;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(damageOnImpact && collision.gameObject.tag == "Player"){
            TakeDamage(GameManager.instance.player.motorcycleImpactDamage, KillType.collisionKill);
        }
    }

    public void OnShoot()
    {
        TakeDamage(GameManager.instance.player.gunDamage, KillType.normalGunKill);
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

        public virtual void TakeDamage(int damageValue, KillType killType)
        {
            currentHealth -= damageValue;

            if(currentHealth <= 0){
                // TODO: Do death stuff

                Debug.Log("you killed someone!");

                Destroy(gameObject);
            }
        }

        public virtual void Heal(int healValue)
        {
            currentHealth += healValue;

            if(currentHealth > maxHealth){
                currentHealth = maxHealth;
            }
        }
    #endregion
}
