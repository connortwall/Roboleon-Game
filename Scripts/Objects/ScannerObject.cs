using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class ScannerObject : MonoBehaviour
{
    public List<GameObject> objectsToActivate;
    public GameObject key;
    public bool destroyKeyAfterUse;

    [HideInInspector]
    public UnityEvent scannerEvent = new UnityEvent();

    //item in reach of scanner
    public virtual void OnTriggerEnter(Collider collision){
        //check if collision is key to scanner
        if (key.gameObject.GetInstanceID() == collision.gameObject.GetInstanceID()) {
            OnScan();

        }
    }

    public void OnScan(){
        //beep!
        GameManager.instance.AudioEvent(AudioEvents.instance.scannerBeep);
        //then do stuff :)
        OnActionObject tempObj;
        foreach (GameObject obj in objectsToActivate){
            tempObj = obj.GetComponent<OnActionObject>();
            if (tempObj == null) continue;
            tempObj.OnAction();
        }

        //notify anyone listening that we scanned!
        scannerEvent.Invoke();

        //destroy the key if wanted
        if (destroyKeyAfterUse) Destroy(key);
    }
}
