using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomizeHorseMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Button continueButton;

    public void ContinueClicked()
    {
        SaveCustomization();
        // TODO: Tell the Game Manager to go to the intro cutscene and the level 1
        // GameManager.instance.ChangeScene(GameManager.INTRO_SCENE_NAME);
    }

    public void SaveCustomization()
    {
        GameManager.instance.SetHorseName(nameText.text);
    }

    public void CheckForValidHorseName()
    {
        if(nameText.text == ""){
            continueButton.interactable = false;
        }
        else{
            continueButton.interactable = true;
        }
    }

    public void BackToMenuClicked()
    {
        GameManager.instance.ChangeScene(GameManager.MAIN_MENU_SCENE_NAME);
    }
}
