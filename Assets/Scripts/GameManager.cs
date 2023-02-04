using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum KillType{
    normalGunKill,
    collisionKill,
    driftKill,
    midairKill,
    environmentalKill,

    enumSize
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    #region Scene Name Strings
        // CONFIRM THESE ARE SPELLED /EXACTLY/ AS THE SCENE NAMES IN THE EDITOR
        public const string MAIN_MENU_SCENE_NAME = "MainMenu";
        public const string CUSTOMIZE_HORSE_SCENE_NAME = "CustomizeHorseUI";
        public const string INTRO_SCENE_NAME = "IntroCutscene";
        public const string LEVEL_1_SCENE_NAME = "Level1";
    #endregion

    public Player player;
    public string horseName {get; private set;}

    public EnemySpawnManager spawnManager;
    public GameUIManager UIManager;

    #region Ruckus
        public int ruckusPoints {get; private set;}
        public int maxRuckusMeter {get; private set;}

        private int ruckusFromNormalGunKill;
        private int ruckusFromCollisionKill;
        private int ruckusDriftKill;
        private int ruckusFromMidairKill;
        private int ruckusFromEnvironmentalKill;

        private int ruckusDecayValue;
        private float ruckusDecayDelay;
        private Coroutine ruckusDecayRoutine;
    #endregion

    public string currentScene {get; private set;}

    void Awake()
    {
        if( instance ){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);

        currentScene = SceneManager.GetActiveScene().name;

        ruckusPoints = 0;
        maxRuckusMeter = 250;

        ruckusFromNormalGunKill = 1;
        ruckusFromCollisionKill = 2;
        ruckusDriftKill = 3;
        ruckusFromMidairKill = 5;
        ruckusFromEnvironmentalKill = 5;

        ruckusDecayValue = 2;
        ruckusDecayDelay = 2f;
    }

    public void SetHorseName(string input)
    {
        horseName = input;
    }

    #region Scene Management
        public void ChangeScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
            currentScene = sceneName;

            ruckusDecayRoutine = null;
        }

        public bool CurrentSceneIsALevel()
        {
            if(currentScene == LEVEL_1_SCENE_NAME){
                return true;
            }
            return false;
        }
    #endregion

    #region Ruckus Management
        public void IncreaseRuckusValue(KillType killType)
        {
            int value = 0;

            switch(killType){
                case KillType.normalGunKill:
                    value = ruckusFromNormalGunKill;
                    break;
                case KillType.collisionKill:
                    value = ruckusFromCollisionKill;
                    break;
                case KillType.driftKill:
                    value = ruckusDriftKill;
                    break;
                case KillType.midairKill:
                    value = ruckusFromMidairKill;
                    break;
                case KillType.environmentalKill:
                    value = ruckusFromEnvironmentalKill;
                    break;
            }

            ruckusPoints += value;

            if(ruckusPoints >= maxRuckusMeter){
                SetRuckusValue(maxRuckusMeter);
                TriggerBossFight();
                return;
            }

            UIManager.IncreaseRuckusMeter(value);
        }

        public void SetRuckusValue(int value)
        {
            ruckusPoints = value;
            UIManager.SetRuckusMeter(ruckusPoints);
        }

        public void DecreaseRuckusValue(int value)
        {
            ruckusPoints -= value;
            UIManager.DecreaseRuckusMeter(value);
        }

        public void ToggleRuckusDecay(bool decayActive)
        {
            if(decayActive){
                ruckusDecayRoutine = StartCoroutine(RuckusDecay());
            }
            else{
                StopCoroutine(ruckusDecayRoutine);
                ruckusDecayRoutine = null;
            }
        }

        private IEnumerator RuckusDecay()
        {
            yield return new WaitForSecondsRealtime(ruckusDecayDelay);
            DecreaseRuckusValue(ruckusDecayValue);

            if(ruckusPoints < 0){
                SetRuckusValue(0);
            }

            if(ruckusPoints == 0){
                ruckusDecayRoutine = null;
                yield break;
            }
        }
    
        public void TriggerBossFight()
        {
            // TODO: Trigger boss fight based on scene

            // TEMP
            UIManager.levelClearUI.ActivateLevelClearUI();
        }
    #endregion
}
