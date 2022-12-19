using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Screwdriver : Task
{
    public GameObject screwObject;
    //constructer utilized by TaskSpawner
    public Screwdriver(
        string taskInstructions,
        string DkTaskInstructions,
        bool timed,
        float timeToComplete,
        bool audioOnStart,
        string audioOnStartEvent,
        List<GameObject> onActionAfterCompletion,
        GameObject destination,
        GameObject screwObject)
    {
        //set task type
        base.typeOfTask = TaskType.ButtonPress;

        //generic task vars
        base.taskInstructions = taskInstructions;
        base.DkTaskInstructions = DkTaskInstructions;
        base.timed = timed;
        base.timeToComplete = timeToComplete;
        base.audioOnStart = audioOnStart;
        base.audioOnStartEvent = audioOnStartEvent;
        base.onActionAfterCompletion = onActionAfterCompletion;
        base.destination = destination;

        //screwdriver specific vars
        this.screwObject = screwObject;
    }

    public override void StartTask(){
        //subscribe to the button press event
        ScrewObject screwObj = screwObject.GetComponent<ScrewObject>();
        Assert.IsNotNull(screwObj);
        screwObj.screwEvent.AddListener(CompleteTask);
    }

    public override void UpdateTask(){
    }

    public override void CompleteTask(){
        //unsubscribe
        ScrewObject screwObj = screwObject.GetComponent<ScrewObject>();
        screwObj.screwEvent.RemoveListener(CompleteTask);
        TaskManager.instance.CompleteTask();
    }

    public override bool IsValid(){
        //check base validity
        if (!base.IsValid()) return false;

        if (screwObject == null){
            TaskManager.instance.printTaskError(this, @"No gameobject was assigned");
            return false;
        }
        if (screwObject.GetComponent<ScrewObject>() == null){
            TaskManager.instance.printTaskError(this, @"The game object attached did not have 
            a ScrewObject task assigned to it");
            return false;
        }

        return true;
    }
}
