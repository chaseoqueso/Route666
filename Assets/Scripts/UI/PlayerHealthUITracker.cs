using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUITracker : MonoBehaviour
{
    [SerializeField] private GameObject healthIconPrefab;

    [SerializeField] private Stack<GameObject> healthIconList = new Stack<GameObject>();

    public void RemoveHealth()
    {
        if(healthIconList.Count > 0){
            Destroy(healthIconList.Pop());
        }
    }

    public void AddHealth()
    {
        GameObject newIcon = Instantiate( healthIconPrefab, new Vector3(0,0,0), Quaternion.identity, GetComponent<Transform>() );
        healthIconList.Push( newIcon );
    }

    public void RefillAllHealth()
    {
        int maxHealth = GameManager.instance.player.GetPlayerHealth().GetMaxHealth();

        if(healthIconList.Count > maxHealth){
            Debug.LogError("Health icon count > max health");
        }

        for(int i = healthIconList.Count; i < maxHealth; i++){
            AddHealth();
        }
    }
}
