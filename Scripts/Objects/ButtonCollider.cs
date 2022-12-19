using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCollider : MonoBehaviour
{
    public GameObject button;
    public ButtonObject buttonObject;

    void OnCollisionEnter(Collision collision)
    {
        if (button == null) return;
        //make sure the collider is the button
        if (collision.gameObject.GetInstanceID() != button.gameObject.GetInstanceID())
        {
            Physics.IgnoreCollision(collision.collider, this.GetComponent<Collider>());
        }
        else {
            buttonObject.OnPress();
        }
    }
}
