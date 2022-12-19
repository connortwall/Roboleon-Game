using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class LanguageToggleButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public MenuButtonManager label;
    public GameObject buttonGlow;
    public bool noClick = false;
    // Start is called before the first frame update
    void Start()
    {
        label.StartButton();
        buttonGlow.SetActive(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        //Debug.Log("LanguageToggleButton");
        label.OnSelect(null);
        buttonGlow.SetActive(true);
        //click!
        if (noClick){
            noClick = false;
            return;
        }
        AudioManager.instance.AddEvent(AudioEvents.instance.menuSelection);
    }
 
    public void OnDeselect(BaseEventData data)
    {
        label.OnDeselect(null);
        buttonGlow.SetActive(false);
    }
}
