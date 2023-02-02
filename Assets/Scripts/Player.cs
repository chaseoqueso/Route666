using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerHealth health;
    
    public int gunAttackValue = 1;
    public int motorcycleImpactDamage = 1;

    void Awake()
    {
        if(!GameManager.instance){
            Debug.LogWarning("No Game Manager found in scene!");
            return;
        }
        GameManager.instance.player = this;
    }

    public Health GetPlayerHealth()
    {
        return health;
    }
}
