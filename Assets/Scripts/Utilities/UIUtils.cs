using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUtils : MonoBehaviour
{
    public static void SetImageColorFromHex( Image img, string hexCode )
    {
        Color color;
        if(ColorUtility.TryParseHtmlString( hexCode, out color )){
            img.color = color;
        }
        else{
            Debug.LogError("Failed to set color");
        }   
    }

    public static void SetImageVisible( Image img, bool setVisible )
    {
        if(setVisible){
            img.color = new Color(255,255,255,255);
        }
        else{
            img.color = new Color(255,255,255,0);
        }
    }
}
