using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBottle : MonoBehaviour, IShootable
{
    public int healValue = 1;
   public void OnShoot()
   {
    // bottle should be destroyed, disappear from the game, health increase after shooting
    GameManager.instance.player.Heal(healValue);
    Destroy(gameObject);
   }
}
