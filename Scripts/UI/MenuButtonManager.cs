using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MenuButtonManager : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public Color selectedColor;
    public Color deselectedColor;

    public TMP_Text buttonText;
    public GameObject glow;
    public TMP_Text glowText;

    [HideInInspector]
    public bool initButton = false;
    [HideInInspector]
    public bool startButton = false;
    [HideInInspector]
    public bool isSelected = false;

    public void StartButton(){
        OnDeselect(null);
    }
    
    public void OnSelect(BaseEventData eventData)
    {   
        //Debug.Log($"OnSelect {buttonText.text}");
        isSelected = true;
        glow.SetActive(true);
        buttonText.color = selectedColor;
        //beep!
        if (initButton) AudioManager.instance.AddEvent(AudioEvents.instance.menuSelection);
        else initButton = true;
    }
 
    public void OnDeselect(BaseEventData data)
    {
        isSelected = false;
        glow.SetActive(false);
        buttonText.color = deselectedColor;
    }

    public void onClickAction(){
        //beep!
        if (startButton){
            AudioManager.instance.AddEvent(AudioEvents.instance.startGameButton);
        } 
        else{
            AudioManager.instance.AddEvent(AudioEvents.instance.menuButtonPress);
        }
        
    }

    public void OnDisable()
    {
        OnDeselect(null);
    }

    public void updateText(string newText){
        buttonText.text = newText;
        glowText.text = newText;
    }
}
