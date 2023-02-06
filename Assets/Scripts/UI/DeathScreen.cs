using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    public GameObject deathUI;
    public static bool deathUIActive;

    [SerializeField] private Button continueButton;

    public void ActivateDeathUI()
    {
        Time.timeScale = 0f;
        GameManager.instance.UIManager.SetCursorActive(true);

        deathUI.SetActive(true);
        deathUIActive = true;

        continueButton.GetComponent<UIButtonFixer>().SelectOnMenuSwitch();
    }

    public void ContinueGame()
    {
        deathUIActive = false;
        
        // TEMP
        GameManager.instance.ChangeScene(GameManager.MAIN_MENU_SCENE_NAME);

        // TODO: Reset this level
    }
}
