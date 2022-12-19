using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Firefighter : Task
{
    public bool lockDoors;
    public List<GameObject> doorsToLock;
    public List<GameObject> fireSpawners;

    [HideInInspector]
    public List<GameObject> fires;

    //constructer utilized by TaskSpawner
    public Firefighter(
        string taskInstructions,
        string DkTaskInstructions,
        bool timed,
        float timeToComplete,
        bool audioOnStart,
        string audioOnStartEvent,
        List<GameObject> onActionAfterCompletion,
        GameObject destination,
        bool lockDoors,
        List<GameObject> doorsToLock,
        List<GameObject> fireSpawners)
    {
        //set task type
        base.typeOfTask = TaskType.Firefighter;

        //generic task vars
        base.taskInstructions = taskInstructions;
        base.DkTaskInstructions = DkTaskInstructions;
        base.timed = timed;
        base.timeToComplete = timeToComplete;
        base.audioOnStart = audioOnStart;
        base.audioOnStartEvent = audioOnStartEvent;
        base.onActionAfterCompletion = onActionAfterCompletion;
        base.destination = destination;

        //firefighter specific vars
        this.lockDoors = lockDoors;
        this.doorsToLock = doorsToLock;
        this.fireSpawners = fireSpawners;
    }

    public override void StartTask(){
        //start the fires and keep track of them
        fires = new List<GameObject>();

        //spawn each firespawner's fires then add them to our fire list
        FireSpawner fs;
        foreach (GameObject fireSpawnerObj in fireSpawners){
            fs = fireSpawnerObj.GetComponent<FireSpawner>();
            List<GameObject> newFires = fs.StartFire();
            if (newFires == null) continue;
            fires.AddRange(newFires);
        }
        if (fires.Count <= 0)
        {
            if (TaskManager.instance.debugMode) UIManager.instance.UpdateTaskPanel("INVALID TASK: Firespawner not configured correctly (no fires spawned). Press T to continue.");
            TaskManager.instance.currTask = null;
            return;
        }
        //Assert.IsTrue(fires.Count > 0);

        //subscribe to the fire destroy events
        FireObject fireObj;
        foreach (GameObject fire in fires){
            fireObj = fire.GetComponent<FireObject>();
            fireObj.fireDestroyEvent.AddListener(FireDestroyed);
        }

        //lock the doors if that is a part of the task
        if (!lockDoors) return;

        DoorObject door;
        foreach(GameObject doorObj in doorsToLock){
            door = doorObj.GetComponent<DoorObject>();
            door.FirefighterTask(true);
        }
    }

    //called each time a fire is destroyed
    public void FireDestroyed(){
        //remove any null fires from the list
        fires.RemoveAll(fire => (fire == null || fire.GetComponent<FireObject>().destroyed));
        //if there's no fires left, the fire task is complete!
        if (fires == null || fires.Count <= 0){
            CompleteTask();
        }
    }

    public override void UpdateTask(){
    }

    public override void CompleteTask(){
        //unlock all the doors
        if (lockDoors){
            DoorObject door;
            foreach(GameObject doorObj in doorsToLock){
                door = doorObj.GetComponent<DoorObject>();
                door.FirefighterTask(false);
            }
        }

        TaskManager.instance.CompleteTask();
    }

    public override bool IsValid(){
        //check base validity
        if (!base.IsValid()) return false;

        if(lockDoors){
            if (doorsToLock == null || doorsToLock.Count <= 0){
                TaskManager.instance.printTaskError(this, @"lockDoors was checked, but 
                there were no doors in the doorsToLock list");
                return false;
            }
            //make sure doors are actually doors
            foreach (GameObject doorObj in doorsToLock){
                if (doorObj.GetComponent<DoorObject>() == null) {
                    TaskManager.instance.printTaskError(this, @"one of the doorsToLock 
                    did not have a DoorObject script attached to it");
                    return false;
                }
            }
        }
        
        if (fireSpawners == null || fireSpawners.Count <= 0){
            TaskManager.instance.printTaskError(this, @"there were no fire spawners in 
            the fireSpawners list");
            return false;
        }
        //make sure fireSpawners are actually firespawners
        foreach (GameObject fsObj in fireSpawners){
            if (fsObj == null)return false;
            if (fsObj.GetComponent<FireSpawner>() == null) {
                TaskManager.instance.printTaskError(this, @"A fire spawner in the firespawners 
                list did not have a FireSpawner script attached to it");
                return false;
            }
        }

        return true;
    }

/************ DEBUG FUNCTIONS BELOW **************/
    public override void DebugCompleteTask(){
        if (!IsValid() || fires == null){
            CompleteTask();
            return;
        }

        foreach (GameObject fire in fires){
            if (fire == null) continue;
            Object.Destroy(fire);
        }
    }
}
