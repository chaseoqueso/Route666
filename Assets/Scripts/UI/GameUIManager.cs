using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public PlayerHealthUITracker healthUI;
    
    public PauseMenu pauseMenu;

    public Slider ruckusMeter;

    void Awake()
    {
        if(!GameManager.instance){
            Debug.LogWarning("No Game Manager found in scene!");
            return;
        }
        GameManager.instance.UIManager = this;
    }

    void Start()
    {
        healthUI.RefillAllHealth();
    }

    #region Ruckus Meter
        public void IncreaseRuckusPoints()
        {

        }

        public void DecreaseRuckusOverTime()
        {

        }
    #endregion
}
