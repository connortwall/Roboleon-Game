using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScrewObject : MonoBehaviour
{
    public List<GameObject> objectsToActivate;
    [HideInInspector]
    public UnityEvent screwEvent = new UnityEvent();

    public void OnScrewFinish()
    {
        //activate all OnActionObjects
        OnActionObject tempObj;
        foreach (GameObject obj in objectsToActivate){
            tempObj = obj.GetComponent<OnActionObject>();
            if (tempObj == null) continue;
            tempObj.OnAction();
        }

        //notify anyone listening that we scanned!
        screwEvent.Invoke();

    }
}
