using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCollider : MonoBehaviour
{
    //var for travel tasks
    [HideInInspector]
    public bool playerCollision = false;
    //send collision event to door
    void OnTriggerEnter(Collider collision)
     {
        //check if collision is player
        if (collision.gameObject.tag == GameManager.instance.playerTag) {
             playerCollision = true;
        }
        
        //send to parent door in case of automatic door functinality
        transform.parent.GetComponent<DoorObject>().ChildCollision();
        //check if is player
     }

     void OnTriggerExit(Collider collision)
    {
       //check if collision is player
        if (collision.gameObject.tag == GameManager.instance.playerTag) {
            playerCollision = false;
        }
    }
}
