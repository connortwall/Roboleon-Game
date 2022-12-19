using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Travel : Task
{
    public GameObject destinationCollider;

    //constructer utilized by TaskSpawner
    public Travel(
        string taskInstructions,
        string DkTaskInstructions,
        bool timed,
        float timeToComplete,
        bool audioOnStart,
        string audioOnStartEvent,
        List<GameObject> onActionAfterCompletion,
        GameObject destination,
        GameObject destinationCollider)
    {
        //set task type
        base.typeOfTask = TaskType.Travel;

        //generic task vars
        base.taskInstructions = taskInstructions;
        base.DkTaskInstructions = DkTaskInstructions;
        base.timed = timed;
        base.timeToComplete = timeToComplete;
        base.audioOnStart = audioOnStart;
        base.audioOnStartEvent = audioOnStartEvent;
        base.onActionAfterCompletion = onActionAfterCompletion;
        base.destination = destination;

        //travel specific vars
        this.destinationCollider = destinationCollider;
    }

    public override void StartTask(){
        //add the travel collider script to the collider and register event
        TravelCollider travelCollider = destinationCollider.AddComponent<TravelCollider>();
        travelCollider.collisionEvent.AddListener(CompleteTask);
    }

    public override void UpdateTask(){
    }

    public override void CompleteTask(){
        //remove the travel collider script from the collider 
        Object.Destroy(destinationCollider.GetComponent<TravelCollider>());
        TaskManager.instance.CompleteTask();
    }

    public override bool IsValid(){
        //check base validity
        if (!base.IsValid()) return false;

        if (destinationCollider == null){
            TaskManager.instance.printTaskError(this, @"No gameobject was assigned");
            return false;
        }
        if (destinationCollider.GetComponent<Collider>() == null){
            TaskManager.instance.printTaskError(this, @"The game object assigned does not have a collider");
            return false;
        }

        return true;
    }
}
