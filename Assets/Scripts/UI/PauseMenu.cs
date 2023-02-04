using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public static bool gameIsPaused;

    void Start()
    {
        gameIsPaused = false;
        GameManager.instance.UIManager.SetCursorActive(false);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        GameManager.instance.UIManager.SetCursorActive(true);

        pauseMenuUI.SetActive(true);
        gameIsPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        GameManager.instance.UIManager.SetCursorActive(false);

        pauseMenuUI.SetActive(false);
        gameIsPaused = false;
    }

    public void ReturnToMenu()
    {
        GameManager.instance.ChangeScene(GameManager.MAIN_MENU_SCENE_NAME);
    }
}
