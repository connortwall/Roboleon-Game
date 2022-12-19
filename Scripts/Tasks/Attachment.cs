using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puppet.ReviewScripts;

public class Attachment : Task
{
    public GameObject toolToGrab;
    //constructer utilized by TaskSpawner
    public Attachment(
        string taskInstructions,
        string DkTaskInstructions,
        bool timed,
        float timeToComplete,
        bool audioOnStart,
        string audioOnStartEvent,
        List<GameObject> onActionAfterCompletion,
        GameObject destination,
        GameObject toolToGrab)
    {
        //set task type
        base.typeOfTask = TaskType.Attachment;

        //generic task vars
        base.taskInstructions = taskInstructions;
        base.DkTaskInstructions = DkTaskInstructions;
        base.timed = timed;
        base.timeToComplete = timeToComplete;
        base.audioOnStart = audioOnStart;
        base.audioOnStartEvent = audioOnStartEvent;
        base.onActionAfterCompletion = onActionAfterCompletion;
        base.destination = destination;

        //change tool specific vars
        this.toolToGrab = toolToGrab;
    }

    public override void StartTask(){
    }

    public override void UpdateTask(){
    }

    //game manager handles all of this logic, so just call for the next task.
    public override void CompleteTask(){
        TaskManager.instance.CompleteTask();
    }

    public override bool IsValid(){
        //check base validity
        if (!base.IsValid()) return false;

        if (toolToGrab == null){
            TaskManager.instance.printTaskError(this, @"No toolToGrab object was assigned");
            return false;
        }
        if (toolToGrab.GetComponent<AttachmentAssetObject>() == null){
            TaskManager.instance.printTaskError(this, @"The toolToGrab game object did not 
            have an AttachmentAssetObjects script attached");
            return false;
        }

        return true;
    }

/************ DEBUG FUNCTIONS BELOW **************/
    public override void DebugCompleteTask(){
        if (!IsValid() || GameManager.instance.unlockTools){
            CompleteTask();
            return;
        }
        //make sure not already unlocked 
        ArmMode armMode = toolToGrab.GetComponent<AttachmentAssetObject>().attachmentType;
        if (GameManager.instance.toolKeeper.Contains(armMode)){
            CompleteTask();
            return;
        }
        //force unlock the tool
        GameManager.instance.attachmentInRange = toolToGrab;
        GameManager.instance.UnlockAttachment();
    }
}
