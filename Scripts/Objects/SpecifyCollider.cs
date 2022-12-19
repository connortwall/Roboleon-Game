using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SpecifyCollider : MonoBehaviour
{
    public GameObject onlyCollideWith;
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetInstanceID() != onlyCollideWith.gameObject.GetInstanceID())
        {
            Physics.IgnoreCollision(collision.collider, this.GetComponent<Collider>());
        }
    }
}