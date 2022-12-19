using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SettingsButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        //click!
        AudioManager.instance.AddEvent(AudioEvents.instance.menuSelection);
    }
 
    public void OnDeselect(BaseEventData data)
    {
    }
}
