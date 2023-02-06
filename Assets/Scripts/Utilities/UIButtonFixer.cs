using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Selectable))]
public class UIButtonFixer : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IDeselectHandler, IPointerClickHandler, ISubmitHandler
{
    // [SerializeField] private bool playSFXOnHover = true;
    // [SerializeField] private bool playSFXOnClick = true;
    
    // [FMODUnity.EventRef] public string buttonClickSFXOverride;

    private GameObject buttonIcon;

    private void SetIconIfNull()
    {
        if(!buttonIcon){
            buttonIcon = Instantiate( GameManager.instance.buttonIconPrefab, Vector2.zero, Quaternion.identity, GetComponentInChildren<TMP_Text>().transform );

            RectTransform parentTransform = buttonIcon.transform.parent.GetComponent<RectTransform>();
            float xVal = - parentTransform.rect.width / 2 - 40;
            buttonIcon.GetComponent<RectTransform>().localPosition = new Vector2( xVal, 0 );
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!EventSystem.current.alreadySelecting){
            EventSystem.current.SetSelectedGameObject(this.gameObject);
            // TriggerHoverSFX();

            SetIconIfNull();
            ToggleButtonIcon(true);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // TriggerClickSFX();

        SetIconIfNull();
        ToggleButtonIcon(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        // TriggerHoverSFX();
        SetIconIfNull();
        ToggleButtonIcon(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        this.GetComponent<Selectable>().OnPointerExit(null);

        SetIconIfNull();
        ToggleButtonIcon(false);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        // TriggerClickSFX();

        SetIconIfNull();
        ToggleButtonIcon(false);
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
        ToggleButtonIcon(true);
    }

    public void ToggleButtonIcon(bool set)
    {
        buttonIcon.SetActive(set);
    }
}
