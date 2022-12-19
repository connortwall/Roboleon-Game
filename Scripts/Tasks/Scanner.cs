using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Scanner : Task
{
    public GameObject scanner;

    //constructer utilized by TaskSpawner
    public Scanner(
        string taskInstructions,
        string DkTaskInstructions,
        bool timed,
        float timeToComplete,
        bool audioOnStart,
        string audioOnStartEvent,
        List<GameObject> onActionAfterCompletion,
        GameObject destination,
        GameObject scanner)
    {
        //set task type
        base.typeOfTask = TaskType.Scanner;

        //generic task vars
        base.taskInstructions = taskInstructions;
        base.DkTaskInstructions = DkTaskInstructions;
        base.timed = timed;
        base.timeToComplete = timeToComplete;
        base.audioOnStart = audioOnStart;
        base.audioOnStartEvent = audioOnStartEvent;
        base.onActionAfterCompletion = onActionAfterCompletion;
        base.destination = destination;

        //scanner specific vars
        this.scanner = scanner;
    }

    public override void StartTask(){
        //subscribe to the button press event
        ScannerObject scannerObj = scanner.GetComponent<ScannerObject>();
        Assert.IsNotNull(scannerObj);
        scannerObj.scannerEvent.AddListener(CompleteTask);
    }

    public override void UpdateTask(){
    }

    public override void CompleteTask(){
        //unsubscribe
        ScannerObject scannerObj = scanner.GetComponent<ScannerObject>();
        scannerObj.scannerEvent.RemoveListener(CompleteTask);
        TaskManager.instance.CompleteTask();
    }

    public override bool IsValid(){
        //check base validity
        if (!base.IsValid()) return false;

        if (scanner == null) {
            TaskManager.instance.printTaskError(this, @"A scanner object was not assigned");
            return false;
        }
        if (scanner.GetComponent<ScannerObject>() == null){
            TaskManager.instance.printTaskError(this, @"The scanner object did not have a 
            ScannerObject script attached to it");
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
        //pretend that you scanned the thing
        ScannerObject scannerObj = scanner.GetComponent<ScannerObject>();
        scannerObj.OnScan();
    }
}
