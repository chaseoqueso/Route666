using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void PlayGame ()
    {
        GameManager.instance.ChangeScene(GameManager.CUSTOMIZE_HORSE_SCENE_NAME);
    }

    public void QuitGame ()
    {
    //    Debug.Log ("QUIT!");
        Application.Quit();
    }
}
