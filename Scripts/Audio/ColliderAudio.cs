using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderAudio : MonoBehaviour
{
    public GameObject colliderObject;
    public string eventName = "";
    public bool characterOnly = true;
    public bool roomAudio = false;

    void Start(){
        //check valid
        if (colliderObject == null) return;
        if (colliderObject.GetComponent<Collider>() == null) return;
        if (colliderObject.GetComponent<AudioTrigger>() != null) return;

        //add audioTrigger to the colliderObject object
        AudioTrigger at = colliderObject.AddComponent<AudioTrigger>();
        //configure
        at.StartAT(characterOnly, eventName, roomAudio);

    }

}
