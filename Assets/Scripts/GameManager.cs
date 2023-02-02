using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [HideInInspector] public Player player;
    public string horseName {get; private set;}

    [HideInInspector] public int ruckusPoints;
    public int maxRuckusMeter;

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
    }

    public void SetHorseName(string input)
    {
        horseName = input;
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        currentScene = sceneName;
    }
}
