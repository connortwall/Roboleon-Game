using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class VolumeSlider : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public GameObject handleGlow;
    public MenuButtonManager label;
    
    public void Start(){
        handleGlow.SetActive(false);
        label.StartButton();
    }

    public void OnSelect(BaseEventData eventData)
    {
        handleGlow.SetActive(true);
        label.OnSelect(null);
        //click!
        AudioManager.instance.AddEvent(AudioEvents.instance.menuSelection);
    }
 
    public void OnDeselect(BaseEventData data)
    {
        label.OnDeselect(null);
        handleGlow.SetActive(false);
    }
}
