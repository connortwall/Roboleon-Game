using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireObject : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent fireDestroyEvent = new UnityEvent();
    [HideInInspector]
    public bool destroyed = false;

    void OnDestroy(){
        destroyed = true;
        fireDestroyEvent.Invoke();
    }
}
