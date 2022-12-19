using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


//SETTINGS MENU MUST BE INSTANTIATED BEFORE CONTROLS MENU
public class ControlsMenu : MonoBehaviour
{
    public MenuButtonManager backButton;
    public Image controlsBackground;
    public Sprite controlsEN;
    public Sprite controlsDK;

    public void StartCM(){
        TranslationManager.instance.languageChangeEvent.AddListener(RefreshText);
        RefreshText();
    }

    public void OnEnable(){
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(backButton.gameObject);   
        if (!backButton.isSelected) backButton.OnSelect(null); 
    }

    public void RefreshText(){
        //temp vars
        string key = SettingsLabel.Back.ToString();
        string newText = TranslationManager.instance.Get(key);
        if (newText != null) backButton.updateText(newText);

        Language lang = TranslationManager.instance.currLanguage;
        if (lang == Language.dk){
            controlsBackground.sprite = controlsDK;
        }
        else {
            controlsBackground.sprite = controlsEN;
        }
    }
}
