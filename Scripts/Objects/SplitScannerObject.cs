using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitScannerObject : ScannerObject
{
    public GameObject badKey;

    //item in reach of scanner
    public override void OnTriggerEnter(Collider collision){
        //check if collision is key to scanner
        if (key.gameObject.GetInstanceID() == collision.gameObject.GetInstanceID()) {
            OnScan();

        }
        else if (badKey.gameObject.GetInstanceID() == collision.gameObject.GetInstanceID()){
            OnScanBad();
        }
    }

    //split the task system!!!!!!!!
    public void OnScanBad(){
        TaskManager.instance.AlternateEnding();
        //destroy the bad key if wanted
        if (destroyKeyAfterUse) Destroy(badKey);
    }


}
