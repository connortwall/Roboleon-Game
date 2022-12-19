using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleObject : OnActionObject
{
    public bool activateOnlyOnce = false;
    public List<GameObject> toggleObjects = new List<GameObject>();

    [HideInInspector]
    public bool onActionCompleted = false;

    public override void OnAction(){
        if (activateOnlyOnce && onActionCompleted) return;
        if (toggleObjects == null) return;
        if (toggleObjects.Count <= 0) return;
        //only allow activation a single time
        foreach (GameObject toggleObj in toggleObjects){
            if (toggleObj.activeSelf) toggleObj.SetActive(false);
            else toggleObj.SetActive(true);
        }
        onActionCompleted = true;
    }
}
