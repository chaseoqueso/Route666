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
    public Image ruckusMeterFill;

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
            if(ruckusMeter.value == 0 && value > 0){
                ToggleRuckusFillVis(true);
            }

            ruckusMeter.value += value;
        }

        public void SetRuckusMeter(int value)
        {
            if(ruckusMeter.value == 0 && value > 0){
                ToggleRuckusFillVis(true);
            }

            ruckusMeter.value = value;

            if(ruckusMeter.value == 0){
                ToggleRuckusFillVis(false);
            }
        }

        public void DecreaseRuckusMeter(int value)
        {
            ruckusMeter.value -= value;

            if(ruckusMeter.value == 0){
                ToggleRuckusFillVis(false);
            }
        }

        public void ToggleRuckusFillVis( bool set )
        {
            ruckusMeterFill.enabled = set;
        }

        // public void RuckusDecay()
        // {

        // }
    #endregion
}
