using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
 * this script is dynamically added and removed
 * from colliders that are attached to travel tasks
 */

public class TravelCollider : MonoBehaviour
{
    public UnityEvent collisionEvent = new UnityEvent();

    //compatible with both triggers and colliders

    void OnTriggerEnter(Collider collision)
    {
        //check if collision is player
        if (collision.gameObject.tag == GameManager.instance.playerTag) {
            //invoke travel event
            collisionEvent.Invoke();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //check if collision is player
        if (collision.gameObject.tag == GameManager.instance.playerTag) {
            //invoke travel event
            collisionEvent.Invoke();
        }
    }
}
