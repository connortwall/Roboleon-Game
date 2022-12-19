using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;


public class ButtonObject : OnActionObject
{
    //REMINDER: NO UPDATE FUNCTION ALLOWED WITHIN THIS SCRIPT\
    //EXCEPTION: FIXEDUPDATE MAY BE USED FOR PHYSICS RELATED TASKS ONLY

    [Header("Button Settings")]
    public bool buttonEnabled = false;
    public List<GameObject> objectsToActivate = new List<GameObject>();

    [Header("Lights")]
    public Material pressedLight;
    public Material enabledLight;
    public Material disabledLight;

    [Header("Double Button")]
    public bool doubleButton = false;
    public List<GameObject> objectsToActivate2 = new List<GameObject>();

    [HideInInspector]
    public GameObject baseObj;
    [HideInInspector]
    public UnityEvent buttonPressEvent = new UnityEvent();
    [HideInInspector]
    public bool pressed;
    
    /**
     * Start may only be used for actions
     * that are solely for the object this script is
     * attached to. No other object in the game may
     * rely on the actions in this start function
     */
    public void Start() {
        baseObj = transform.Find("Base").gameObject;
        Assert.IsNotNull(baseObj);

        if (buttonEnabled) UpdateLight(enabledLight);
        else UpdateLight(disabledLight);

        pressed = false;
    }

    /** 
     * this is the action that will occur if another
     * button has this button in it's onaction list
     */
    public override void OnAction(){
        //only allow activation a single time
        if (buttonEnabled) return;
        buttonEnabled = true;
        UpdateLight(enabledLight);
    }

    /**
     * this function is called when the button is pressed 
     * it calls the OnAction() function in of all gameobjects
     * in objectsToActivate List
     */
    public virtual void OnPress(){
        //reject if the button is not enabled 
        if (!buttonEnabled) {
            //play disabled sound then exit
            AudioManager.instance.AddEvent(AudioEvents.instance.disabledButtonPress);
            return;
        }

        //first press of double button!
        else if (doubleButton && !pressed){
            //play broken button sound and change light
            AudioManager.instance.AddEvent(AudioEvents.instance.brokenButtonPress);
            UpdateLight(disabledLight);
            pressed = true;
            buttonEnabled = false;

            OnActionObject tempObj;
            foreach (GameObject obj in objectsToActivate){
                tempObj = obj.GetComponent<OnActionObject>();
                if (tempObj == null) continue;
                tempObj.OnAction();
            }
            //notify anyone listening we got pushed
            buttonPressEvent.Invoke();
            return;
        }

        //2nd press on double button!
        else if (doubleButton && pressed){
            //play buttonEnabled sound and change light
            AudioManager.instance.AddEvent(AudioEvents.instance.enabledButtonPress);
            UpdateLight(pressedLight);

            OnActionObject tempObj;
            foreach (GameObject obj in objectsToActivate2){
                tempObj = obj.GetComponent<OnActionObject>();
                if (tempObj == null) continue;
                tempObj.OnAction();
            }
            //notify anyone listening we got pushed
            buttonPressEvent.Invoke();
        }

        //normal button!
        else {
            //play buttonEnabled sound and change light
            AudioManager.instance.AddEvent(AudioEvents.instance.enabledButtonPress);
            UpdateLight(pressedLight);

            pressed = true;
            OnActionObject tempObj;
            foreach (GameObject obj in objectsToActivate){
                tempObj = obj.GetComponent<OnActionObject>();
                if (tempObj == null) continue;
                tempObj.OnAction();
            }
            //notify anyone listening we got pushed
            buttonPressEvent.Invoke();
        }
    }

/************* HELPER FUNCTIONS BELOW ************/

    protected void UpdateLight(Material mat){
        if (baseObj == null) baseObj = transform.Find("Base").gameObject;
        Material[] matArray = baseObj.GetComponent<Renderer>().materials;
        matArray[1] = mat;
        baseObj.GetComponent<Renderer>().materials = matArray;
    }

}
