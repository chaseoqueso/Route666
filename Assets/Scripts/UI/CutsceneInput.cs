using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class CutsceneInput : MonoBehaviour
{
    private bool skipClickedOnce = false;
    
    [SerializeField] private TMP_Text skipText;
    [SerializeField] private Cutscene cutscene;

    public void OnSkipCutscene(InputValue input)
    {
        if(skipClickedOnce){
            cutscene.EndCutscene();
        }
        else{
            skipClickedOnce = true;
            skipText.gameObject.SetActive(true);
        }
    }
}
