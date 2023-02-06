using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelClearUI : MonoBehaviour
{
    public GameObject levelClearUI;
    public static bool levelClearUIActive;

    [SerializeField] private Button continueButton;

    public void ActivateLevelClearUI()
    {
        Time.timeScale = 0f;
        GameManager.instance.UIManager.SetCursorActive(true);

        levelClearUI.SetActive(true);
        levelClearUIActive = true;

        continueButton.GetComponent<UIButtonFixer>().SelectOnMenuSwitch();
    }

    public void ContinueGame()
    {
        levelClearUIActive = false;
        
        // TEMP
        GameManager.instance.ChangeScene(GameManager.MAIN_MENU_SCENE_NAME);

        // TODO: Go to next level
    }
}
