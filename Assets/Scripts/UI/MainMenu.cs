using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button creditsBackButton;
    [SerializeField] private Button controlsBackButton;

    void Start()
    {
        // ActivateDefaultHorseshoe();
    }

    public void ActivateDefaultHorseshoe()
    {
        playButton.GetComponent<UIButtonFixer>().SelectOnMenuSwitch();
    }

    public void ActivateCreditsHorseshoe()
    {
        creditsBackButton.GetComponent<UIButtonFixer>().SelectOnMenuSwitch();
    }

    public void ActivateControlsHorseshoe()
    {
        controlsBackButton.GetComponent<UIButtonFixer>().SelectOnMenuSwitch();
    }

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
