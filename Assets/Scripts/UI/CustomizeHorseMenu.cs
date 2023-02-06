using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomizeHorseMenu : MonoBehaviour
{
    // DOES THIS NEED DEFAULT HORSESHOE STATE???

    [SerializeField] private Button continueButton;
    [SerializeField] private TMP_Text continueButtonText;
    [SerializeField] private TMP_InputField nameInputField;

    [SerializeField] private Color buttonActiveColor;
    [SerializeField] private Color buttonDisabledColor;

    void Start()
    {
        nameInputField.onValueChanged.AddListener(delegate{
            OnHorseNameInputChanged();
        });
    }

    public void ContinueClicked()
    {
        SaveCustomization();

        Debug.Log("(assign scene to transition to)");

        // TODO: Tell the Game Manager to go to the intro cutscene and the level 1
        // GameManager.instance.ChangeScene(GameManager.INTRO_SCENE_NAME);
    }

    public void SaveCustomization()
    {
        GameManager.instance.SetHorseName(nameInputField.text);
        Debug.Log("horse name saved as: " + GameManager.instance.horseName);
    }

    public void OnHorseNameInputChanged()
    {
        if(nameInputField.text == ""){
            continueButton.interactable = false;
            continueButtonText.color = buttonDisabledColor;
        }
        else{
            nameInputField.text = nameInputField.text.ToUpper();

            continueButton.interactable = true;
            continueButtonText.color = buttonActiveColor;
        }
    }

    public void BackToMenuClicked()
    {
        GameManager.instance.ChangeScene(GameManager.MAIN_MENU_SCENE_NAME);
    }
}
