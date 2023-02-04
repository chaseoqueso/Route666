using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public PlayerHealthUITracker healthUI;    
    
    public PauseMenu pauseMenu;
    public LevelClearUI levelClearUI;
    public DeathScreen deathUI;

    public Slider ruckusMeter;

    void Awake()
    {
        if(!GameManager.instance){
            Debug.LogError("No Game Manager found in scene!");
            return;
        }
        GameManager.instance.UIManager = this;

        healthUI.RefillAllHealth();
        
        ruckusMeter.maxValue = GameManager.instance.maxRuckusMeter;
        ruckusMeter.value = 0;
    }

    #region Ruckus Meter
        public void IncreaseRuckusMeter(int value)
        {
            ruckusMeter.value += value;
        }

        public void SetRuckusMeter(int value)
        {
            ruckusMeter.value = value;
        }

        public void DecreaseRuckusMeter(int value)
        {
            ruckusMeter.value -= value;
        }

        // public void RuckusDecay()
        // {

        // }
    #endregion
}
