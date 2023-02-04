using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelClearUI : MonoBehaviour
{
    public GameObject levelClearUI;
    public static bool levelClearUIActive;

    public void ActivateLevelClearUI()
    {
        Time.timeScale = 0f;
        GameManager.instance.UIManager.SetCursorActive(true);

        levelClearUI.SetActive(true);
        levelClearUIActive = true;
    }

    public void ContinueGame()
    {
        levelClearUIActive = false;
        
        // TEMP
        GameManager.instance.ChangeScene(GameManager.MAIN_MENU_SCENE_NAME);

        // TODO: Go to next level
    }
}
