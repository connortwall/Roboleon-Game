using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : Task
{
    
    public GameObject doorPrefabToOpen;

    //constructer utilized by TaskSpawner
    public DoorOpen(
        string taskInstructions,
        string DkTaskInstructions,
        bool timed,
        float timeToComplete,
        bool audioOnStart,
        string audioOnStartEvent,
        List<GameObject> onActionAfterCompletion,
        GameObject destination,
        GameObject doorPrefabToOpen)
    {
        //set task type
        base.typeOfTask = TaskType.DoorOpen;

        //generic task vars
        base.taskInstructions = taskInstructions;
        base.DkTaskInstructions = DkTaskInstructions;
        base.timed = timed;
        base.timeToComplete = timeToComplete;
        base.audioOnStart = audioOnStart;
        base.audioOnStartEvent = audioOnStartEvent;
        base.onActionAfterCompletion = onActionAfterCompletion;
        base.destination = destination;

        //open door specific vars
        this.doorPrefabToOpen = doorPrefabToOpen;
    }

    public override void StartTask(){
        DoorObject doorScript = doorPrefabToOpen.GetComponent<DoorObject>();
        //if the door is already open, complete task
        if (doorScript.isOpen) CompleteTask();
        doorScript.openEvent.AddListener(CompleteTask);
    }

    public override void UpdateTask(){
    }

    public override void CompleteTask(){
        DoorObject doorScript = doorPrefabToOpen.GetComponent<DoorObject>();
        doorScript.openEvent.RemoveListener(CompleteTask);
        TaskManager.instance.CompleteTask();
    }    
    
    public override bool IsValid(){
        //check base validity
        if (!base.IsValid()) return false;

        if (doorPrefabToOpen == null){
            TaskManager.instance.printTaskError(this, @"doorPrefabToOpen was not assigned");
            return false;
        }
        //make sure gameobject is of type doorobject
        if (doorPrefabToOpen.GetComponent<DoorObject>() == null) {
            TaskManager.instance.printTaskError(this, @"doorPrefabToOpen did not have a DoorObject 
            script attached");
            return false;
        }

        return true;
    }

/************ DEBUG FUNCTIONS BELOW **************/
    public override void DebugCompleteTask(){
        if (!IsValid()){
            CompleteTask();
            return;
        }
        
        DoorObject doorScript = doorPrefabToOpen.GetComponent<DoorObject>();
        //make sure is unlocked/open
        doorScript.OnAction();
        //complete task
        CompleteTask();
    }
}
