using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Tooltip("The whole parent pause menu with the dark background")]
    public GameObject pauseMenuUI;
    [Tooltip("JUST the panel with the pause and pause buttons, not controls")]
    [SerializeField] private GameObject pausePanel;

    public static bool gameIsPaused;

    [SerializeField] private GameObject controlsUI;
    private bool controlsUIActive;

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button controlsBackButton;

    void Start()
    {
        gameIsPaused = false;
        GameManager.instance.UIManager.SetCursorActive(false);
        controlsUIActive = false;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        GameManager.instance.UIManager.SetCursorActive(true);

        pauseMenuUI.SetActive(true);
        gameIsPaused = true;

        resumeButton.GetComponent<UIButtonFixer>().SelectOnMenuSwitch();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        GameManager.instance.UIManager.SetCursorActive(false);

        pauseMenuUI.SetActive(false);
        gameIsPaused = false;

        if(controlsUIActive){
            ToggleControlsUI();
        }
    }

    public void ReturnToMenu()
    {
        GameManager.instance.ChangeScene(GameManager.MAIN_MENU_SCENE_NAME);
    }

    public void ToggleControlsUI()
    {
        controlsUIActive = !controlsUIActive;
        controlsUI.SetActive(controlsUIActive);

        pausePanel.SetActive(!controlsUIActive);

        if(controlsUIActive){
            controlsBackButton.GetComponent<UIButtonFixer>().SelectOnMenuSwitch();
        }
        else{
            resumeButton.GetComponent<UIButtonFixer>().SelectOnMenuSwitch();
        }
    }
}
