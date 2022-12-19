using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingObject : MonoBehaviour
{
    public SlidingAxis slidingAxis = SlidingAxis.x;
    public float movementSpeed = 2f;
    public float distance = 10f;
    public float waitTime = 2f;
    public bool reverse = false;
    

    [HideInInspector]
    public Vector3 beginningPosition; 
    [HideInInspector]
    public Vector3 direction;
    [HideInInspector]
    public Vector3 reverseDirection;
    [HideInInspector]
    public float timeAtStop;

    void Start(){
        timeAtStop = 0;
        beginningPosition = transform.position;
        switch(slidingAxis){
            case SlidingAxis.x:
                direction = Vector3.right;
                reverseDirection = Vector3.left;
                break;
            case SlidingAxis.y:
                direction = Vector3.up;
                reverseDirection = Vector3.down;
                break;
            case SlidingAxis.z:
                direction = Vector3.forward;
                reverseDirection = Vector3.back;
                break;
            default:
                Debug.LogError("Sliding object was not configured correctly.");
                break;
        }//end switch
    }

    // Update is called once per frame
    void Update()
    {
        //wait at ends if needed
        if (waitTime > 0 && timeAtStop > waitTime)
            timeAtStop = -1;
        else if (waitTime > 0 && timeAtStop >= 0){
            timeAtStop += Time.deltaTime;
            return;
        }
        //swap directions when you get to the destination
        if (Vector3.Distance(transform.position, beginningPosition) >= distance){
            reverse = !reverse;
            beginningPosition = transform.position;
            timeAtStop = 0f;
        }
        //move
        if (!reverse) transform.position += direction * Time.deltaTime * movementSpeed;
        else transform.position += reverseDirection * Time.deltaTime * movementSpeed;
        
    }
}

public enum SlidingAxis {
    x,
    y,
    z
}
