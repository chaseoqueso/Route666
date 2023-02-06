using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class UIButtonFixer : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IDeselectHandler, IPointerClickHandler, ISubmitHandler
{
    // [SerializeField] private bool playSFXOnHover = true;
    // [SerializeField] private bool playSFXOnClick = true;
    
    // [FMODUnity.EventRef] public string buttonClickSFXOverride;

    private ButtonIcon buttonIcon;

    private void SetIconIfNull()
    {
        if(!buttonIcon){
            buttonIcon = GetComponentInChildren<ButtonIcon>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!EventSystem.current.alreadySelecting){
            EventSystem.current.SetSelectedGameObject(this.gameObject);
            // TriggerHoverSFX();

            SetIconIfNull();
            buttonIcon.ToggleIcon(true);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // TriggerClickSFX();

        SetIconIfNull();
        buttonIcon.ToggleIcon(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        // TriggerHoverSFX();
        SetIconIfNull();
        buttonIcon.ToggleIcon(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        this.GetComponent<Selectable>().OnPointerExit(null);

        SetIconIfNull();
        buttonIcon.ToggleIcon(false);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        // TriggerClickSFX();

        SetIconIfNull();
        buttonIcon.ToggleIcon(false);
    }

    private void TriggerHoverSFX()
    {
        // if(playSFXOnHover)
        //     AudioManager.Instance.PlaySFX(AudioManager.SFX.ButtonHover);
    }

    private void TriggerClickSFX()
    {
        // if(playSFXOnClick){
        //     if(buttonClickSFXOverride != ""){
        //         AudioManager.Instance.PlaySFX(buttonClickSFXOverride);
        //     }
        //     else{
        //         AudioManager.Instance.PlaySFX(AudioManager.SFX.ButtonClick);
        //     }            
        // }
    }

    // For each menu, do Button.GetComponent<UIButtonFixer>().SelectOnMenuSwitch(); when going to that menu
    public void SelectOnMenuSwitch()
    {
        GetComponent<Selectable>().Select();
        
        SetIconIfNull();
        buttonIcon.ToggleIcon(true);
    }
}
